﻿using System.ComponentModel.DataAnnotations;

namespace BPHN.ModelLayer
{
    public class Account : BaseModel
    {
        [Required]
        [MaxLength(255)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(500)]
        public string Password { get; set; }
        public string Gender { get; set; }
        [Required]
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        public RoleEnum Role { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Status { get; set; }
        public Guid? ParentId { get; set; }
        public string IPAddress { get; set; }
        public List<Permission>? Permissions { get; set; }
        public List<Guid> RelationIds { get; set; }
        public string LanguageConfig { get; set; }
    }
}
