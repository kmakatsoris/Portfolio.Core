using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Types.Enums.Users;

namespace Portfolio.Core.Types.Models.Users
{
    [Table("Users")]
    public class User : IUserProperties
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(255)]
        public string ConfirmPassword { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required]
        public byte[] Salt { get; set; }

        [Required]
        [MaxLength(512)]
        public string Token { get; set; }

        [Required]
        public string Role { get; set; } // UserRoleEnum

        [Required]
        public string Status { get; set; } // UserStatusEnum

        [Required]

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}