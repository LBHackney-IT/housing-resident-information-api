using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("ccphone")]
    public class TelephoneNumber
    {
        [Column("phoneid")]
        [Key]
        public int PhoneID { get; set; }

        [Column("contactno")]
        [MaxLength(32)]
        public int ContactID { get; set; }

        [Column("phoneno")]
        [MaxLength(20)]
        public string Number { get; set; }


        [Column("phonetype")]
        [MaxLength(1)]
        public string Type { get; set; }

        [Column("moddate")]
        public DateTime DateCreated { get; set; }

    }
}
