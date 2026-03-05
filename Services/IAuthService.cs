using DiaryApi.Models;

public interface IAuthService
{
    Task<UserModel?>  Authenticate(LoginRequest loginRequest);
    Task<UserModel?> SignUpAsync(SignUpResquest signUpResquest); 
}