using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Configuration
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = default!;
    
    public int BoardWidth { get; set; }
    public int BoardHeight { get; set; }
    
    // I won't include data for all saved games => nullable
    public ICollection<SaveGame>? SaveGames { get; set; }

    public override string ToString()
    {
        return Id + " " + Name + " (" + BoardWidth + "x" + BoardHeight 
               + ") Games: " 
               + (SaveGames?.Count.ToString() ?? "not joined");
    }
}