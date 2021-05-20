using CsvHelper.Configuration;
using System;
using System.Data.Linq.Mapping;

namespace TvsfTest.Models
{
    [Table(Name = "employee")]
    public class EmployeeModel
    {
        [Column(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(Name = "organization_id")]
        public int OrganizationId;

        [Column(Name = "surname")]
        public string Surname;

        [Column(Name = "name")]
        public string Name;

        [Column(Name = "middle_name")]
        public string MiddleName;

        [Column(Name = "birth_date")]
        public DateTime BirthDate;

        [Column(Name = "passport_number")]
        public long PassportNumber;

        [Column(Name = "description")]
        public string Description;
    }

    public class EmployeeMap : ClassMap<EmployeeModel>
    {
        public EmployeeMap()
        {
            //Map(m => m.Id).Name("Id");
            Map(m => m.OrganizationId).Name("OrganizationId");
            Map(m => m.Surname).Name("Surname");
            Map(m => m.Name).Name("Name");
            Map(m => m.MiddleName).Name("MiddleName");
            Map(m => m.BirthDate).Name("BirthDate");
            Map(m => m.PassportNumber).Name("PassportNumber");
            Map(m => m.Description).Name("Description");
        }
    }
}
