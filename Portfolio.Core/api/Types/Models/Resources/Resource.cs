using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Interfaces.Context.Resources;

namespace Portfolio.Core.Types.Models.Resources
{
    [Table("Resources")]
    public class Resource : IResourceProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(1000)]
        public string BriefDescription { get; set; }

        [Required]
        public string ExtensiveDescription { get; set; } // IEnumarable<string>

        [Required]
        public Guid MetaDataID { get; set; }

        [Required]
        public string RenderPath { get; set; } // Index

        [Required]
        public string StoragePath { get; set; }

        [Required]
        public string PreviewData { get; set; } // JSON

        [Required]
        public string Dimension { get; set; }

        [Required]
        public string Tags { get; set; }

        [Required]
        public string Type { get; set; } // Enum

        [Required]

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}