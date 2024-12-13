using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TestController.Models;

public class OutputAnalizeData
{
    [Required]
    [JsonPropertyName("name")]
    [MinLength(1, ErrorMessage = "Name cannot be empty")]
    public string? Name { get; set; }
    
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("number-of-friends")]
    public int NumberOfFriends { get; set; }
    
    [JsonPropertyName("employee-friends-couples")]
    public string? EmployeeFriendCouples { get; set; }
}