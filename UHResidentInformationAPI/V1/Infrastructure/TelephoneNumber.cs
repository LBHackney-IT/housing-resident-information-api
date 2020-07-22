using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UHResidentInformationAPI.V1.Enums;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("CCPhone", Schema = "dbo")]
    public class TelephoneNumber
    {
        [Column("PhoneID")]
        [Key]
        public int PhoneId { get; set; }

        [Column("ContactNo")]
        public int ContactID { get; set; }

        [Column("PhoneNo")]
        [MaxLength(20)]
        public string Number { get; set; }

        [Column("PhoneType")]
        public string Type { get; set; }

        [Column("modDate")]
        public DateTime DateCreated { get; set; }

        [Column("modUser")]
        [MaxLength(20)]
        public string ModUser { get; set; }

        [Column("modType")]
        [StringLength(1)]
        public string ModType { get; set; }
    }
}
