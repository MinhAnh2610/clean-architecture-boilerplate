using CleanArchitecture.Application.DTOs.Role;
using CleanArchitecture.Application.DTOs.User;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IRoleService
{
  Task<Result<UserProfileResponse>> AssignRoleAsync(AssignRoleRequest request);
}
