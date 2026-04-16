using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MYCV.Shared.Extensions;
using MYCV.Application.DTOs;
using MYCV.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MYCV.API.Controllers
{
    [ApiController]
    [Route("api/cv")]
    [Authorize] 
    public class UserCvController : ControllerBase
    {
        private readonly ILogger<UserCvController> _logger;
        private readonly IUserPersonalDetailService _userPersonalDetail;
        private readonly IUserEducationService _userEducationService;
        private readonly IUserExperienceService _userExperienceService;
        private readonly IUserSkillService _userSkillService;
        private readonly IUserProjectService _userProjectService;
        private readonly IUserLanguageService _userLanguageService;
        private readonly IUserSummaryObjectiveService _userSummaryObjectiveService;
        private readonly IUserReferenceService _userReferenceService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ICvTemplateService _cvTemplateService;
        private readonly IUserSelectedTemplateService _userSelectedTemplateService;
        private readonly ICvPreviewService _cvPreviewService;

        public UserCvController(ILogger<UserCvController> logger, IUserPersonalDetailService userPersonalDetail,
            IUserEducationService userEducationService, IUserExperienceService userExperienceService, IUserSkillService userSkillService, IUserProjectService userProjectService, IUserLanguageService userLanguageService, IUserSummaryObjectiveService userSummaryObjectiveService, IUserReferenceService userReferenceService, IUserSubscriptionService userSubscriptionService, ICvTemplateService cvTemplateService, IUserSelectedTemplateService userSelectedTemplateService, ICvPreviewService cvPreviewService)
        {
            _logger = logger;
            _userPersonalDetail = userPersonalDetail;
            _userEducationService = userEducationService;
            _userExperienceService = userExperienceService;
            _userSkillService = userSkillService;
            _userProjectService = userProjectService;
            _userLanguageService = userLanguageService;
            _userSummaryObjectiveService = userSummaryObjectiveService;
            _userReferenceService = userReferenceService;
            _userSubscriptionService = userSubscriptionService;
            _cvTemplateService = cvTemplateService;
            _userSelectedTemplateService = userSelectedTemplateService;
            _cvPreviewService = cvPreviewService;
        }

        /// <summary>
        /// Get personal detail for a user by userId (int)
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's personal detail</returns>
        [HttpGet("{userId:int}/personalDetail")]
        public async Task<IActionResult> GetUserPersonalDetail(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching personal detail for user {UserId}", userId);

                var personalDetail = await _userPersonalDetail.GetUserPersonalDetailAsync(userId);
                if (personalDetail == null)
                {
                    _logger.LogWarning("No personal detail found for user {UserId}", userId);
                    return NotFound(ApiResponse<UserPersonalDetailDto>.ErrorResponse("Personal detail not found"));
                }

                return Ok(ApiResponse<UserPersonalDetailDto>.SuccessResponse(personalDetail, "Personal detail fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching personal detail for user {UserId}", userId);
                return StatusCode(500, ApiResponse<UserPersonalDetailDto>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user CV personal information
        /// </summary>
        [HttpPost("personal-detail")]
        public async Task<IActionResult> SaveUserPersonalDetail([FromForm] UserPersonalDetailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<UserPersonalDetailDto>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                dto.UserId = User.GetUserId();

                var savedCv = await _userPersonalDetail.SaveUserPersonalDetailAsync(dto);

                return Ok(ApiResponse<UserPersonalDetailDto>
                    .SuccessResponse(savedCv, "Personal information saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<UserPersonalDetailDto>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving personal info for user {UserId}", dto.UserId);
                return StatusCode(500,
                    ApiResponse<UserPersonalDetailDto>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all education records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's education list</returns>
        [HttpGet("{userId:int}/education")]
        public async Task<IActionResult> GetUserEducation(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching education records for user {UserId}", userId);

                var educationList = await _userEducationService.GetUserEducationAsync(userId);

                if (educationList == null || !educationList.Any())
                {
                    _logger.LogWarning("No education records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserEducationDto>>.ErrorResponse("No education records found"));
                }

                return Ok(ApiResponse<List<UserEducationDto>>.SuccessResponse(educationList, "Education records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching education records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserEducationDto>>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user education information
        /// </summary>
        [HttpPost("education")]
        public async Task<IActionResult> SaveUserEducation([FromBody] List<UserEducationDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserEducationDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userEducationService
                    .SaveUserEducationsAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserEducationDto>>
                    .SuccessResponse(savedList, "Education information saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserEducationDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving education info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserEducationDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all work experience records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's experience list</returns>
        [HttpGet("{userId:int}/experience")]
        public async Task<IActionResult> GetUserExperience(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching work experience records for user {UserId}", userId);

                var experienceList = await _userExperienceService.GetUserExperiencesAsync(userId);

                if (experienceList == null || !experienceList.Any())
                {
                    _logger.LogWarning("No work experience records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserExperienceDto>>
                        .ErrorResponse("No work experience records found"));
                }

                return Ok(ApiResponse<List<UserExperienceDto>>
                    .SuccessResponse(experienceList, "Work experience records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching work experience records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserExperienceDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user experience information
        /// </summary>
        [HttpPost("experience")]
        public async Task<IActionResult> SaveUserExperience([FromBody] List<UserExperienceDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserExperienceDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId(); 

                var savedList = await _userExperienceService
                    .SaveUserExperiencesAsync(dtoList, userId); 

                return Ok(ApiResponse<List<UserExperienceDto>>
                    .SuccessResponse(savedList, "Work experience saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserExperienceDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving experience info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserExperienceDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all skills records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's skill list</returns>
        [HttpGet("{userId:int}/skill")]
        public async Task<IActionResult> GetUserSkill(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user skill records for user {UserId}", userId);

                var skillList = await _userSkillService.GetUserSkillAsync(userId);

                if (skillList == null || !skillList.Any())
                {
                    _logger.LogWarning("No skill records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserSkillDto>>
                        .ErrorResponse("No skill records found"));
                }

                return Ok(ApiResponse<List<UserSkillDto>>
                    .SuccessResponse(skillList, "Skill records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching skill records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserSkillDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user skill information
        /// </summary>
        [HttpPost("skill")]
        public async Task<IActionResult> SaveUserSkill([FromBody] List<UserSkillDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserSkillDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userSkillService
                    .SaveUserSkillAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserSkillDto>>
                    .SuccessResponse(savedList, "Skill saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserSkillDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving skill info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserSkillDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all projects records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's project list</returns>
        [HttpGet("{userId:int}/project")]
        public async Task<IActionResult> GetUserProject(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user project records for user {UserId}", userId);

                var projectList = await _userProjectService.GetUserProjectAsync(userId);

                if (projectList == null || !projectList.Any())
                {
                    _logger.LogWarning("No project records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserProjectDto>>
                        .ErrorResponse("No project records found"));
                }

                return Ok(ApiResponse<List<UserProjectDto>>
                    .SuccessResponse(projectList, "Project records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching project records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserProjectDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user skill information
        /// </summary>
        [HttpPost("project")]
        public async Task<IActionResult> SaveUserProject([FromBody] List<UserProjectDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserProjectDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userProjectService
                    .SaveUserProjectAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserProjectDto>>
                    .SuccessResponse(savedList, "Project saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserProjectDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving project info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserProjectDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all languages records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's language list</returns>
        [HttpGet("{userId:int}/language")]
        public async Task<IActionResult> GetUserLanguage(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user language records for user {UserId}", userId);

                var languageList = await _userLanguageService.GetUserLanguageAsync(userId);

                if (languageList == null || !languageList.Any())
                {
                    _logger.LogWarning("No language records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserLanguageDto>>
                        .ErrorResponse("No language records found"));
                }

                return Ok(ApiResponse<List<UserLanguageDto>>
                    .SuccessResponse(languageList, "Language records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching language records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserLanguageDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user language information
        /// </summary>
        [HttpPost("language")]
        public async Task<IActionResult> SaveUserLanguage([FromBody] List<UserLanguageDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserLanguageDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userLanguageService
                    .SaveUserLanguageAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserLanguageDto>>
                    .SuccessResponse(savedList, "Languages saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserLanguageDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving language info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserLanguageDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all summaryObjective records for a user
        /// </summary>
        [HttpGet("{userId:int}/summaryObjective")]
        public async Task<IActionResult> GetUserSummaryObjective(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user summary & objective records for user {UserId}", userId);

                var summaryObjectiveList = await _userSummaryObjectiveService.GetUserSummaryObjectiveAsync(userId);

                if (summaryObjectiveList == null || !summaryObjectiveList.Any())
                {
                    _logger.LogWarning("No summaryObjective records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserSummaryObjectiveDto>>
                        .ErrorResponse("No summary & objective records found"));
                }

                return Ok(ApiResponse<List<UserSummaryObjectiveDto>>
                    .SuccessResponse(summaryObjectiveList, "SummaryObjective records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching summary & objective records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserSummaryObjectiveDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user Summary & Objective information
        /// </summary>
        [HttpPost("summaryObjective")]
        public async Task<IActionResult> SaveUserSummaryObjective([FromBody] List<UserSummaryObjectiveDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserSummaryObjectiveDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userSummaryObjectiveService
                    .SaveUserSummaryObjectiveAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserSummaryObjectiveDto>>
                    .SuccessResponse(savedList, "Summary & Objective saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserSummaryObjectiveDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving summary & objective info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserSummaryObjectiveDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all references records for a user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ApiResponse with user's reference list</returns>
        [HttpGet("{userId:int}/reference")]
        public async Task<IActionResult> GetUserReference(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching user reference records for user {UserId}", userId);

                var referenceList = await _userReferenceService.GetUserReferenceAsync(userId);

                if (referenceList == null || !referenceList.Any())
                {
                    _logger.LogWarning("No reference records found for user {UserId}", userId);
                    return NotFound(ApiResponse<List<UserReferenceDto>>
                        .ErrorResponse("No reference records found"));
                }

                return Ok(ApiResponse<List<UserReferenceDto>>
                    .SuccessResponse(referenceList, "Reference records fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reference records for user {UserId}", userId);
                return StatusCode(500, ApiResponse<List<UserReferenceDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user reference information
        /// </summary>
        [HttpPost("reference")]
        public async Task<IActionResult> SaveUserReference([FromBody] List<UserReferenceDto> dtoList)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<List<UserReferenceDto>>
                    .ErrorResponse("Please fill all required fields."));

            try
            {
                int userId = User.GetUserId();

                var savedList = await _userReferenceService
                    .SaveUserReferenceAsync(dtoList, userId);

                return Ok(ApiResponse<List<UserReferenceDto>>
                    .SuccessResponse(savedList, "References saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<List<UserReferenceDto>>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving reference info for user {UserId}", User.Identity?.Name);
                return StatusCode(500,
                    ApiResponse<List<UserReferenceDto>>
                        .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get subscription information for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>ApiResponse with user's subscription details</returns>
        [HttpGet("{userId:int}/subscription")]
        public async Task<IActionResult> GetUserSubscription(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching subscription for user {UserId}", userId);

                var subscription = await _userSubscriptionService.GetUserSubscriptionAsync(userId);

                if (subscription == null)
                {
                    _logger.LogWarning("No active subscription found for user {UserId}", userId);
                    return NotFound(ApiResponse<UserSubscriptionDto>
                        .ErrorResponse("No subscription found for this user"));
                }

                return Ok(ApiResponse<UserSubscriptionDto>
                    .SuccessResponse(subscription, "Subscription fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription for user {UserId}", userId);
                return StatusCode(500, ApiResponse<UserSubscriptionDto>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user subscription information
        /// </summary>
        [HttpPost("subscription")]
        public async Task<IActionResult> SaveUserSubscription([FromBody] UserSubscriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserSubscriptionDto>
                    .ErrorResponse("Please fill all required fields."));
            }

            try
            {
                int userId = User.GetUserId();
                dto.UserId = userId;

                var savedSubscription = await _userSubscriptionService.SaveUserSubscriptionAsync(dto);

                return Ok(ApiResponse<UserSubscriptionDto>
                    .SuccessResponse(savedSubscription, "Subscription saved successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<UserSubscriptionDto>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving subscription for user {UserId}", User.Identity?.Name);
                return StatusCode(500, ApiResponse<UserSubscriptionDto>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get all CV templates
        /// </summary>
        /// <returns>List of available CV templates</returns>
        [HttpGet("templates")]
        public async Task<IActionResult> GetCvTemplates()
        {
            try
            {
                _logger.LogInformation("Fetching all CV templates");

                var templates = await _cvTemplateService.GetAllAsync();

                if (templates == null || !templates.Any())
                {
                    _logger.LogWarning("No CV templates found in database");

                    return NotFound(ApiResponse<List<CvTemplateDto>>
                        .ErrorResponse("No templates found"));
                }

                var result = templates.Select(t => new CvTemplateDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    ImageUrl = t.ImageUrl,
                    Description = t.Description,
                    IsPremium = t.IsPremium
                }).ToList();

                _logger.LogInformation("Successfully fetched {Count} CV templates", result.Count);

                return Ok(ApiResponse<List<CvTemplateDto>>
                    .SuccessResponse(result, "Templates fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching CV templates");

                return StatusCode(500, ApiResponse<List<CvTemplateDto>>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Save user selected CV template
        /// </summary>
        [HttpPost("selected-template")]
        public async Task<IActionResult> SaveSelectedTemplate([FromBody] UserSelectedTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserSelectedTemplateDto>
                    .ErrorResponse("Please select a template."));
            }

            try
            {
                int userId = User.GetUserId();
                dto.UserId = userId;

                var savedTemplate = await _userSelectedTemplateService
                    .SaveUserSelectedTemplateAsync(dto);

                return Ok(ApiResponse<UserSelectedTemplateDto>
                    .SuccessResponse(savedTemplate, "Template selected successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<UserSelectedTemplateDto>
                    .ErrorResponse("User not authorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving selected template for user {UserId}", User.Identity?.Name);

                return StatusCode(500, ApiResponse<UserSelectedTemplateDto>
                    .ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get full CV preview data for a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Full CV preview data</returns>
        [HttpGet("preview/{userId}")]
        public async Task<IActionResult> GetCvPreview(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(ApiResponse<CvPreviewDto>
                    .ErrorResponse("Invalid user id"));
            }

            try
            {
                _logger.LogInformation(
                    "Fetching CV preview for user {UserId}",
                    userId);

                var previewData = await _cvPreviewService
                    .GetCvPreviewAsync(userId);

                if (previewData == null)
                {
                    _logger.LogWarning(
                        "No CV preview data found for user {UserId}",
                        userId);

                    return NotFound(ApiResponse<CvPreviewDto>
                        .ErrorResponse("CV preview data not found"));
                }

                _logger.LogInformation(
                    "Successfully fetched CV preview for user {UserId}",
                    userId);

                return Ok(ApiResponse<CvPreviewDto>
                    .SuccessResponse(previewData, "CV preview loaded successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error fetching CV preview for user {UserId}",
                    userId);

                return StatusCode(500, ApiResponse<CvPreviewDto>
                    .ErrorResponse("Internal server error"));
            }
        }
    }
}