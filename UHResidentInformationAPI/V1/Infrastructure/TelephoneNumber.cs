using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHResidentInformationAPI.V1.Enums;

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
        public string Type { get; set; }

        [Column("moddate")]
        public DateTime DateCreated { get; set; }

        [Column("phoneid")]
        [Key]
        public int PhoneId { get; set; }

        [Column("moduser")]
        [MaxLength(20)]
        public string ModUser { get; set; }

        [Column("modtype")]
        [StringLength(1)]
        public string ModType { get; set; }
    }
}
