using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("tenagree")]
    public class TenancyAgreement
    {
        [Column("house_ref")]
        [StringLength(10)]
        public string HouseRef { get; set; }

        [Column("tag_ref")]
        [Key]
        public string TagRef { get; set; }
    }
}
