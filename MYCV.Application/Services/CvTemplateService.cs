using MYCV.Application.DTOs;
using MYCV.Application.Interfaces;
using MYCV.Domain.Entities;

namespace MYCV.Application.Services
{
    /// <summary>
    /// Service for managing CV templates
    /// </summary>
    public class CvTemplateService : ICvTemplateService
    {
        private readonly ICvTemplateRepository _cvTemplateRepository;

        public CvTemplateService(ICvTemplateRepository cvTemplateRepository)
        {
            _cvTemplateRepository = cvTemplateRepository;
        }

        /// <summary>
        /// Get all CV templates
        /// </summary>
        public async Task<List<CvTemplateDto>> GetAllAsync()
        {
            var templates = await _cvTemplateRepository.GetAllAsync();

            return templates.Select(MapToDto).ToList();
        }

        /// <summary>
        /// Get CV template by ID
        /// </summary>
        public async Task<CvTemplateDto?> GetByIdAsync(int id)
        {
            var template = await _cvTemplateRepository.GetByIdAsync(id);

            return template == null ? null : MapToDto(template);
        }

        /// <summary>
        /// Map Entity → DTO
        /// </summary>
        private static CvTemplateDto MapToDto(CvTemplate entity)
        {
            return new CvTemplateDto
            {
                Id = entity.Id,
                Title = entity.Title,
                ImageUrl = entity.ImageUrl,
                Description = entity.Description,
                IsPremium = entity.IsPremium
            };
        }
    }
}