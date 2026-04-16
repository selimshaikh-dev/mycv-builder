using MYCV.Domain.Entities;

namespace MYCV.Application.Interfaces
{
    /// <summary>
    /// Repository for CV templates
    /// </summary>
    public interface ICvTemplateRepository
    {
        Task<CvTemplate?> GetByIdAsync(int id);
        Task<List<CvTemplate>> GetAllAsync();

        Task AddAsync(CvTemplate template);
        Task UpdateAsync(CvTemplate template);
        Task DeleteAsync(CvTemplate template);
    }
}