using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Client
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(128)]
    public string Number { get; set; } = default!;
    
    [MaxLength(128)]
    public string Email { get; set; } = default!;
}