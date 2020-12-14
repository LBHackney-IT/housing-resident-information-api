using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingResidentInformationAPI.V1.Infrastructure
{
    [Table("tenure", Schema = "dbo")]
    public class UhTenureType
    {
        [StringLength(3)]
        [Key]
        [Column("ten_type")] public string UhTenureTypeId { get; set; }
        [StringLength(15)]
        [Column("ten_desc")] public string Description { get; set; }
    }
}
