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
        [StringLength(20)]
        public string TagRef { get; set; }

        [Column("key2")]
        [StringLength(10)]
        public string PersonNo { get; set; }

        [Column("linktype")]
        [MaxLength(40)]
        public string LinkType { get; set; }

        [Column("linkno")]
        public int LinkNo { get; set; }

        [Column("modtype")]
        [MaxLength(1)]
        public string MODTYPE { get; set; }
    }
}
