using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("CCContactLink", Schema = "dbo")]
    public class ContactLink
    {
        [Column("ContactNo")]
        [Key]
        public int ContactID { get; set; }

        [Column("Key1")]
        [StringLength(20)]
        public string TagRef { get; set; }

        [Column("Key2")]
        [StringLength(10)]
        public string PersonNo { get; set; }

        [Column("LinkType")]
        [MaxLength(40)]
        public string LinkType { get; set; }

        [Column("LinkNo")]
        public int LinkNo { get; set; }

        [Column("MODTYPE")]
        [MaxLength(1)]
        public string MODTYPE { get; set; }
    }
}
