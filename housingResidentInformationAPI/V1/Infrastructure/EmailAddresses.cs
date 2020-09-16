using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace housingResidentInformationAPI.V1.Infrastructure
{
    [Table("CCEmailAddress", Schema = "dbo")]
    public class EmailAddresses
    {
        [Column("ContactNo")]
        public int ContactID { get; set; }

        [Column("Email")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Column("EmailType")]
        [MaxLength(5)]
        public string EmailType { get; set; }

        [Column("OKToEmail")]
        [MaxLength(5)]
        public string OkToEmail { get; set; }

        [Column("Defualt")]
        [MaxLength(5)]
        public string Default { get; set; }

        [Column("EmailID")]
        [Key]
        public int EmailId { get; set; }

        [Column("modDate")]
        public DateTime DateModified { get; set; }

        [Column("modUser")]
        [MaxLength(20)]
        public string ModUser { get; set; }

        [Column("modType")]
        [StringLength(1)]
        public string ModType { get; set; }
    }
}
