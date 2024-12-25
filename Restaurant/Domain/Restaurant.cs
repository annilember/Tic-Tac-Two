using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Restaurant
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    
}