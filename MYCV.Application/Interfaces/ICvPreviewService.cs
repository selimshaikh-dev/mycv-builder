using MYCV.Application.DTOs;

namespace MYCV.Application.Interfaces
{
    /// <summary>
    /// Service interface for generating full CV preview data
    /// </summary>
    public interface ICvPreviewService
    {
        /// <summary>
        /// Get full CV preview data for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>CvPreviewDto with all CV information</returns>
        Task<CvPreviewDto> GetCvPreviewAsync(int userId);
    }
}