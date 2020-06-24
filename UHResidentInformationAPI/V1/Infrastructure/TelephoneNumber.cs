using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("ccphone")]
    public class TelephoneNumber
    {
        [Column("contactno")]
        public int ContactID { get; set; }

        [Column("phoneno")]
        [MaxLength(20)]
        public string Number { get; set; }

        [Column("phonetype")]
        [MaxLength(1)]
        public string Type { get; set; }

        [Column("moddate")]
        [MaxLength(3)]
        public DateTime DateCreated { get; set; }

        [Column("phoneid")]
        [Key]
        public int PhoneId { get; set; }

        [Column("moduser")]
        [MaxLength(20)]
        public string ModUser { get; set; }

        [Column("modtype")]
        [MaxLength(1)]
        public string ModType { get; set; }
    }
}
