using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SavedGame
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    [MaxLength(128)]
    public string ModeName { get; set; } = default!;
    
    [MaxLength(128)]
    public string PlayerXName { get; set; } = default!;
    
    [MaxLength(128)]
    public string PlayerOName { get; set; } = default!;
    
    [MaxLength(128)]
    public string PlayerXPassword { get; set; } = default!;
    
    [MaxLength(128)]
    public string PlayerOPassword { get; set; } = default!;

    [MaxLength(128)]
    public string CreatedAtDateTime { get; set; } = default!;
    
    [MaxLength(10240)]
    public string State { get; set; } = default!;
    
    public int ConfigurationId { get; set; }

    public GameConfiguration? Configuration { get; set; }
}