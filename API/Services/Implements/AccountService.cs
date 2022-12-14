using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace API.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IMemberService _memberService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly BuildingContext _context;
        public AccountService(UserManager<Account> userManager,
            SignInManager<Account> signInManager,
            ITokenService tokenService,
            BuildingContext context,
            IEmailService emailService,
            IMapper mapper,
            IMemberService memberService,
            IImageService imageService,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _mapper = mapper;
            _memberService = memberService;
            _imageService = imageService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<DataResponse<RegisterResponse>> Register(RegisterRequest request)
        {
            var checkRequest = CheckExistAccount(request);
            if (!checkRequest.Result.IsSuccess)
                return new DataResponse<RegisterResponse>
                {
                    IsSuccess = false,
                    Message = checkRequest.Result.Message
                };

            var newAcount = new Account
            {
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(newAcount, request.Password);
            return CompleteRegister(result, newAcount).Result;
        }

        public async Task<BaseResponse> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy người dùng"
                };

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return new BaseResponse
                {
                    Message = "Xác nhận Email thành công",
                    IsSuccess = true,
                };

            }
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Xác nhận Email thất bại",
            };
        }

        public async Task<LoginResponse> GetCurrentUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                return new LoginResponse
                {
                    Username = user.UserName,
                    AvatarUrl = user.AvatarUrl,
                    Token = await _tokenService.GenerateToken(user)
                };
            }
            return null;
        }

        public async Task<BaseResponse> RemoveUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            _context.Users.Remove(user);

            var rs = await _context.SaveChangesAsync() > 0;
            if (rs)
            {
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Xóa thành công",
                };
            }
            else
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Xóa thất bại"
                };
            }
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || user.EmailConfirmed == false) return null;
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!result.Succeeded) return null;
            return new LoginResponse
            {
                Username = user.UserName,
                AvatarUrl = user.AvatarUrl,
                Token = await _tokenService.GenerateToken(user)
            };
        }

        public async Task<ForgetPasswordResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ForgetPasswordResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy Email của người dùng",
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string url = $"https://buildingmanager.herokuapp.com/reset-password?email={email}&token={token}";
            /* Template*/
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
            + "templates" + Path.DirectorySeparatorChar.ToString() +
            "ForgotPassTemplate.html";

            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }
            string messageBody = string.Format(HtmlBody, user.UserName, user.Email, url.ToString());

            await _emailService.SendEmailAsync(email, "ResetPassword", messageBody, null);

            return new ForgetPasswordResponse
            {
                IsSuccess = true,
                Message = "URL đặt lại mật khẩu đã được gửi đến email thành công!",
                Data = token
            };
        }

        public async Task<BaseResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy tài khoản với Email này",
                };

            if (request.Password != request.ConfirmPassword)
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "Nhập lại mật khẩu sai",
                };

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

            if (result.Succeeded)
                return new BaseResponse
                {
                    Message = "Mật khẩu đã được đặt lại thành công!",
                    IsSuccess = true,
                };

            return new BaseResponse
            {
                Message = "Đã xảy ra sự cố",
                IsSuccess = false,
            };
        }

        public async Task<ProfileResponse> GetProfile(string name)
        {
            var user = await _context.Users
                    .FirstOrDefaultAsync(x => x.UserName == name);
            if (user == null) return null;
            var response = _mapper.Map<ProfileResponse>(user);

            return response;

        }

        public async Task<DataResponse<ProfileResponse>> EditProfile(string username, EditProfileRequest request)
        {
            var user = await _context.Users
                .Include(a => a.Members)
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (request.Avatar != null)
            {
                var imageResult = await _imageService.AddImageAsync(request.Avatar);

                if (imageResult.Error != null) return new DataResponse<ProfileResponse> { IsSuccess = false, Message = "Lỗi tải ảnh" };
                if (!string.IsNullOrEmpty(user.AvatarId))
                    await _imageService.DeleteMediaAsync(user.AvatarId);
                user.AvatarUrl = imageResult.SecureUrl.ToString();
                user.AvatarId = imageResult.PublicId;
            }
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;
            _context.Users.Update(user);
            if (await _context.SaveChangesAsync() > 0)
            {
                return new DataResponse<ProfileResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProfileResponse>(user)
                };
            }
            return new DataResponse<ProfileResponse> { IsSuccess = false, Message = "Thay đổi thất bại" };
        }

        public async Task<DataResponse<PagedList<AccountResponse>>> GetAccounts(AccountParams param)
        {
            var users = await _context.Users
                .Username(param.AccountName)
                .Email(param.Email)
                .ToListAsync();
            var userResponses = _mapper.Map<List<AccountResponse>>(users);
            for (int i = 0; i < userResponses.Count(); i++)
            {
                var roles = await _userManager.GetRolesAsync(users.ElementAt(i));
                userResponses[i].Roles = roles.ToList();
            }

            var responses = PagedList<AccountResponse>.ToPagedList(userResponses,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<AccountResponse>>
            {
                IsSuccess = true,
                Data = responses
            };
        }

        public async Task<BaseResponse> BanMemberAccount(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy tài khoản có Id: " + userId };
            var roles = new List<string>();
            if (user != null)
                roles = (await _userManager.GetRolesAsync(user)).ToList();

            if (roles.Count() > 0 && !roles.Contains("Admin"))
            {
                if (user.EmailConfirmed == true)
                {
                    user.EmailConfirmed = false;

                    _context.Users.Update(user);
                    var banrs = await _context.SaveChangesAsync() > 0;
                    if (banrs) return new BaseResponse { IsSuccess = true, Message = "Cấm thành công" };
                }
                if (user.EmailConfirmed == false)
                {
                    user.EmailConfirmed = true;
                    _context.Users.Update(user);
                    var unbanrs = await _context.SaveChangesAsync() > 0;
                    if (unbanrs) return new BaseResponse { IsSuccess = true, Message = "Hủy cấm thành công" };
                }
            }
            return new BaseResponse { IsSuccess = false, Message = "Thay đổi thất bại" };
        }




        private async Task<BaseResponse> CheckExistAccount(RegisterRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
                return new BaseResponse
                {
                    Message = "Tên đăng nhập đã tồn tại",
                    IsSuccess = false,
                };
            if (await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email) != null)
                return new BaseResponse
                {
                    Message = "Email đã được sử dụng",
                    IsSuccess = false,
                };

            if (await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber) != null)
                return new BaseResponse
                {
                    Message = "Số điện thoại đã được sử dụng",
                    IsSuccess = false,
                };
            if (request.Password != request.ConfirmPass)
                return new BaseResponse
                {
                    Message = "Xác nhận mật khẩu sai",
                    IsSuccess = false,
                };
            //if (Regex.Match(request.PhoneNumber, @"^[0-9]+${9,11}").Success)
            //{
            //    return new BaseResponse
            //    {
            //        Message = "Số điện thoai không đúng định dạng",
            //        IsSuccess = false,
            //    };
            //}
            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private async Task<DataResponse<RegisterResponse>> CompleteRegister(IdentityResult result, Account newAcount)
        {
            if (!result.Succeeded)
            {
                return new DataResponse<RegisterResponse>
                {
                    Message = "Đăng ký thất bại",
                    IsSuccess = false,
                };
            }
            else
            {
                await _userManager.AddToRoleAsync(newAcount, "Member");
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newAcount);
                //var uriBuilder = new UriBuilder("https://buildingmanager-api.herokuapp.com/api/system/confirm-email");
                var uriBuilder = new UriBuilder("https://localhost:5001/api/system/confirm-email");

                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["token"] = token;
                query["userId"] = newAcount.Id.ToString();
                uriBuilder.Query = query.ToString();
                var urlString = uriBuilder.ToString();

                /* Template*/
                var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                + "templates" + Path.DirectorySeparatorChar.ToString() +
                "EmailTemplate.html";

                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }
                //Name: { 0}
                //Email: { 1}
                //Phone: { 2}

                string messageBody = string.Format(HtmlBody, newAcount.UserName, newAcount.Email, newAcount.PhoneNumber, urlString);


                //await _emailService.SendEmailAsync(newAcount.Email, "ConfirmMemberEmail", $"<html><body><p>Chào {newAcount.UserName},</p><p>Vui lòng bấm vào liên kết để xác nhận email của bạn</p><a href=\"{urlString}\"><strong>Xác nhận Email</strong></a></body></html>");
                await _emailService.SendEmailAsync(newAcount.Email, "ConfirmMemberEmail", messageBody,null);
                return new DataResponse<RegisterResponse>
                {
                    Message = "Đăng ký thành công",
                    IsSuccess = true,
                    Data = new RegisterResponse
                    {
                        Username = newAcount.UserName,
                        AvatarUrl = newAcount.AvatarUrl
                    }
                };
            }
        }
    }
}
