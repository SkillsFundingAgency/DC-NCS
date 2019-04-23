using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ESFA.DC.NCS.EF
{
    public partial class NcsContext : DbContext
    {
        public NcsContext()
        {
        }

        public NcsContext(DbContextOptions<NcsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FundingValue> FundingValues { get; set; }
        public virtual DbSet<NcsSubmission> NcsSubmissions { get; set; }
        public virtual DbSet<OutcomeRate> OutcomeRates { get; set; }
        public virtual DbSet<Source> Sources { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\;Database=NCS;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<FundingValue>(entity =>
            {
                entity.HasKey(e => new { e.Ukprn, e.TouchpointId, e.CustomerId, e.ActionPlanId, e.OutcomeId });

                entity.Property(e => e.Ukprn).HasColumnName("UKPRN");

                entity.Property(e => e.TouchpointId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.OutcomeEffectiveDate).HasColumnType("date");

                entity.Property(e => e.Period)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<NcsSubmission>(entity =>
            {
                entity.HasKey(e => new { e.Ukprn, e.TouchpointId, e.CustomerId, e.ActionPlanId, e.OutcomeId });

                entity.ToTable("NcsSubmission");

                entity.Property(e => e.Ukprn).HasColumnName("UKPRN");

                entity.Property(e => e.TouchpointId)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.AdviserName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DssTimestamp).HasColumnType("datetime");

                entity.Property(e => e.HomePostCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OutcomeEffectiveDate).HasColumnType("date");

                entity.Property(e => e.SessionDate).HasColumnType("date");

                entity.Property(e => e.SubContractorId)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<OutcomeRate>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Delivery)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EffectiveFrom).HasColumnType("date");

                entity.Property(e => e.EffectiveTo).HasColumnType("date");
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.ToTable("Source");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.SubmissionDate).HasColumnType("datetime");

                entity.Property(e => e.TouchpointId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Ukprn).HasColumnName("UKPRN");
            });
        }
    }
}
