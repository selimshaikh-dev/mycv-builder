using MYCV.Application.DTOs;

namespace MYCV.Web.Services.Api
{
    public interface ICvApiService
    {
        Task<ApiResponse<UserPersonalDetailDto>> GetUserPersonalDetailAsync(int userId);
        Task<ApiResponse<UserPersonalDetailDto>> SaveUserPersonalDetailAsync(UserPersonalDetailDto dto);

        Task<ApiResponse<List<UserEducationDto>>> GetUserEducationAsync(int userId);
        Task<ApiResponse<List<UserEducationDto>>> SaveUserEducationAsync(List<UserEducationDto> educationList);

        Task<ApiResponse<List<UserExperienceDto>>> GetUserExperiencesAsync(int userId);
        Task<ApiResponse<List<UserExperienceDto>>> SaveUserExperiencesAsync(List<UserExperienceDto> experienceList);

        Task<ApiResponse<List<UserSkillDto>>> GetUserSkillAsync(int userId);
        Task<ApiResponse<List<UserSkillDto>>> SaveUserSkillAsync(List<UserSkillDto> skillList);

        Task<ApiResponse<List<UserProjectDto>>> GetUserProjectAsync(int userId);
        Task<ApiResponse<List<UserProjectDto>>> SaveUserProjectAsync(List<UserProjectDto> projectList);

        Task<ApiResponse<List<UserLanguageDto>>> GetUserLanguageAsync(int userId);
        Task<ApiResponse<List<UserLanguageDto>>> SaveUserLanguageAsync(List<UserLanguageDto> languageList);

        Task<ApiResponse<List<UserSummaryObjectiveDto>>> GetUserSummaryObjectiveAsync(int userId);
        Task<ApiResponse<List<UserSummaryObjectiveDto>>> SaveUserSummaryObjectiveAsync(List<UserSummaryObjectiveDto> summaryObjectiveList);

        Task<ApiResponse<List<UserReferenceDto>>> GetUserReferencesAsync(int userId);
        Task<ApiResponse<List<UserReferenceDto>>> SaveUserReferencesAsync(List<UserReferenceDto> referenceList);

        Task<ApiResponse<UserSubscriptionDto>> GetUserSubscriptionAsync(int userId);
        Task<ApiResponse<UserSubscriptionDto>> SaveUserSubscriptionAsync(UserSubscriptionDto subscription);

        Task<ApiResponse<UserSelectedTemplateDto>> GetUserSelectedTemplateAsync(int userId);
        Task<ApiResponse<UserSelectedTemplateDto>> SaveUserSelectedTemplateAsync(UserSelectedTemplateDto template);

        Task<ApiResponse<List<CvTemplateDto>>> GetCvTemplatesAsync();
    }
}