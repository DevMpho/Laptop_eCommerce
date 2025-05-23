using System.ComponentModel.DataAnnotations;

namespace Laptops.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; }  // e.g., "MSP", "Finance", "HR"
    }

}
