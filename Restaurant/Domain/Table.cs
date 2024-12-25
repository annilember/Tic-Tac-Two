using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Table
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public int Capacity { get; set; } = default!;
}