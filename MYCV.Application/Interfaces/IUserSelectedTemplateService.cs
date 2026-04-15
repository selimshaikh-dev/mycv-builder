using MYCV.Application.DTOs;

namespace MYCV.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing user selected CV templates
    /// </summary>
    public interface IUserSelectedTemplateService
    {
        /// <summary>
        /// Get selected template for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>UserSelectedTemplateDto if found, otherwise null</returns>
        Task<UserSelectedTemplateDto?> GetUserSelectedTemplateAsync(int userId);

        /// <summary>
        /// Save or update selected template for a user
        /// </summary>
        /// <param name="dto">Template selection data</param>
        /// <returns>The saved UserSelectedTemplateDto</returns>
        Task<UserSelectedTemplateDto> SaveUserSelectedTemplateAsync(UserSelectedTemplateDto dto);
    }
}