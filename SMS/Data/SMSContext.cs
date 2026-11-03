using CodeflexSystems.SMS.Models;
using CodeflexSystems.SMS.Models.Mapping;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace CodeflexSystems.SMS.Data
{
    public partial class SMSContext : DbContext
    {
        static SMSContext()
        {
            Database.SetInitializer<SMSContext>(null);
        }

        public SMSContext()
            : base("Name=SMSContext")
        {
        }

        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<EmailType> EmailTypes { get; set; }
        public DbSet<FeeType> FeeTypes { get; set; }
        public DbSet<GuardianAddress> GuardianAddresses { get; set; }
        public DbSet<GuardianEmail> GuardianEmails { get; set; }
        public DbSet<GuardianPhone> GuardianPhones { get; set; }
        public DbSet<GuardianPhoto> GuardianPhotos { get; set; }
        public DbSet<Guardian> Guardians { get; set; }
        public DbSet<GuardianType> GuardianTypes { get; set; }
        public DbSet<PhoneType> PhoneTypes { get; set; }
        public DbSet<ReceiptDetail> ReceiptDetails { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<SchoolEmail> SchoolEmails { get; set; }
        public DbSet<SchoolPhone> SchoolPhones { get; set; }
        public DbSet<School> Schools { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<StudentAddress> StudentAddresses { get; set; }
        public DbSet<StudentDiscount> StudentDiscounts { get; set; }
        public DbSet<StudentEmail> StudentEmails { get; set; }
        public DbSet<StudentFerryFee> StudentFerryFees { get; set; }
        public DbSet<StudentGuardian> StudentGuardians { get; set; }
        public DbSet<StudentPhone> StudentPhones { get; set; }
        public DbSet<StudentPhoto> StudentPhotos { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentSection> StudentSections { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
        public DbSet<UserOption> UserOptions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AcademicYearMap());
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new AddressTypeMap());
            modelBuilder.Configurations.Add(new ClassMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new CourseMap());
            modelBuilder.Configurations.Add(new CurrencyMap());
            modelBuilder.Configurations.Add(new EmailTypeMap());
            modelBuilder.Configurations.Add(new FeeTypeMap());
            modelBuilder.Configurations.Add(new GuardianAddressMap());
            modelBuilder.Configurations.Add(new GuardianEmailMap());
            modelBuilder.Configurations.Add(new GuardianPhoneMap());
            modelBuilder.Configurations.Add(new GuardianPhotoMap());
            modelBuilder.Configurations.Add(new GuardianMap());
            modelBuilder.Configurations.Add(new GuardianTypeMap());
            modelBuilder.Configurations.Add(new PhoneTypeMap());
            modelBuilder.Configurations.Add(new ReceiptDetailMap());
            modelBuilder.Configurations.Add(new ReceiptMap());
            modelBuilder.Configurations.Add(new ReligionMap());
            modelBuilder.Configurations.Add(new SchoolEmailMap());
            modelBuilder.Configurations.Add(new SchoolPhoneMap());
            modelBuilder.Configurations.Add(new SchoolMap());
            modelBuilder.Configurations.Add(new SectionMap());
            modelBuilder.Configurations.Add(new StudentAddressMap());
            modelBuilder.Configurations.Add(new StudentDiscountMap());
            modelBuilder.Configurations.Add(new StudentEmailMap());
            modelBuilder.Configurations.Add(new StudentFerryFeeMap());
            modelBuilder.Configurations.Add(new StudentGuardianMap());
            modelBuilder.Configurations.Add(new StudentPhoneMap());
            modelBuilder.Configurations.Add(new StudentPhotoMap());
            modelBuilder.Configurations.Add(new StudentMap());
            modelBuilder.Configurations.Add(new StudentSectionMap());
            modelBuilder.Configurations.Add(new UserMembershipMap());
            modelBuilder.Configurations.Add(new UserOptionMap());
            modelBuilder.Configurations.Add(new UserPermissionMap());
            modelBuilder.Configurations.Add(new UserMap());
        }
    }
}
