using CleanArchitecture.Application.DTOs.User;

namespace CleanArchitecture.Application.ServiceContracts;

public interface IUserService
{
  Task<Result<UserProfileResponse>> GetUserProfile();
  Task<Result<UserProfileResponse>> UpdateUserProfileAsync(UpdateProfileRequest updateProfileRequest);
}
