using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class VehicleService : IVehicleService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public VehicleService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse> SendVehicleRequest(int userId, CreateVehicleRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            Vehicle newVehicle = new Vehicle
            {
                AccountId = userId,
                AccountName = user.UserName,
                LicensePlates = request.LicensePlates,
                Transportation = request.Transportation,
                Status = false
            };
            _context.Vehicles.Add(newVehicle);
            try
            {
                if (await _context.SaveChangesAsync() > 0)
                    return new BaseResponse { IsSuccess = true, Message = "Yêu cầu thành công" };
            }
            catch (System.Exception)
            {
                return new BaseResponse { IsSuccess = false, Message = "Phương tiện đã được đăng ký" };
            }

            return new BaseResponse { IsSuccess = false, Message = "Yêu cầu thất bại" };
        }

        public async Task<DataResponse<PagedList<Vehicle>>> GetVehicles(VehicleParams param)
        {
            var query = await _context.Vehicles
                .AccountName(param.AccountName)
                .LicensePlates(param.LicensePlates)
                .Transportation(param.Transportation)
                .Status(param.Status)
                .ToListAsync();

            var reports = PagedList<Vehicle>.ToPagedList(query,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<Vehicle>>
            {
                IsSuccess = true,
                Data = reports
            };
        }

        public async Task<BaseResponse> AcceptVehicle(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
                return new BaseResponse { IsSuccess = false, Message = "Không tìm thấy phương tiện" };
            vehicle.Status = true;
            if (await _context.SaveChangesAsync() > 0)
                return new BaseResponse { IsSuccess = true, Message = "Chấp nhận thành công" };
            return new BaseResponse { IsSuccess = false, Message = "Chấp nhận thất bại" };
        }

        public async Task<DataResponse<PagedList<Vehicle>>> GetVehiclesOfAccount(int userId, VehicleForAccountParams param)
        {
            var query = await _context.Vehicles
                .Where(v => v.AccountId == userId)
                .LicensePlates(param.LicensePlates)
                .Transportation(param.Transportation)
                .Status(param.Status)
                .ToListAsync();

            var responses = PagedList<Vehicle>.ToPagedList(query,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<Vehicle>>
            {
                IsSuccess = true,
                Data = responses
            };
        }
    }
}
