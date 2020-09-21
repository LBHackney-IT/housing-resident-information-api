using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingResidentInformationAPI.V1.Infrastructure
{
    [Table("contacts", Schema = "dbo")]
    public class Contact
    {
        [Column("con_key")]
        [Key]
        public int ContactKey { get; set; }

        [Column("tag_ref")]
        [MaxLength(11)]
        public string TagRef { get; set; }

        [Column("con_ref")]
        [MaxLength(12)]
        public string HouseReference { get; set; }
    }
}
