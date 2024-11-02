using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain;

public class Table
{
    public int Id { get; set; }
    
    [MaxLength(128)]
    public string TableName { get; set; } = default!;
    
    public int RestaurantId { get; set; } // FK!
    public Restaurant? Restaurant { get; set; }
}