using System.ComponentModel.DataAnnotations;

namespace Domain;

public class SaveGame
{
    // Primary Key
    public int Id { get; set; }

    [MaxLength(128)]
    public string CreatedAtDateTime { get; set; } = default!;
    
    [MaxLength(10240)]
    public string State { get; set; } = default!;
    
    // Expose the Foreign Key
    public int ConfigurationId { get; set; }
    // write this as a nullable when the data might not always be there
    // i.e. when you explicitly write out a join, you might forget
    // FK is always there, but the data might not be
    public Configuration? Configuration { get; set; }
    
}