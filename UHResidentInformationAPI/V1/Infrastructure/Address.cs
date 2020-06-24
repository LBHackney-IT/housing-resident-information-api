using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("property")]
    public class Address
    {
        [Column("prop_ref")]
        [MaxLength(9)]
        [Key]
        public int PropertyRef { get; set; }

        [Column("house_ref")]
        [MaxLength(9)]
        public int HouseRef { get; set; }

        [Column("address1")]
        [MaxLength(9)]
        public string AddressLine1 { get; set; }

        [Column("address2")]
        [MaxLength(9)]
        public string AddressLine2 { get; set; }

        [Column("address3")]
        [MaxLength(9)]
        public string AddressLine3 { get; set; }

        [Column("post_code")]
        [MaxLength(9)]
        public string PostCode { get; set; }
    }
}
