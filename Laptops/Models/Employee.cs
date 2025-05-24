using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Laptops.Models;

public class Employee
{
    [Key]
    public int employee_id { get; set; }

    [Required]
    public string Email { get; set; }
    public string? firstname { get; set; }
    public string? lastname { get; set; }
    public long? contactnumber { get; set; }

    // New foreign key
    public int RoleId { get; set; }
    [ForeignKey("RoleId")] 
    public Role? Role { get; set; }

}
