using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("member")]
    public class Person
    {
        [Column("house_ref")]
        [MaxLength(10)]

        public string HouseRef { get; set; }

        [Column("person_no")]
        [MaxLength(32)]
        public int PersonNo { get; set; }

        [Column("title")]
        [MaxLength(10)]
        public string Title { get; set; }

        [Column("forename")]
        [MaxLength(24)]
        public string FirstName { get; set; }

        [Column("surname")]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Column("ni_no")]
        [MaxLength(12)]
        public string NINumber { get; set; }

        [Column("dob")]
        public DateTime DateOfBirth { get; set; }
    }
}
