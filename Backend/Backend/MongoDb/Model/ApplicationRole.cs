using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Backend.MongoDb.Model;

[CollectionName("Roles")]
public class ApplicationRole :  MongoIdentityRole<string>
{
    public ApplicationRole() : base() { }

    public ApplicationRole(string roleName) : base(roleName) { }
}