using API.Entities;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IMemberService
    {
        public Task<List<PersonResponse>> GetMemberByUsername(string username);
        Task<BaseResponse> AddMemberForAccount(int userId, CreateMemberRequest request);
        Task<DataResponse<PagedList<MemberForAdminResponse>>> GetMembers(MemberParams param);
    }
}
