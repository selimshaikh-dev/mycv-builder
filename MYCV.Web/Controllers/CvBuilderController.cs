using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MYCV.Web.Services.Api;
using MYCV.Application.DTOs;
using MYCV.Web.Helpers;
using MYCV.Domain.Enums;
using MYCV.Shared.Extensions;

namespace MYCV.Web.Controllers
{
    [Authorize] 
    public class CvBuilderController : Controller
    {
        private readonly ICvApiService _cvApiService;
        private readonly ILogger<CvBuilderController> _logger;

        public CvBuilderController(ICvApiService cvApiService, ILogger<CvBuilderController> logger)
        {
            _cvApiService = cvApiService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                int userId = User.GetUserId();

                // ===============================
                // STEP 1: Personal Information
                // ===============================
                var personalInfoResponse = await _cvApiService.GetUserPersonalDetailAsync(userId);
                bool step1Completed = personalInfoResponse.Success
                                      && personalInfoResponse.Data != null;

                // ===============================
                // STEP 2: Education
                // ===============================
                var educationResponse = await _cvApiService.GetUserEducationAsync(userId);
                bool step2Completed = educationResponse.Success
                                      && educationResponse.Data != null
                                      && educationResponse.Data.Any();

                // ===============================
                // STEP 3: Experience
                // ===============================
                var experienceResponse = await _cvApiService.GetUserExperiencesAsync(userId);
                bool step3Completed = experienceResponse.Success
                                      && experienceResponse.Data != null
                                      && experienceResponse.Data.Any();

                // ===============================
                // STEP 4: Skills
                // ===============================
                var skillResponse = await _cvApiService.GetUserSkillAsync(userId);
                bool step4Completed = skillResponse.Success
                                      && skillResponse.Data != null
                                      && skillResponse.Data.Any();

                // ===============================
                // STEP 5: Projects
                // ===============================
                var projectResponse = await _cvApiService.GetUserProjectAsync(userId);
                bool step5Completed = projectResponse.Success
                                      && projectResponse.Data != null
                                      && projectResponse.Data.Any();

                // ===============================
                // STEP 6: Languages
                // ===============================
                var languageResponse = await _cvApiService.GetUserLanguageAsync(userId);
                bool step6Completed = languageResponse.Success
                                      && languageResponse.Data != null
                                      && languageResponse.Data.Any();

                // ===============================
                // STEP 7: Summary Objective
                // ===============================
                var summaryObjectiveResponse = await _cvApiService.GetUserSummaryObjectiveAsync(userId);
                bool step7Completed = summaryObjectiveResponse.Success
                                      && summaryObjectiveResponse.Data != null
                                      && summaryObjectiveResponse.Data.Any();

                // ===============================
                // STEP 8: References
                // ===============================
                var referenceResponse = await _cvApiService.GetUserReferencesAsync(userId);
                bool step8Completed = referenceResponse.Success
                                      && referenceResponse.Data != null
                                      && referenceResponse.Data.Any();

                // ===============================
                // STEP LOCK RULES
                // ===============================
                if (!step1Completed)
                    return RedirectToAction("Index");

                if (!step2Completed)
                    return RedirectToAction("Education");

                if (!step3Completed)
                    return RedirectToAction("Experience");

                if (!step4Completed)
                    return RedirectToAction("Skill");

                if (!step5Completed)
                    return RedirectToAction("Project");

                if (!step6Completed)
                    return RedirectToAction("Language");

                if (!step7Completed)
                    return RedirectToAction("SummaryObjective");

                if (!step8Completed)
                    return RedirectToAction("References");

                // ===============================
                // STEP 9: Subscription Check
                // ===============================
                var subscriptionResponse = await _cvApiService.GetUserSubscriptionAsync(userId);
                var subscription = subscriptionResponse.Data;

                bool hasActiveSubscription = subscriptionResponse.Success
                                             && subscription != null
                                             && !subscription.IsExpired;

                if (!hasActiveSubscription)
                    return RedirectToAction("Subscription");

                // ===============================
                // STEP 10: Template Selection Check
                // ===============================
                var templateResponse = await _cvApiService.GetUserSelectedTemplateAsync(userId);

                bool hasSelectedTemplate = templateResponse.Success
                                           && templateResponse.Data != null
                                           && templateResponse.Data.TemplateId > 0;

                if (!hasSelectedTemplate)
                    return RedirectToAction("TemplateSelection");

                // ===============================
                // STEP 11: Preview & Download
                // ===============================
                return RedirectToAction("PreviewDownload");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error loading CV Builder for user {User}",
                    User.Identity?.Name);

