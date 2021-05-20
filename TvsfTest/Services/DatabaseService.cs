using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TvsfTest.Models;

namespace TvsfTest.Services
{
    public static class DatabaseService
    {
        static string ConnectionString => ConfigurationManager.ConnectionStrings["MainConnection"].ConnectionString;

        public static async Task<(DataContext, IEnumerable<OrganizationModel>)> GetOrganizations()
        {
            return await Task.Run(() =>
            {
                var db = new DataContext(ConnectionString);
                return (db, db.GetTable<OrganizationModel>().ToArray());
            });
        }

        public static async Task<(DataContext, IEnumerable<EmployeeModel>)> GetEmployees(OrganizationModel organization)
        {
            return await Task.Run(() =>
            {
                var db = new DataContext(ConnectionString);
                return (db, db.GetTable<EmployeeModel>().Where(e => e.OrganizationId == organization.Id).ToArray());
            });
        }

        public static async Task BulkCopy(IEnumerable<EmployeeModel> employees, OrganizationViewModel targetOrganization)
        {
            var table = new DataTable();
            table.Columns.Add("organization_id", typeof(int));
            table.Columns.Add("surname", typeof(string));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("middle_name", typeof(string));
            table.Columns.Add("birth_date", typeof(DateTime));
            table.Columns.Add("passport_number", typeof(long));
            table.Columns.Add("description", typeof(string));

            foreach (var e in employees)
            {
                var row = table.NewRow();
                row.ItemArray = new object[] { targetOrganization.Id, e.Surname, e.Name, e.MiddleName, e.BirthDate, e.PassportNumber, e.Description };
                table.Rows.Add(row);
            }

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                await new SqlCommand("TRUNCATE TABLE dbo.employee_stg;", connection).ExecuteNonQueryAsync();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "dbo.employee_stg";
                    bulkCopy.WriteToServer(table);
                }

                await new SqlCommand("MERGE dbo.employee AS dest "
                    + "USING dbo.employee_stg AS source "
                    + "ON dest.surname = source.surname "
                    + "    AND dest.name = source.name "
                    + "    AND dest.middle_name = source.middle_name "
                    + "    AND dest.birth_date = source.birth_date "
                    + "    AND dest.passport_number = source.passport_number "
                    + "WHEN MATCHED THEN "
                    + "    UPDATE SET "
                    + "        organization_id = source.organization_id, "
                    + "        description = source.description "
                    + "WHEN NOT MATCHED THEN "
                    + "    INSERT (organization_id, surname, name, middle_name, birth_date, passport_number, description) "
                    + "    VALUES (source.organization_id, source.surname, source.name, source.middle_name, source.birth_date, source.passport_number, source.description) "
                    + $"WHEN NOT MATCHED BY SOURCE AND dest.organization_id = {targetOrganization.Id} THEN "
                    + "    DELETE;", connection).ExecuteNonQueryAsync();
            }
        }
    }
}
