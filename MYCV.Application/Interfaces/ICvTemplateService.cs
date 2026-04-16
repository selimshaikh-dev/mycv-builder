using MYCV.Application.DTOs;

namespace MYCV.Application.Interfaces
{
    /// <summary>
    /// Service interface for managing CV templates
    /// </summary>
    public interface ICvTemplateService
    {
        /// <summary>
        /// Get all available CV templates
        /// </summary>
        Task<List<CvTemplateDto>> GetAllAsync();

        /// <summary>
        /// Get CV template by ID
        /// </summary>
        Task<CvTemplateDto?> GetByIdAsync(int id);
    }
}