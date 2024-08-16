using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Interfaces.Context;

namespace Portfolio.Core.Types.Models.MetaData
{
    [Table("MetaData")]
    public class MetaDataModel : IMetaDataProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
        public string Data { get; set; }
    }
}