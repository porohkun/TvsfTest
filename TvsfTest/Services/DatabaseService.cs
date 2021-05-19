using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
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
                return (db, db.GetTable<OrganizationModel>());
            });
        }

        public static async Task<(DataContext, IEnumerable<EmployeeModel>)> GetEmployees(OrganizationModel organization)
        {
            return await Task.Run(() =>
            {
                var db = new DataContext(ConnectionString);
                return (db, db.GetTable<EmployeeModel>().Where(e => e.OrganizationId == organization.Id));
            });
        }
    }
}
