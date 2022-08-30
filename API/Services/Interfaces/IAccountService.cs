using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IAccountService
    {
        Task<LoginResponse> Login(LoginRequest request);
        Task<DataResponse<RegisterResponse>> Register(RegisterRequest registerDto);
        Task<BaseResponse> ConfirmEmail(string userId, string token);
        Task<BaseResponse> RemoveUser(int userId);
        Task<LoginResponse> GetCurrentUser(string username);
        Task<ForgetPasswordResponse> ForgetPasswordAsync(string email);
        Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request);

        Task<ProfileResponse> GetProfile(string name);
        Task<DataResponse<ProfileResponse>> EditProfile(string username, EditProfileRequest editProfileRequest);

        Task<DataResponse<PagedList<AccountResponse>>> GetAccounts(AccountParams param);
        Task<BaseResponse> BanMemberAccount(int userId);
    }
}
