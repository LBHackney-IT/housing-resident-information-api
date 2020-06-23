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
        public int propertyRef { get; set; }

        [Column("house_ref")]
        [MaxLength(9)]
        public int houseRef { get; set; } 

        [Column("address1")]
        [MaxLength(9)]
        public int addressLine1 { get; set; } 

        [Column("address2")]
        [MaxLength(9)]
        public int addressLine2 { get; set; } 

        [Column("address3")]
        [MaxLength(9)]
        public int addressLine3 { get; set; } 

        [Column("post_code")]
        [MaxLength(9)]
        public int postCode { get; set; } 
    }
}