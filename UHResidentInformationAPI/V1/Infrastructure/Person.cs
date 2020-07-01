using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("member")]
    public class Person
    {
        [Column("house_ref")]
        [StringLength(10)]
        public string HouseRef { get; set; }

        [Column("person_no")]
        [Range(-99, 99)]
        public int PersonNo { get; set; }

        [Column("title")]
        [MaxLength(10)]
        public string Title { get; set; }

        [Column("forename")]
        [StringLength(24)]
        public string FirstName { get; set; }

        [Column("surname")]
        [StringLength(20)]
        public string LastName { get; set; }

        [Column("ni_no")]
        [StringLength(12)]
        public string NINumber { get; set; }

        [Column("dob")]
        public DateTime DateOfBirth { get; set; }

        [Column("oap")]
        public Boolean Oap { get; set; }

        [Column("responsible")]
        public Boolean Responsible { get; set; }

        [Column("at_risk")]
        public Boolean AtRisk { get; set; }

        [Column("full_ed")]
        public Boolean FullEd { get; set; }

        [Column("member_sid")]
        public int MemberSID { get; set; }

        [Column("bank_acc_type")]
        [StringLength(3)]
        public string BankAccType { get; set; }
    }
}
