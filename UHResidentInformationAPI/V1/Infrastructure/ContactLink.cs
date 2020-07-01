using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("cccontactlink")]
    public class ContactLink
    {
        [Column("contactno")]
        [Key]
        public int ContactID { get; set; }

        [Column("key1")]
        public string TagRef { get; set; }
    }
}
