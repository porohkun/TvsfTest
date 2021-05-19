using System;
using System.Data.Linq.Mapping;

namespace TvsfTest.Models
{
    [Table(Name = "employees")]
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
}
