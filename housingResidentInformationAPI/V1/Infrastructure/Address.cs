using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingResidentInformationAPI.V1.Infrastructure
{
    [Table("property", Schema = "dbo")]
    public class Address
    {
        [Column("prop_ref")]
        [StringLength(12)]
        [Key]
        public string PropertyRef { get; set; }

        [Column("house_ref")]
        [StringLength(10)]
        public string HouseRef { get; set; }

        [Column("address1")]
        [StringLength(255)]
        public string AddressLine1 { get; set; }

        [Column("post_code")]
        [StringLength(10)]
        public string PostCode { get; set; }

        [Column("u_llpg_ref")]
        [StringLength(16)]
        public string UPRN { get; set; }

        [Column("managed_property")]
        public Boolean ManagedProperty { get; set; }

        [Column("ownership")]
        [StringLength(10)]
        public string Ownership { get; set; }

        [Column("letable")]
        public Boolean Letable { get; set; }

        [Column("lounge")]
        public Boolean Lounge { get; set; }

        [Column("laundry")]
        public Boolean Laundry { get; set; }

        [Column("visitor_bed")]
        public Boolean VisitorBed { get; set; }

        [Column("store")]
        public Boolean Store { get; set; }

        [Column("warden_flat")]
        public Boolean WardenFlat { get; set; }

        [Column("sheltered")]
        public Boolean Sheltered { get; set; }

        [Column("shower")]
        public Boolean Shower { get; set; }

        [Column("rtb")]
        public Boolean Rtb { get; set; }

        [Column("core_shared")]
        public Boolean CoreShared { get; set; }

        [Column("asbestos")]
        public Boolean Asbestos { get; set; }

        [Column("no_single_beds")]
        public int NoSingleBeds { get; set; }

        [Column("no_double_beds")]
        public int NoDoubleBeds { get; set; }

        [Column("online_repairs")]
        public Boolean OnlineRepairs { get; set; }

        [Column("repairable")]
        public Boolean Repairable { get; set; }

        [Column("dtstamp")]
        public DateTime Dtstamp { get; set; }
    }
}
