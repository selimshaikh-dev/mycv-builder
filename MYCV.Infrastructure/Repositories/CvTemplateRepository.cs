using Microsoft.EntityFrameworkCore;
using MYCV.Application.Interfaces;
using MYCV.Domain.Entities;
using MYCV.Infrastructure.Data;

namespace MYCV.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing CV templates
    /// </summary>
    public class CvTemplateRepository : ICvTemplateRepository
    {
        private readonly MyCvDbContext _context;

        public CvTemplateRepository(MyCvDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get template by ID
        /// </summary>
        public async Task<CvTemplate?> GetByIdAsync(int id)
        {
            return await _context.CvTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Get all CV templates
        /// </summary>
        public async Task<List<CvTemplate>> GetAllAsync()
        {
            return await _context.CvTemplates
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Add new CV template
        /// </summary>
        public async Task AddAsync(CvTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            await _context.CvTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Update CV template
        /// </summary>
        public async Task UpdateAsync(CvTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            _context.CvTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Delete CV template
        /// </summary>
        public async Task DeleteAsync(CvTemplate template)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            _context.CvTemplates.Remove(template);
            await _context.SaveChangesAsync();
        }
    }
}