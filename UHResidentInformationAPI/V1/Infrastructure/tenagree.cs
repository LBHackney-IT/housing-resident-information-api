using System;
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
        [StringLength(11)]
        public string TagRef { get; set; }

        [Column("spec_terms")]
        public Boolean SpecTerms { get; set; }

        [Column("other_accounts")]
        public Boolean OtherAccounts { get; set; }

        [Column("active")]
        public Boolean Active { get; set; }

        [Column("present")]
        public Boolean Present { get; set; }

        [Column("terminated")]
        public Boolean Terminated { get; set; }

        [Column("free_active")]
        public Boolean FreeActive { get; set; }

        [Column("nop")]
        public Boolean Nop { get; set; }

        [Column("additional_debit")]
        public Boolean AdditionalDebit { get; set; }

        [Column("fd_charge")]
        public Boolean FdCharge { get; set; }

        [Column("receiptcard")]
        public Boolean ReceiptCard { get; set; }

        [Column("nosp")]
        public Boolean Nosp { get; set; }

        [Column("ntq")]
        public Boolean Ntq { get; set; }

        [Column("eviction")]
        public Boolean Eviction { get; set; }

        [Column("committee")]
        public Boolean Committee { get; set; }

        [Column("suppossorder")]
        public Boolean Suppossorder { get; set; }

        [Column("possorder")]
        public Boolean Possorder { get; set; }

        [Column("courtapp")]
        public Boolean Courtapp { get; set; }

        [Column("open_item")]
        public Boolean OpenItem { get; set; }

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