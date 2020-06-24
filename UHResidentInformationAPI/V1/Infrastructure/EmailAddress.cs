using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("ccemailaddress")]
    public class EmailAddresses
    {
        [Column("emailid")]
        [MaxLength(9)]
        [Key]
        public int EmailId { get; set; }

        [Column("contactno")]
        [MaxLength(9)]
        public int ContactID { get; set; }

        [Column("email")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Column("emailtype")]
        [MaxLength(5)]
        public string EmailType { get; set; }

        [Column("moddate")]
        public DateTime DateModified { get; set; }

    }
}
