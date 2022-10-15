using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class MemberService : IMemberService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public MemberService(BuildingContext context,
            IMapper mapper,
            IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
        }
        public async Task<List<PersonResponse>> GetMemberByUsername(string username)
        {
            var members = await _context.Members
                .Include(c => c.Account)
                .Include(m => m.PlaceOfOrigin)
                .Where(p => p.Account.UserName == username && p.Status == true)
                .OrderByDescending(m => m.CreateDate)
                .ToListAsync();
            return _mapper.Map<List<PersonResponse>>(members);
        }

        public async Task<BaseResponse> AddMemberForAccount(int userId, CreateMemberRequest request)
        {
            var account = await _context.Users.Include(a => a.Members).FirstOrDefaultAsync(a => a.Id == userId);
            if (account.Members.Count < account.NumberOfParent)
            {
                var member = _mapper.Map<Member>(request);
                member.Account = account;
                if (request.AvatarUrl != null)
                {
                    if (!string.IsNullOrEmpty(member.AvatarId))
                    {
                        await _imageService.DeleteMediaAsync(member.AvatarId);
                    }
                    var avatarResult = await _imageService.AddImageAsync(request.AvatarUrl);
                    if (avatarResult.Error != null) return new BaseResponse
                    {
                        IsSuccess = false,
                        Message = "Lỗi tải video"
                    };
                    member.AvatarUrl = avatarResult.SecureUrl.ToString();
                    member.AvatarId = avatarResult.PublicId;
                }
                _context.Members.Add(member);
                try
                {
                    if (await _context.SaveChangesAsync() > 0)
                        return new BaseResponse { IsSuccess = true, Message = "Thành công" };
                }
                catch (System.Exception)
                {
                    return new BaseResponse { IsSuccess = false, Message = "Thất bại" };
                }
            }
            return new BaseResponse { IsSuccess = false, Message = "Thất bại" };
        }

        public async Task<BaseResponse> AcceptMember(AcceptMemberRequest request)
        {
            var member = await _context.Members.FindAsync(request.Id);
            if (member == null) return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy thông tin" };
            member.Status = true;
            try
            {
                if (await _context.SaveChangesAsync() > 0)
                    return new BaseResponse { IsSuccess = true, Message = "Thành công" };
            }
            catch (System.Exception)
            {
                return new BaseResponse { IsSuccess = false, Message = "Thất bại" };
            }
            return new BaseResponse { IsSuccess = false, Message = "Thất bại" };
        }
        public async Task<DataResponse<PagedList<MemberForAdminResponse>>> GetMembers(MemberParams param)
        {
            var query = await _context.Members
                .Include(m => m.Account)
                .Include(m => m.PlaceOfOrigin)
                .AccountName(param.AccountName)
                .FullName(param.FullName)
                .Gender(param.Gender)
                .CCCD(param.CCCD)
                .ToListAsync();
            List<MemberForAdminResponse> members = _mapper.Map<List<MemberForAdminResponse>>(query);

            var responses = PagedList<MemberForAdminResponse>.ToPagedList(members,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<MemberForAdminResponse>>
            {
                IsSuccess = true,
                Data = responses
            };
        }
    }
}
