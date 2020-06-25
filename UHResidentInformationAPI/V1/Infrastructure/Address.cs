using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("property")]
    public class Address
    {
        [Column("prop_ref")]
        [MaxLength(12)]
        [Key]
        public string PropertyRef { get; set; }

        [Column("house_ref")]
        [MaxLength(10)]
        public string HouseRef { get; set; }

        [Column("address1")]
        [MaxLength(255)]
        public string AddressLine1 { get; set; }

        [Column("post_code")]
        [MaxLength(10)]
        public string PostCode { get; set; }
    }
}
