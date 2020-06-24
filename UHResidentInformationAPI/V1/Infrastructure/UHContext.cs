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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Person>()
                    .HasKey(person => new
                    {
                        person.HouseRef,
                        person.PersonNo
                    });

            modelBuilder.Entity<TelephoneNumber>()
                .HasOne<Person>()
                .WithMany()
                .HasForeignKey(t => t.ContactID)
                .HasPrincipalKey(p => p.PersonNo);
        }

    }
}
