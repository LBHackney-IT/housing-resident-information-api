using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("CCEmailAddress")]
    public class EmailAddresses
    {
        [Column("EmailID")]
        [MaxLength(9)]
        [Key]
        public int EmailId { get; set; }

        [Column("ContactNo")]
        [MaxLength(9)]
        public int ContactID { get; set; }

        [Column("Email")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Column("EmailType")]
        [MaxLength(5)]
        public string EmailType { get; set; }

        [Column("ModDate")]
        public DateTime DateModified { get; set; }

    }
}
