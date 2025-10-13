
namespace ByteBattlesServer.Microservices.AuthService.Domain.Entities;

public class Role
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }
}