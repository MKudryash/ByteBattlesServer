using System.Text.Json.Serialization;

namespace ByteBattlesServer.Domain.enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    student,
    teacher ,
    admin 
}
