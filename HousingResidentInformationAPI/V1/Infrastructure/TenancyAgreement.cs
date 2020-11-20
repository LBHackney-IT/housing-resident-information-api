using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HousingResidentInformationAPI.V1.Infrastructure
{
    [Table("tenagree", Schema = "dbo")]
    public class TenancyAgreement
    {
        [Column("house_ref")]
        [StringLength(10)]
        public string HouseRef { get; set; }

        [Column("tag_ref")]
        [Key]
        [StringLength(11)]
        public string TagRef { get; set; }

        [Column("spec_terms")]
        public bool SpecTerms { get; set; }

        [Column("other_accounts")]
        public bool OtherAccounts { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("present")]
        public bool Present { get; set; }

        [Column("terminated")]
        public bool IsTerminated { get; set; }

        [Column("free_active")]
        public bool FreeActive { get; set; }

        [Column("nop")]
        public bool Nop { get; set; }

        [Column("additional_debit")]
        public bool AdditionalDebit { get; set; }

        [Column("fd_charge")]
        public bool FdCharge { get; set; }

        [Column("receiptcard")]
        public bool ReceiptCard { get; set; }

        [Column("nosp")]
        public bool Nosp { get; set; }

        [Column("ntq")]
        public bool Ntq { get; set; }

        [Column("eviction")]
        public bool Eviction { get; set; }

        [Column("committee")]
        public bool Committee { get; set; }

        [Column("suppossorder")]
        public bool Suppossorder { get; set; }

        [Column("possorder")]
        public bool Possorder { get; set; }

        [Column("courtapp")]
        public bool Courtapp { get; set; }

        [Column("open_item")]
        public bool OpenItem { get; set; }

        [Column("potentialenddate")]
        public DateTime Potentialenddate { get; set; }

        [Column("u_payment_expected")]
        [StringLength(3)]
        public string UPaymentExpected { get; set; }

        [Column("dtstamp")]
        public DateTime Dtstamp { get; set; }

        [Column("intro_date")]
        public DateTime IntroDate { get; set; }

        [Column("intro_ext_date")]
        public DateTime IntroExtDate { get; set; }
    }
}
