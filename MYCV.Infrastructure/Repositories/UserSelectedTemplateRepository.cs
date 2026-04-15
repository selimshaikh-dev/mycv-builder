using Microsoft.EntityFrameworkCore;
using MYCV.Application.Interfaces;
using MYCV.Domain.Entities;
using MYCV.Infrastructure.Data;

namespace MYCV.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing user selected CV templates
    /// </summary>
    public class UserSelectedTemplateRepository : IUserSelectedTemplateRepository
    {
        private readonly MyCvDbContext _context;

        public UserSelectedTemplateRepository(MyCvDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get template selection by ID
        /// </summary>
        public async Task<UserSelectedTemplate?> GetByIdAsync(int id)
        {
            return await _context.UserSelectedTemplates
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Get all selected templates by user
        /// </summary>
        public async Task<List<UserSelectedTemplate>> GetByUserIdAsync(int userId)
        {
            return await _context.UserSelectedTemplates
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        /// <summary>
        /// Add new template selection
        /// </summary>
        public async Task AddAsync(UserSelectedTemplate selectedTemplate)
        {
            await _context.UserSelectedTemplates.AddAsync(selectedTemplate);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update template selection
        /// </summary>
        public async Task UpdateAsync(UserSelectedTemplate selectedTemplate)
        {
            _context.UserSelectedTemplates.Update(selectedTemplate);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Soft delete template selection (recommended)
        /// </summary>
        public async Task DeleteAsync(UserSelectedTemplate selectedTemplate)
        {
            selectedTemplate.IsDeleted = true;
            selectedTemplate.DeletedDate = DateTime.UtcNow;

            _context.UserSelectedTemplates.Update(selectedTemplate);
            await _context.SaveChangesAsync();
        }
    }
}