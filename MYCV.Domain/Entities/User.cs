using MYCV.Domain.Enums;

namespace MYCV.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public bool IsEmailVerified { get; set; } = false;
        public string? VerificationCode { get; set; }

        public UserRole Role { get; set; } = UserRole.Client;

        // =========================
        // NAVIGATION PROPERTIES
        // =========================

        public virtual UserPersonalDetail? UserPersonalDetail { get; set; }

        public virtual ICollection<UserEducation> UserEducations { get; set; }
            = new List<UserEducation>();

        public virtual ICollection<UserExperience> UserExperiences { get; set; }
            = new List<UserExperience>();

        public virtual ICollection<UserSkill> UserSkills { get; set; }
            = new List<UserSkill>();

        public virtual ICollection<UserProject> UserProjects { get; set; }
            = new List<UserProject>();

        public virtual ICollection<UserLanguage> UserLanguages { get; set; }
            = new List<UserLanguage>();

        public virtual ICollection<UserSummaryObjective> UserSummaryObjectives { get; set; }
            = new List<UserSummaryObjective>();

        public virtual ICollection<UserReference> UserReferences { get; set; }
            = new List<UserReference>();

        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }
            = new List<UserSubscription>();

        public virtual ICollection<UserSelectedTemplate> UserSelectedTemplates { get; set; }
            = new List<UserSelectedTemplate>();
    }
}