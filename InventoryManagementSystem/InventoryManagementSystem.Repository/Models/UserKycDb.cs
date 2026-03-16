using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Repository.Models
{
    [Table("user_kyc")]
    public class UserKycDb
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("user_id")]
        public long UserId { get; set; }
        [Column("pan_card_number")]
        public string? PanCardNumber { get; set; }   // Should these be nullable fields? We are keeping KYCID in user table as a nullable field, so should these two be nullable here?
        [Column("aadhaar_card_number")]
        public string AadhaarCardNumber { get; set; } // They should be unique per userId
        [Column("is_verified")]
        public bool IsVerified { get; set; }   // How to conclude this Isverified? Should it be like OTP verification or just a physical verification by sales person at POC
        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        [Column("created_by")]
        public long CreatedBy { get; set; }
        [Column("updated_by")]
        public long? UpdatedBy { get; set; }
        [Column("updated_date")]
        public DateTime? UpdatedDate { get; set; }
        [Column("status_id")]
        public int StatusId { get; set; }
        [ForeignKey(nameof(UserId))]

        public virtual UserDb User { get; set; }
        [ForeignKey(nameof(StatusId))]

        public virtual GenericStatusDb Status { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public virtual UserDb CreatedUser { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public virtual UserDb? UpdatedByUser { get; set; }
    }
}