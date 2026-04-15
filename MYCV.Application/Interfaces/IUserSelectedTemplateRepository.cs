using MYCV.Domain.Entities;

namespace MYCV.Application.Interfaces
{
    public interface IUserSelectedTemplateRepository
    {
        Task<UserSelectedTemplate?> GetByIdAsync(int id);
        Task<List<UserSelectedTemplate>> GetByUserIdAsync(int userId);
        Task AddAsync(UserSelectedTemplate selectedTemplate);
        Task UpdateAsync(UserSelectedTemplate selectedTemplate);
        Task DeleteAsync(UserSelectedTemplate selectedTemplate);
    }
}