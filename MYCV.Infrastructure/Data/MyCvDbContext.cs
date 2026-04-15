using Microsoft.EntityFrameworkCore;
using MYCV.Domain.Entities;

namespace MYCV.Infrastructure.Data
{
    public class MyCvDbContext : DbContext
    {
        public MyCvDbContext(DbContextOptions<MyCvDbContext> options) : base(options) { }

        // =========================
        // DB SETS
        // =========================
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserPersonalDetail> UserPersonalDetails { get; set; } = null!;
        public DbSet<UserEducation> UserEducations { get; set; } = null!;
        public DbSet<UserExperience> UserExperiences { get; set; } = null!;
        public DbSet<UserSkill> UserSkills { get; set; } = null!;
        public DbSet<UserProject> UserProjects { get; set; } = null!;
        public DbSet<UserLanguage> UserLanguages { get; set; } = null!;
        public DbSet<UserSummaryObjective> UserSummaryObjectives { get; set; } = null!;
        public DbSet<UserReference> UserReferences { get; set; } = null!;
        public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
        public DbSet<UserSelectedTemplate> UserSelectedTemplates { get; set; } = null!;
        public DbSet<CvTemplate> CvTemplates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // USER - PERSONAL DETAIL (1:1)
            // =========================
            modelBuilder.Entity<UserPersonalDetail>()
                .HasOne(x => x.User)
                .WithOne(x => x.UserPersonalDetail)
                .HasForeignKey<UserPersonalDetail>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // USER - EDUCATION (1:M)
            // =========================
            modelBuilder.Entity<UserEducation>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserEducations)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - EXPERIENCE (1:M)
            // =========================
            modelBuilder.Entity<UserExperience>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserExperiences)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - SKILL (1:M)
            // =========================
            modelBuilder.Entity<UserSkill>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSkills)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - PROJECT (1:M)
            // =========================
            modelBuilder.Entity<UserProject>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserProjects)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - LANGUAGE (1:M)
            // =========================
            modelBuilder.Entity<UserLanguage>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserLanguages)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - SUMMARY OBJECTIVE (1:M)
            // =========================
            modelBuilder.Entity<UserSummaryObjective>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSummaryObjectives)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - REFERENCE (1:M)
            // =========================
            modelBuilder.Entity<UserReference>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserReferences)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - SUBSCRIPTION (1:M)
            // =========================
            modelBuilder.Entity<UserSubscription>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSubscriptions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // USER - SELECTED TEMPLATE (1:M)
            // =========================
            modelBuilder.Entity<UserSelectedTemplate>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserSelectedTemplates)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.ClientCascade);

            // =========================
            // INDEXES
            // =========================

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<UserSkill>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserExperience>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserProject>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserLanguage>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserSummaryObjective>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserReference>()
                .HasIndex(x => new { x.UserId, x.Priority });

            modelBuilder.Entity<UserSubscription>()
                .HasIndex(x => new { x.UserId, x.StartDate });

            modelBuilder.Entity<UserSelectedTemplate>()
                .HasIndex(x => new { x.UserId, x.CreatedDate });

            modelBuilder.Entity<CvTemplate>()
                .HasIndex(x => new { x.IsPremium });
        }
    }
}