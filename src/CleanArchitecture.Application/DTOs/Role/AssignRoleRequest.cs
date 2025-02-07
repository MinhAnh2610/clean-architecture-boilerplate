namespace CleanArchitecture.Application.DTOs.Role;

public record AssignRoleRequest(string UserName, List<string> Roles);
