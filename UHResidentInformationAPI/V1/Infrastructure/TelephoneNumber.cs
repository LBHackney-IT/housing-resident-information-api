using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("CCPhone")]
    public class TelephoneNumber
    {
        [Column("ContactNo")]
        [MaxLength(32)]
        [Key]
        public string ContactID { get; set; }

        [Column("PhoneNo")]
        [MaxLength(80)]
        public string Number { get; set; }

        
        [Column("PhoneType")]
        [MaxLength(80)]
        public string Type { get; set; }

        [Column("ModDate")]
        [MaxLength(80)]
        public DateTime DateCreated { get; set; }
        
    }
}