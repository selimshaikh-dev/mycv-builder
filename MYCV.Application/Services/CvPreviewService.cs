using MYCV.Application.DTOs;
using MYCV.Application.Interfaces;

namespace MYCV.Application.Services
{
    /// <summary>
    /// Service for generating full CV preview data
    /// </summary>
    public class CvPreviewService : ICvPreviewService
    {
        private readonly IUserSelectedTemplateService _userSelectedTemplateService;
        private readonly IUserPersonalDetailService _userPersonalDetailService;
        private readonly IUserEducationService _userEducationService;
        private readonly IUserExperienceService _userExperienceService;
        private readonly IUserSkillService _userSkillService;
        private readonly IUserProjectService _userProjectService;
        private readonly IUserLanguageService _userLanguageService;
        private readonly IUserSummaryObjectiveService _userSummaryObjectiveService;
        private readonly IUserReferenceService _userReferenceService;

        public CvPreviewService(
            IUserSelectedTemplateService userSelectedTemplateService,
            IUserPersonalDetailService userPersonalDetailService,
            IUserEducationService userEducationService,
            IUserExperienceService userExperienceService,
            IUserSkillService userSkillService,
            IUserProjectService userProjectService,
            IUserLanguageService userLanguageService,
            IUserSummaryObjectiveService userSummaryObjectiveService,
            IUserReferenceService userReferenceService)
        {
            _userSelectedTemplateService = userSelectedTemplateService;
            _userPersonalDetailService = userPersonalDetailService;
            _userEducationService = userEducationService;
            _userExperienceService = userExperienceService;
            _userSkillService = userSkillService;
            _userProjectService = userProjectService;
            _userLanguageService = userLanguageService;
            _userSummaryObjectiveService = userSummaryObjectiveService;
            _userReferenceService = userReferenceService;
        }

        /// <summary>
        /// Get full CV preview data for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>CvPreviewDto with all CV sections</returns>
        public async Task<CvPreviewDto> GetCvPreviewAsync(int userId)
        {
            var template = await _userSelectedTemplateService
                .GetUserSelectedTemplateAsync(userId);

            var personalDetail = await _userPersonalDetailService
                .GetUserPersonalDetailAsync(userId);

            var educations = await _userEducationService
                .GetUserEducationAsync(userId);

            var experiences = await _userExperienceService
                .GetUserExperiencesAsync(userId);

            var skills = await _userSkillService
                .GetUserSkillAsync(userId);

            var projects = await _userProjectService
                .GetUserProjectAsync(userId);

            var languages = await _userLanguageService
                .GetUserLanguageAsync(userId);

            var summary = await _userSummaryObjectiveService
                .GetUserSummaryObjectiveAsync(userId);

            var references = await _userReferenceService
                .GetUserReferenceAsync(userId);

            return new CvPreviewDto
            {
                Template = template,
                PersonalDetail = personalDetail,
                Educations = educations ?? new List<UserEducationDto>(),
                Experiences = experiences ?? new List<UserExperienceDto>(),
                Skills = skills ?? new List<UserSkillDto>(),
                Projects = projects ?? new List<UserProjectDto>(),
                Languages = languages ?? new List<UserLanguageDto>(),
                Summary = summary ?? new List<UserSummaryObjectiveDto>(),
                References = references ?? new List<UserReferenceDto>()
            };
        }
    }
}