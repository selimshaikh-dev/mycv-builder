namespace MYCV.Application.DTOs
{
    /// <summary>
    /// Represents a CV template available for user selection
    /// </summary>
    public class CvTemplateDto
    {
        /// <summary>
        /// Unique identifier of the template
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Template display title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Template preview image path/url
        /// Example: /images/cv-templates/template-1.png
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Short description about who should use this template
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indicates whether this template is premium
        /// </summary>
        public bool IsPremium { get; set; } = false;
    }
}