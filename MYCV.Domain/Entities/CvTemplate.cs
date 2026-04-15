
namespace MYCV.Domain.Entities
{
    public class CvTemplate : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPremium { get; set; } = false;
    }
}