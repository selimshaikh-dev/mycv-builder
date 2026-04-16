using MYCV.Application.DTOs;

namespace MYCV.Application.DTOs
{
    /// <summary>
    /// Master DTO for full CV preview
    /// </summary>
    public class CvPreviewDto
    {
        public UserSelectedTemplateDto? Template { get; set; }

        public UserPersonalDetailDto? PersonalDetail { get; set; }

        public List<UserEducationDto> Educations { get; set; } = new();

        public List<UserExperienceDto> Experiences { get; set; } = new();

        public List<UserSkillDto> Skills { get; set; } = new();

        public List<UserProjectDto> Projects { get; set; } = new();

        public List<UserLanguageDto> Languages { get; set; } = new();

        public List<UserSummaryObjectiveDto> Summary { get; set; } = new();

        public List<UserReferenceDto> References { get; set; } = new();
    }
}