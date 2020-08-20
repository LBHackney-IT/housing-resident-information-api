using Microsoft.EntityFrameworkCore;

namespace UHResidentInformationAPI.V1.Infrastructure
{

    public class UHContext : DbContext
    {
        public UHContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<TelephoneNumber> TelephoneNumbers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<EmailAddresses> EmailAddresses { get; set; }
        public DbSet<ContactLink> ContactLinks { get; set; }
        public DbSet<TenancyAgreement> TenancyAgreements { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // composite primary key for Member table
            modelBuilder.Entity<Person>()
                .HasKey(person => new
                {
                    person.HouseRef,
                    person.PersonNo
                });
        }
    }
}
