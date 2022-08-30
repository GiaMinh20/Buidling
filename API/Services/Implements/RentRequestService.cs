using API.Data;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class RentRequestService : IRentRequestService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public RentRequestService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse> SendRentRequest(int userId, CreateRentRequest request)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Status == EItemStatus.Empty && i.Id == request.ItemId);
            if (item == null)
                return new BaseResponse { IsSuccess = false, Message = "Item không tồn tại" };
            RentRequest newRent = new RentRequest
            {
                FullName = request.FullName,
                CCCD = request.CCCD,
                ItemId = request.ItemId,
                RenterId = userId,
                NumberOfParent = request.NumberOfParent,
                status = false,
            };
            _context.RentRequests.Add(newRent);
            if (await _context.SaveChangesAsync() > 0)
                return new BaseResponse { IsSuccess = true, Message = "Yêu cầu thành công" };
            return new BaseResponse { IsSuccess = false, Message = "Yêu cầu thất bại" };
        }

        public async Task<DataResponse<PagedList<RentRequest>>> GetRentRequest(RentRequestParams param)
        {
            var query = await _context.RentRequests.Status(param.Status).ToListAsync();

            var reports = PagedList<RentRequest>.ToPagedList(query,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<RentRequest>>
            {
                IsSuccess = true,
                Data = reports
            };
        }

        public async Task<BaseResponse> SendUnRentRequest(int userId, CreateUnRentRequest request)
        {
            var item = await _context.Items.Include(i => i.Renter).FirstOrDefaultAsync(i => i.Id == request.ItemId && i.Status == EItemStatus.Rented && i.Renter.Id == userId);
            if (item == null)
                return new BaseResponse { IsSuccess = false, Message = "Item không sở hữu" };
            UnRentRequest newUnRent = new UnRentRequest
            {
                FullName = request.FullName,
                CCCD = request.CCCD,
                ItemId = request.ItemId,
                RenterId = userId,
                status = false,
            };
            _context.UnRentRequests.Add(newUnRent);
            if (await _context.SaveChangesAsync() > 0)
                return new BaseResponse { IsSuccess = true, Message = "Yêu cầu thành công" };
            return new BaseResponse { IsSuccess = false, Message = "Yêu cầu thất bại" };
        }

        public async Task<DataResponse<PagedList<UnRentRequest>>> GetUnRentRequest(RentRequestParams param)
        {
            var query = await _context.UnRentRequests.Status(param.Status).ToListAsync();

            var response = PagedList<UnRentRequest>.ToPagedList(query,
                param.PageNumber, param.PageSize);

            return new DataResponse<PagedList<UnRentRequest>>
            {
                IsSuccess = true,
                Data = response
            };
        }
    }
}
