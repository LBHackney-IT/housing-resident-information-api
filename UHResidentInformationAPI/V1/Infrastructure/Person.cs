using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UHResidentInformationAPI.V1.Infrastructure
{
    [Table("member")]
    public class Person
    {
        [Column("house_ref")]
        [MaxLength(16)]
        [Key]
        public int houseRef { get; set; }

        [Column("person_no")]
        [MaxLength(8)]
        public string personNo { get; set; }

        [Column("title")]
        [MaxLength(8)]
        public string Title { get; set; }

        [Column("forename")]
        [MaxLength(30)]
        public string FirstName { get; set; }

        [Column("surname")]
        [MaxLength(30)]
        public string LastName { get; set; }

        [Column("ni_no")]
        [MaxLength(10)]
        public int NINumber { get; set; }

        [Column("dob")]
        public DateTime DateOfBirth { get; set; }
    }
}
