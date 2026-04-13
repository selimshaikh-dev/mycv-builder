
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MYCV.Domain.Entities
{
    public class UserSelectedTemplate : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [Required]
        public int TemplateId { get; set; }  

        [MaxLength(100)]
        public string? TemplateName { get; set; }

        [MaxLength(250)]
        public string? TemplateImageUrl { get; set; }
    }
}