                TempData["ErrorMessage"] = "Unexpected error loading CV Builder.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePersonalDetail([FromForm] UserPersonalDetailDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid personal info submitted for user {User}", User.Identity?.Name);
                return BadRequest(new { Success = false, Message = "Please fill all required fields." });
            }

            try
            {
                model.UserId = User.GetUserId();

                var result = await _cvApiService.SaveUserPersonalDetailAsync(model);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save personal info for user {UserId}: {Message}",
                        model.UserId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved personal info successfully for user {UserId}", model.UserId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Personal info saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving personal info for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Education()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserEducationAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load education for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load education data.";
                    return View(new List<UserEducationDto>());
                }

                return View(response.Data ?? new List<UserEducationDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading education for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading education data.";
                return View(new List<UserEducationDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveEducation([FromBody] List<UserEducationDto> educationList)
        {
            if (educationList == null || !educationList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one education record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var edu in educationList)
                {
                    edu.UserId = userId;
                }

                var result = await _cvApiService.SaveUserEducationAsync(educationList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save education for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved education successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Education saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving education for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Experience()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserExperiencesAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load experiences for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load experience data.";
                    return View(new List<UserExperienceDto>());
                }

                return View(response.Data ?? new List<UserExperienceDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading experiences for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading experience data.";
                return View(new List<UserExperienceDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveExperience([FromBody] List<UserExperienceDto> experienceList)
        {
            if (experienceList == null || !experienceList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one experience record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var exp in experienceList)
                {
                    exp.UserId = userId;
                }

                var result = await _cvApiService.SaveUserExperiencesAsync(experienceList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save experiences for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved experiences successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Work experience saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving experiences for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Skill()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserSkillAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load skill for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load skill data.";
                    return View(new List<UserSkillDto>());
                }

                return View(response.Data ?? new List<UserSkillDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading skill for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading skill data.";
                return View(new List<UserSkillDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSkill([FromBody] List<UserSkillDto> skillList)
        {
            if (skillList == null || !skillList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one skill record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var exp in skillList)
                {
                    exp.UserId = userId;
                }

                var result = await _cvApiService.SaveUserSkillAsync(skillList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save skills for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved skills successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Skills saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving skill for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Project()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserProjectAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load project for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load project data.";
                    return View(new List<UserProjectDto>());
                }

                return View(response.Data ?? new List<UserProjectDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading project for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading project data.";
                return View(new List<UserProjectDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveProject([FromBody] List<UserProjectDto> projectList)
        {
            if (projectList == null || !projectList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one project record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var exp in projectList)
                {
                    exp.UserId = userId;
                }

                var result = await _cvApiService.SaveUserProjectAsync(projectList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save project for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved project successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Projects saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving project for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Language()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserLanguageAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load language for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load language data.";
                    return View(new List<UserLanguageDto>());
                }

                return View(response.Data ?? new List<UserLanguageDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading language for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading language data.";
                return View(new List<UserLanguageDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveLanguage([FromBody] List<UserLanguageDto> languageList)
        {
            if (languageList == null || !languageList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one language record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var exp in languageList)
                {
                    exp.UserId = userId;
                }

                var result = await _cvApiService.SaveUserLanguageAsync(languageList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save language for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved language successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Languages saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving language for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SummaryObjective()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserSummaryObjectiveAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning(
                        "Failed to load summary & objective for user {UserId}: {Message}",
                        userId, response.Message);

                    TempData["ErrorMessage"] = response.Message ?? "Unable to load Summary & Objective data.";

                    return View(new UserSummaryObjectiveDto());
                }

                var model = response.Data?.FirstOrDefault() ?? new UserSummaryObjectiveDto();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading summary & objective for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading Summary & Objective data.";

                return View(new UserSummaryObjectiveDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSummaryObjective([FromBody] UserSummaryObjectiveDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid summary & objective submitted for user {User}", User.Identity?.Name);
                return BadRequest(new { Success = false, Message = "Please fill all required fields." });
            }

            try
            {
                model.UserId = User.GetUserId();

                var result = await _cvApiService.SaveUserSummaryObjectiveAsync(new List<UserSummaryObjectiveDto> { model });

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save summary & objective for user {UserId}: {Message}",
                        model.UserId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved summary & objective successfully for user {UserId}", model.UserId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data?.FirstOrDefault(), 
                    Message = "Summary & Objective saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving summary & objective for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> References()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserReferencesAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning("Failed to load references for user {UserId}: {Message}", userId, response.Message);
                    TempData["ErrorMessage"] = response.Message ?? "Unable to load reference data.";
                    return View(new List<UserReferenceDto>());
                }

                return View(response.Data ?? new List<UserReferenceDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading references for user {User}", User.Identity?.Name);
                TempData["ErrorMessage"] = "An unexpected error occurred while loading reference data.";
                return View(new List<UserReferenceDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveReferences([FromBody] List<UserReferenceDto> referenceList)
        {
            if (referenceList == null || !referenceList.Any())
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "At least one reference record is required."
                });
            }

            try
            {
                int userId = User.GetUserId();

                foreach (var reference in referenceList)
                {
                    reference.UserId = userId;
                }

                var result = await _cvApiService.SaveUserReferencesAsync(referenceList);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to save references for user {UserId}: {Message}",
                        userId, result.Message);

                    return BadRequest(new { Success = false, Message = result.Message });
                }

                _logger.LogInformation("Saved references successfully for user {UserId}", userId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "References saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { Success = false, Message = "User not authorized." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving references for user {User}", User.Identity?.Name);
                return StatusCode(500, new { Success = false, Message = "Internal server error." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Subscription()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserSubscriptionAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning(
                        "Failed to load subscription for user {UserId}: {Message}",
                        userId, response.Message);

                    TempData["ErrorMessage"] =
                        response.Message ?? "Unable to load subscription data.";

                    return View(new UserSubscriptionDto());
                }

                var model = response.Data ?? new UserSubscriptionDto();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading subscription for user {User}",
                    User.Identity?.Name);

                TempData["ErrorMessage"] =
                    "An unexpected error occurred while loading subscription data.";

                return View(new UserSubscriptionDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubscription(
            [FromBody] UserSubscriptionDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid subscription submitted for user {User}",
                    User.Identity?.Name);

                return BadRequest(new
                {
                    Success = false,
                    Message = "Please fill all required fields."
                });
            }

            try
            {
                model.UserId = User.GetUserId();

                // Auto calculate start date
                model.StartDate = DateTime.UtcNow;

                // Auto calculate end date from selected plan
                model.EndDate = model.Plan switch
                {
                    SubscriptionPlan.Weekly => model.StartDate.AddDays(7),
                    SubscriptionPlan.Monthly => model.StartDate.AddMonths(1),
                    SubscriptionPlan.Quarterly => model.StartDate.AddMonths(3),
                    SubscriptionPlan.HalfYearly => model.StartDate.AddMonths(6),
                    SubscriptionPlan.Yearly => model.StartDate.AddYears(1),
                    _ => model.StartDate.AddMonths(1)
                };

                var result = await _cvApiService.SaveUserSubscriptionAsync(model);

                if (!result.Success)
                {
                    _logger.LogWarning(
                        "Failed to save subscription for user {UserId}: {Message}",
                        model.UserId,
                        result.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                _logger.LogInformation(
                    "Saved subscription successfully for user {UserId}",
                    model.UserId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Subscription saved successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "User not authorized."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving subscription for user {User}",
                    User.Identity?.Name);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TemplateSelection()
        {
            try
            {
                int userId = User.GetUserId();

                var response = await _cvApiService.GetUserSelectedTemplateAsync(userId);

                if (!response.Success)
                {
                    _logger.LogWarning(
                        "Failed to load template selection for user {UserId}: {Message}",
                        userId, response.Message);

                    TempData["ErrorMessage"] =
                        response.Message ?? "Unable to load template data.";

                    return View(new UserSelectedTemplateDto());
                }

                var model = response.Data ?? new UserSelectedTemplateDto();

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading template selection for user {User}",
                    User.Identity?.Name);

                TempData["ErrorMessage"] =
                    "An unexpected error occurred while loading template data.";

                return View(new UserSelectedTemplateDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTemplateSelection(
            [FromBody] UserSelectedTemplateDto model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Invalid template selection submitted for user {User}",
                    User.Identity?.Name);

                return BadRequest(new
                {
                    Success = false,
                    Message = "Please select a template."
                });
            }

            try
            {
                model.UserId = User.GetUserId();

                var result = await _cvApiService.SaveUserSelectedTemplateAsync(model);

                if (!result.Success)
                {
                    _logger.LogWarning(
                        "Failed to save template selection for user {UserId}: {Message}",
                        model.UserId,
                        result.Message);

                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                _logger.LogInformation(
                    "Saved template successfully for user {UserId}",
                    model.UserId);

                return Ok(new
                {
                    Success = true,
                    Data = result.Data,
                    Message = "Template selected successfully!"
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new
                {
                    Success = false,
                    Message = "User not authorized."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error saving template selection for user {User}",
                    User.Identity?.Name);

                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Internal server error."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Step(int stepNumber)
        {
            if (!CvStepHelper.IsValidStep(stepNumber))
                return NotFound();

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Login", "Account");

            var userId = int.Parse(userIdClaim);

            // Step 1: Personal Info
            var personalInfoResponse = await _cvApiService.GetUserPersonalDetailAsync(userId);
            bool step1Completed = personalInfoResponse.Success && personalInfoResponse.Data != null;

            // Step 2: Education
            var educationResponse = await _cvApiService.GetUserEducationAsync(userId);
            bool step2Completed = educationResponse.Success && educationResponse.Data != null && educationResponse.Data.Any();

            // Step 3: Experience
            var experienceResponse = await _cvApiService.GetUserExperiencesAsync(userId);
            bool step3Completed = experienceResponse.Success && experienceResponse.Data != null && experienceResponse.Data.Any();

            // Step 4: Skill
            var skillResponse = await _cvApiService.GetUserSkillAsync(userId);
            bool step4Completed = skillResponse.Success && skillResponse.Data != null && skillResponse.Data.Any();

            // Lock rules
            if (stepNumber >= (int)CvStep.Education && !step1Completed)
                return RedirectToAction("Index"); 

            if (stepNumber >= (int)CvStep.WorkExperience && !step2Completed)
                return RedirectToAction("Education"); 

            if (stepNumber >= (int)CvStep.PreviewDownload && !step3Completed)
                return RedirectToAction("Experience");

            if (stepNumber >= (int)CvStep.PreviewDownload && !step4Completed)
                return RedirectToAction("Skill");

            // Navigate to the requested step
            return stepNumber switch
            {
                (int)CvStep.PersonalDetail => RedirectToAction("Index"),
                (int)CvStep.Education => RedirectToAction("Education"),
                (int)CvStep.WorkExperience => RedirectToAction("Experience"),
                (int)CvStep.Skills => RedirectToAction("Skill"),
                (int)CvStep.PreviewDownload => RedirectToAction("PreviewDownload"),
                _ => NotFound()
            };
        }
    }
}