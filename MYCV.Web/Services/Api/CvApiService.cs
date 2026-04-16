using MYCV.Application.DTOs;
using System.Net.Http.Headers;
using MYCV.Web.Helpers;


namespace MYCV.Web.Services.Api
{
    public class CvApiService : ICvApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CvApiService> _logger;

        public CvApiService(HttpClient httpClient, ILogger<CvApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Get User personal detail by userId 
        /// </summary>
        /// <summary>
        /// Get User personal detail by userId
        /// </summary>
        public async Task<ApiResponse<UserPersonalDetailDto>> GetUserPersonalDetailAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching personal detail for user {UserId}", userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/personalDetail");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserPersonalDetailAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, error);

                    return ApiResponse<UserPersonalDetailDto>.ErrorResponse(error);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserPersonalDetailDto>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserPersonalDetailAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<UserPersonalDetailDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched personal detail for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserPersonalDetailAsync for user {UserId}",
                    userId);

                return ApiResponse<UserPersonalDetailDto>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save personal detail for a user
        /// </summary>
        /// <summary>
        /// Save personal detail for a user
        /// </summary>
        public async Task<ApiResponse<UserPersonalDetailDto>> SaveUserPersonalDetailAsync(UserPersonalDetailDto dto)
        {
            if (dto == null)
                return ApiResponse<UserPersonalDetailDto>.ErrorResponse("User personal detail is null");

            try
            {
                _logger.LogInformation("Saving personal detail for user {UserId}", dto.UserId);

                using var content = new MultipartFormDataContent();

                void AddString(string name, string? value)
                {
                    content.Add(new StringContent(value ?? string.Empty), name);
                }

                // Required Fields
                AddString("UserId", dto.UserId.ToString());
                AddString("Id", dto.Id.ToString());
                AddString("FullName", dto.FullName);
                AddString("ProfessionalTitle", dto.ProfessionalTitle);
                AddString("DateOfBirth", dto.DateOfBirth.ToString("yyyy-MM-dd"));
                AddString("Gender", dto.Gender?.ToString());
                AddString("PhoneNumber", dto.PhoneNumber);
                AddString("Country", dto.Country);
                AddString("City", dto.City);

                // Optional Fields
                AddString("Religion", dto.Religion?.ToString());
                AddString("PresentAddress", dto.PresentAddress);
                AddString("PermanentAddress", dto.PermanentAddress);
                AddString("Nationality", dto.Nationality);
                AddString("LinkedIn", dto.LinkedIn);
                AddString("GitHub", dto.GitHub);
                AddString("Portfolio", dto.Portfolio);
                AddString("Website", dto.Website);
                AddString("LinkedInHeadline", dto.LinkedInHeadline);

                // Profile Picture
                if (dto.ProfilePicture != null)
                {
                    var stream = dto.ProfilePicture.OpenReadStream();

                    var fileContent = new StreamContent(stream);

                    fileContent.Headers.ContentType =
                        new MediaTypeHeaderValue(dto.ProfilePicture.ContentType ?? "application/octet-stream");

                    content.Add(fileContent, "ProfilePicture", dto.ProfilePicture.FileName ?? "profilepic");
                }

                var response = await _httpClient.PostAsync("api/cv/personal-detail", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserPersonalDetailAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        dto.UserId, response.StatusCode, errorContent);

                    return ApiResponse<UserPersonalDetailDto>.ErrorResponse(errorContent);
                }

                var apiResult = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserPersonalDetailDto>>(AppJsonOptions.Options);

                if (apiResult == null)
                {
                    _logger.LogWarning(
                        "SaveUserPersonalDetailAsync returned null response for user {UserId}",
                        dto.UserId);

                    return ApiResponse<UserPersonalDetailDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Personal detail saved successfully for user {UserId}",
                    dto.UserId);

                return apiResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserPersonalDetailAsync for user {UserId}",
                    dto.UserId);

                return ApiResponse<UserPersonalDetailDto>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user education records by userId
        /// </summary>
        public async Task<ApiResponse<List<UserEducationDto>>> GetUserEducationAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching education records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/education");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserEducationAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserEducationDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserEducationDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserEducationAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserEducationDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched education records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserEducationAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserEducationDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user education records
        /// </summary>
        public async Task<ApiResponse<List<UserEducationDto>>> SaveUserEducationAsync(List<UserEducationDto> educationList)
        {
            if (educationList == null || !educationList.Any())
                return ApiResponse<List<UserEducationDto>>.ErrorResponse("Education list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving education records. Count: {Count}",
                    educationList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/education",
                    educationList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserEducationAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserEducationDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserEducationDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserEducationAsync returned null response");

                    return ApiResponse<List<UserEducationDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Education records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserEducationAsync");

                return ApiResponse<List<UserEducationDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user work experience records by userId
        /// </summary>
        public async Task<ApiResponse<List<UserExperienceDto>>> GetUserExperiencesAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching work experience records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/experience");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserExperiencesAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserExperienceDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserExperienceDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserExperiencesAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserExperienceDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched work experience records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserExperiencesAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserExperienceDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user work/experience records
        /// </summary>
        public async Task<ApiResponse<List<UserExperienceDto>>> SaveUserExperiencesAsync(List<UserExperienceDto> experienceList)
        {
            if (experienceList == null || !experienceList.Any())
                return ApiResponse<List<UserExperienceDto>>.ErrorResponse("Experience list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving work experience records. Count: {Count}",
                    experienceList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/experience",
                    experienceList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserExperiencesAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserExperienceDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserExperienceDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserExperiencesAsync returned null response");

                    return ApiResponse<List<UserExperienceDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Work experience records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserExperiencesAsync");

                return ApiResponse<List<UserExperienceDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all skill records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's skill list</returns>
        public async Task<ApiResponse<List<UserSkillDto>>> GetUserSkillAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching skill records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/skill");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserSkillAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserSkillDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserSkillDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserSkillAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserSkillDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched skill records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserSkillAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserSkillDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user skill records
        /// </summary>
        /// <param name="skillList">List of user skills to save</param>
        /// <returns>ApiResponse with saved user skills</returns>
        public async Task<ApiResponse<List<UserSkillDto>>> SaveUserSkillAsync(List<UserSkillDto> skillList)
        {
            if (skillList == null || !skillList.Any())
                return ApiResponse<List<UserSkillDto>>.ErrorResponse("Skill list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving user skill records. Count: {Count}",
                    skillList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/skill",
                    skillList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserSkillAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserSkillDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserSkillDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserSkillAsync returned null response");

                    return ApiResponse<List<UserSkillDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User skill records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserSkillAsync");

                return ApiResponse<List<UserSkillDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all projects records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's project list</returns>
        public async Task<ApiResponse<List<UserProjectDto>>> GetUserProjectAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching project records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/project");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserProjectAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserProjectDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserProjectDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserProjectAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserProjectDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched project records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserProjectAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserProjectDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user project records
        /// </summary>
        /// <param name="projectList">List of user project to save</param>
        /// <returns>ApiResponse with saved user projects</returns>
        public async Task<ApiResponse<List<UserProjectDto>>> SaveUserProjectAsync(List<UserProjectDto> projectList)
        {
            if (projectList == null || !projectList.Any())
                return ApiResponse<List<UserProjectDto>>.ErrorResponse("Project list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving user project records. Count: {Count}",
                    projectList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/project",
                    projectList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserProjectAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserProjectDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserProjectDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserProjectAsync returned null response");

                    return ApiResponse<List<UserProjectDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User project records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserProjectAsync");

                return ApiResponse<List<UserProjectDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all language records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's language list</returns>
        public async Task<ApiResponse<List<UserLanguageDto>>> GetUserLanguageAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching language records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/language");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserLanguageAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserLanguageDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserLanguageDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserLanguageAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserLanguageDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched language records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserLanguageAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserLanguageDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user language records
        /// </summary>
        /// <param name="languageList">List of user language to save</param>
        /// <returns>ApiResponse with saved user languages</returns>
        public async Task<ApiResponse<List<UserLanguageDto>>> SaveUserLanguageAsync(List<UserLanguageDto> languageList)
        {
            if (languageList == null || !languageList.Any())
                return ApiResponse<List<UserLanguageDto>>.ErrorResponse("Languages list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving user language records. Count: {Count}",
                    languageList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/language",
                    languageList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserLanguageAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserLanguageDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserLanguageDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserLanguageAsync returned null response");

                    return ApiResponse<List<UserLanguageDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User language records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserLanguageAsync");

                return ApiResponse<List<UserLanguageDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all Summary & Objective records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's Summary & Objective list</returns>
        public async Task<ApiResponse<List<UserSummaryObjectiveDto>>> GetUserSummaryObjectiveAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching SummaryObjective records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/summaryObjective");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserSummaryObjectiveAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserSummaryObjectiveDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserSummaryObjectiveAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched summary & objective records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserSummaryObjectiveAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save multiple user summary & objective records
        /// </summary>
        /// <param name="summaryObjectiveList">List of user summary & objective to save</param>
        /// <returns>ApiResponse with saved user summary & objective</returns>
        public async Task<ApiResponse<List<UserSummaryObjectiveDto>>> SaveUserSummaryObjectiveAsync(List<UserSummaryObjectiveDto> summaryObjectiveList)
        {
            if (summaryObjectiveList == null || !summaryObjectiveList.Any())
                return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse("SummaryObjective list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving user summary & objective records. Count: {Count}",
                    summaryObjectiveList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/summaryObjective",
                    summaryObjectiveList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserSummaryObjectiveAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserSummaryObjectiveDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserSummaryObjectiveAsync returned null response");

                    return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User summary & object records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserSummaryObjectiveAsync");

                return ApiResponse<List<UserSummaryObjectiveDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all references records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's reference list</returns>
        public async Task<ApiResponse<List<UserReferenceDto>>> GetUserReferencesAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching references records for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/reference");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserReferencesAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<List<UserReferenceDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserReferenceDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserReferenceAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<List<UserReferenceDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched references records for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserReferenceAsync for user {UserId}",
                    userId);

                return ApiResponse<List<UserReferenceDto>>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save user multiple reference records
        /// </summary>
        /// <param name="referenceList">List of user reference to save</param>
        /// <returns>ApiResponse with saved user reference</returns>
        public async Task<ApiResponse<List<UserReferenceDto>>> SaveUserReferencesAsync(List<UserReferenceDto> referenceList)
        {
            if (referenceList == null || !referenceList.Any())
                return ApiResponse<List<UserReferenceDto>>.ErrorResponse("Reference list is empty");

            try
            {
                _logger.LogInformation(
                    "Saving user reference records. Count: {Count}",
                    referenceList.Count);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/reference",
                    referenceList,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserReferencesAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<UserReferenceDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<UserReferenceDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserReferencesAsync returned null response");

                    return ApiResponse<List<UserReferenceDto>>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User reference records saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserReferencesAsync");

                return ApiResponse<List<UserReferenceDto>>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user subscription record
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user subscription</returns>
        public async Task<ApiResponse<UserSubscriptionDto>> GetUserSubscriptionAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching subscription record for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/subscription");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserSubscriptionAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<UserSubscriptionDto>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserSubscriptionDto>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserSubscriptionAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<UserSubscriptionDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched subscription record for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserSubscriptionAsync for user {UserId}",
                    userId);

                return ApiResponse<UserSubscriptionDto>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save user subscription record
        /// </summary>
        /// <param name="subscription">User subscription to save</param>
        /// <returns>ApiResponse with saved subscription</returns>
        public async Task<ApiResponse<UserSubscriptionDto>> SaveUserSubscriptionAsync(UserSubscriptionDto subscription)
        {
            if (subscription == null)
                return ApiResponse<UserSubscriptionDto>.ErrorResponse("Subscription data is empty");

            try
            {
                _logger.LogInformation(
                    "Saving subscription for user {UserId}",
                    subscription.UserId);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/subscription",
                    subscription,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserSubscriptionAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<UserSubscriptionDto>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserSubscriptionDto>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserSubscriptionAsync returned null response");

                    return ApiResponse<UserSubscriptionDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User subscription saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserSubscriptionAsync");

                return ApiResponse<UserSubscriptionDto>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get user selected template record
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user selected template</returns>
        public async Task<ApiResponse<UserSelectedTemplateDto>> GetUserSelectedTemplateAsync(int userId)
        {
            try
            {
                _logger.LogInformation(
                    "Fetching selected template record for user {UserId}",
                    userId);

                var response = await _httpClient.GetAsync($"api/cv/{userId}/selected-template");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetUserSelectedTemplateAsync failed for user {UserId}. StatusCode: {StatusCode}, Error: {Error}",
                        userId, response.StatusCode, errorContent);

                    return ApiResponse<UserSelectedTemplateDto>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserSelectedTemplateDto>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "GetUserSelectedTemplateAsync returned null response for user {UserId}",
                        userId);

                    return ApiResponse<UserSelectedTemplateDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "Successfully fetched selected template record for user {UserId}",
                    userId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in GetUserSelectedTemplateAsync for user {UserId}",
                    userId);

                return ApiResponse<UserSelectedTemplateDto>.ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Get all CV templates
        /// </summary>
        /// <returns>ApiResponse with template list</returns>
        public async Task<ApiResponse<List<CvTemplateDto>>> GetCvTemplatesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching CV templates");

                var response = await _httpClient.GetAsync("api/cv/templates");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "GetCvTemplatesAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<List<CvTemplateDto>>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<List<CvTemplateDto>>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning("GetCvTemplatesAsync returned null response");

                    return ApiResponse<List<CvTemplateDto>>
                        .ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation("Successfully fetched CV templates");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetCvTemplatesAsync");

                return ApiResponse<List<CvTemplateDto>>
                    .ErrorResponse("Network or API error");
            }
        }

        /// <summary>
        /// Save user selected template record
        /// </summary>
        /// <param name="template">User selected template to save</param>
        /// <returns>ApiResponse with saved template</returns>
        public async Task<ApiResponse<UserSelectedTemplateDto>> SaveUserSelectedTemplateAsync(UserSelectedTemplateDto template)
        {
            if (template == null)
                return ApiResponse<UserSelectedTemplateDto>.ErrorResponse("Template selection data is empty");

            try
            {
                _logger.LogInformation(
                    "Saving selected template for user {UserId}",
                    template.UserId);

                var response = await _httpClient.PostAsJsonAsync(
                    "api/cv/selected-template",
                    template,
                    AppJsonOptions.Options);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();

                    _logger.LogWarning(
                        "SaveUserSelectedTemplateAsync failed. StatusCode: {StatusCode}, Error: {Error}",
                        response.StatusCode, errorContent);

                    return ApiResponse<UserSelectedTemplateDto>.ErrorResponse(errorContent);
                }

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse<UserSelectedTemplateDto>>(AppJsonOptions.Options);

                if (result == null)
                {
                    _logger.LogWarning(
                        "SaveUserSelectedTemplateAsync returned null response");

                    return ApiResponse<UserSelectedTemplateDto>.ErrorResponse("Invalid response from API");
                }

                _logger.LogInformation(
                    "User selected template saved successfully");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Exception occurred in SaveUserSelectedTemplateAsync");

                return ApiResponse<UserSelectedTemplateDto>.ErrorResponse($"Network or API error: {ex.Message}");
            }
        }
    }
}