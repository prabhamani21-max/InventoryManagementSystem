using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InventoryManagementSystem.Repository.Models
{
    [Table("users")]
    public class UserDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [Column("password")]
        public string Password { get; set; }
        [Required]
        [Column("contact_number")]
        public string ContactNumber { get; set; }
        [Column("gender")]
        public int Gender { get; set; }
        [Column("address")]
        public string? Address { get; set; }
        [Column("dob")]
        public DateOnly DOB { get; set; }

        [Column("role_id")]
        public int RoleId { get; set; }
        [Column("status_id")]
        public int StatusId { get; set; }
        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("profile_image")]
        public string? ProfileImage { get; set; }
        [ForeignKey(nameof(RoleId))]

        public virtual RoleDb Role {get; set;}
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedByUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
        

    }
}
