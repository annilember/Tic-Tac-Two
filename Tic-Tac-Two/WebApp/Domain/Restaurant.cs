using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain;

public class Restaurant
{
    public int Id { get; set; }

    [MaxLength(128)] public string RestaurantName { get; set; } = default!;
    
    public ICollection<Table>? Tables { get; set; }
}