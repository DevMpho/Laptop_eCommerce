using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int employee_id { get; set; }

    [Required]
    public string Email { get; set; }

}
