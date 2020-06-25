using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("CCPhone")]
    public class TelephoneNumber
    {
        [Column("PhoneID")]
        [MaxLength(32)]
        [Key]
        public int PhoneID { get; set; }

        [Column("ContactNo")]
        [MaxLength(32)]
        public int ContactID { get; set; }

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
