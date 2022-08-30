using API.Entities;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IRentRequestService
    {
        Task<BaseResponse> SendRentRequest(int userId, CreateRentRequest request);
        Task<DataResponse<PagedList<RentRequest>>> GetRentRequest(RentRequestParams param);

        Task<BaseResponse> SendUnRentRequest(int userId, CreateUnRentRequest request);
        Task<DataResponse<PagedList<UnRentRequest>>> GetUnRentRequest(RentRequestParams param);
    }
}
