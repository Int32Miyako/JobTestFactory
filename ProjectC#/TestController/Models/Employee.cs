using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestController.Models;

public class Employee
{
    [JsonPropertyName("_id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
    
    [JsonPropertyName("balance")]
    public string? Balance { get; set; }
    
    [JsonPropertyName("age")]
    public int Age { get; set; }
    
    [JsonPropertyName("eyeColor")]
    public string? EyeColor { get; set; }
    
    [Required]
    [JsonPropertyName("name")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string? Name { get; set; }
        
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }
        
    [JsonPropertyName("company")]
    public string? Company { get; set; }
        
    [JsonPropertyName("email")]
    public string? Email { get; set; }

        
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    
    [JsonPropertyName("friends")]
    public List<FriendOfEmployee>? Friends { get; set; }
        
    [JsonPropertyName("favoriteFruit")]
    public string? FavoriteFruit { get; set; }
}