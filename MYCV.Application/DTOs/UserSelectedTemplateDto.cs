namespace MYCV.Application.DTOs
{
    /// <summary>
    /// DTO for user selected CV template
    /// </summary>
    public class UserSelectedTemplateDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TemplateId { get; set; }

        public string? TemplateName { get; set; }

        public string? TemplateImageUrl { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}