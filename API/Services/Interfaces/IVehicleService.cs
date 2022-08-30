using API.Entities;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<BaseResponse> SendVehicleRequest(int userId, CreateVehicleRequest request);
        Task<DataResponse<PagedList<Vehicle>>> GetVehicles(VehicleParams param);
        Task<BaseResponse> AcceptVehicle(int vehicleId);
    }
}
