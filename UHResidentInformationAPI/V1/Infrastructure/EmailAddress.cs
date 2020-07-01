using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("ccemailaddress")]
    public class EmailAddresses
    {
        [Column("contactno")]
        public int ContactID { get; set; }

        [Column("email")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Column("emailtype")]
        [MaxLength(5)]
        public string EmailType { get; set; }

        [Column("oktoemail")]
        [MaxLength(5)]
        public string OkToEmail { get; set; }

        [Column("defualt")]
        [MaxLength(5)]
        public string Default { get; set; }

        [Column("emailid")]
        [Key]
        public int EmailId { get; set; }

        [Column("moddate")]
        public DateTime DateModified { get; set; }

        [Column("moduser")]
        [MaxLength(20)]
        public string ModUser { get; set; }

        [Column("modtype")]
        [StringLength(1)]
        public string ModType { get; set; }
    }
}
