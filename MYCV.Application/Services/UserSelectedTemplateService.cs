using MYCV.Application.DTOs;
using MYCV.Application.Interfaces;
using MYCV.Domain.Entities;

namespace MYCV.Application.Services
{
    /// <summary>
    /// Service for managing user selected CV templates
    /// </summary>
    public class UserSelectedTemplateService : IUserSelectedTemplateService
    {
        private readonly IUserSelectedTemplateRepository _userSelectedTemplateRepository;

        public UserSelectedTemplateService(IUserSelectedTemplateRepository userSelectedTemplateRepository)
        {
            _userSelectedTemplateRepository = userSelectedTemplateRepository;
        }

        /// <summary>
        /// Get selected template for a specific user
        /// </summary>
        public async Task<UserSelectedTemplateDto?> GetUserSelectedTemplateAsync(int userId)
        {
            var templates = await _userSelectedTemplateRepository.GetByUserIdAsync(userId);

            var latestTemplate = templates
                .OrderByDescending(t => t.CreatedDate)
                .FirstOrDefault();

            return latestTemplate == null ? null : MapToDto(latestTemplate);
        }

        /// <summary>
        /// Save or update user selected template
        /// </summary>
        public async Task<UserSelectedTemplateDto> SaveUserSelectedTemplateAsync(UserSelectedTemplateDto dto)
        {
            UserSelectedTemplate? entity = null;

            if (dto.Id > 0)
                entity = await _userSelectedTemplateRepository.GetByIdAsync(dto.Id);

            if (entity == null)
            {
                entity = new UserSelectedTemplate
                {
                    UserId = dto.UserId,
                    TemplateId = dto.TemplateId,
                    TemplateName = dto.TemplateName,
                    TemplateImageUrl = dto.TemplateImageUrl
                };

                await _userSelectedTemplateRepository.AddAsync(entity);
            }
            else
            {
                entity.TemplateId = dto.TemplateId;
                entity.TemplateName = dto.TemplateName;
                entity.TemplateImageUrl = dto.TemplateImageUrl;

                await _userSelectedTemplateRepository.UpdateAsync(entity);
            }

            return MapToDto(entity);
        }

        /// <summary>
        /// Map Entity → DTO
        /// </summary>
        private static UserSelectedTemplateDto MapToDto(UserSelectedTemplate entity)
        {
            return new UserSelectedTemplateDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                TemplateId = entity.TemplateId,
                TemplateName = entity.TemplateName,
                TemplateImageUrl = entity.TemplateImageUrl,
                CreatedDate = entity.CreatedDate,
                IsActive = entity.IsActive
            };
        }
    }
}