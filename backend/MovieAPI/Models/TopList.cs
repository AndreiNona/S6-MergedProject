namespace MovieAPI.Models;

public class TopList
{
    public int Id { get; set; } // Unique ID for each top list
    public string Name { get; set; } // Name of the top list (e.g., "Star Wars Top List")
    public string UserId { get; set; } // Reference to the user

    // List of movies in this top list (represented by movie IDs)
    public List<int> MovieIds { get; set; } = new List<int>(); 
}