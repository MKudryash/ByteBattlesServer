
namespace ByteBattlesServer.Microservices.AuthService.Domain.Entities;

public class Role:Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    private Role() { }

    public Role(string name, string description)
    {
        Name = name;
        Description = description;
    }
}