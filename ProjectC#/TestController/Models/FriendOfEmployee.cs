using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestController.Models;

public class FriendOfEmployee
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [Required]
    [JsonPropertyName("name")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string? Name { get; set; }
}