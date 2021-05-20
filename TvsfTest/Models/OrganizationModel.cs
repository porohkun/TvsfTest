using System.Data.Linq.Mapping;

namespace TvsfTest.Models
{
    [Table(Name = "organization")]
    public class OrganizationModel
    {
        [Column(Name = "id", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id;

        [Column(Name = "name")]
        public string Name;

        [Column(Name = "inn")]
        public long INN;

        [Column(Name = "legal_address")]
        public string LegalAddress;

        [Column(Name = "physical_address")]
        public string PhysicalAddress;

        [Column(Name = "description")]
        public string Description;
    }
}
