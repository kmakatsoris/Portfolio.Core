using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Portfolio.Core.Interfaces.Contract;

namespace Portfolio.Core.Types.Contracts
{
    public class BasicSkillsAndProjectsType : IBasicProjectsAndSkillsPropertiesContract
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
        public Guid ResourceIntroId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Category { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        public Guid ResourceMainId { get; set; }
        public string Tags { get; set; } // IEnumarable<string>

        /*
        [NotMapped] // Tags
        public IEnumerable<string> TagsList
        {
            get => JsonConvert.DeserializeObject<IEnumerable<string>>(Tags);
            set => Tags = JsonConvert.SerializeObject(value);
        }
        */

        public string Grades { get; set; } // JSON<T_Grade> Mine Grade
        public string Comments { get; set; } // JSON<T_Comment[]> Others
        public string MemberLabels { get; set; }

    }
}