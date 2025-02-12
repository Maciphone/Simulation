using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;


namespace Backend.MongoDb.Model;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<string>
{
    
    public string? UserDataId { get; set; }
    
}