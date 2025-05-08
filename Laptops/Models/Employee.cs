using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public int employee_id { get; set; }

    [Required]
    public string Email { get; set; }
    public string? firstname { get; set; }
    public string? lastname { get; set; }
    public string? contactnumber { get; set; }

}
