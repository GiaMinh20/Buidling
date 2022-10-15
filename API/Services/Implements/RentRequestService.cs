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
        private readonly INotificationService _notificationService;

        public RentRequestService(BuildingContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
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
                Status = false,
            };
            _context.RentRequests.Add(newRent);
            int existRentRequest = (await _context.RentRequests.Where(r => r.ItemId == request.ItemId).ToListAsync()).Count();
            var rs = await _notificationService.PostNotificatonForAccount(2, new CreateNotificationRequest { Title = "Gửi yêu cầu thuê thành công", Content = $"Bạn là người yêu cầu thuê thứ ${existRentRequest + 1}\\nChúng tôi sẽ liên lạc với bạn trong vòng 7 ngày" }, userId);
            if (rs.IsSuccess == true)
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
                return new BaseResponse { IsSuccess = false, Message = "Item tài khoản không sở hữu" };
            UnRentRequest newUnRent = new UnRentRequest
            {
                FullName = request.FullName,
                CCCD = request.CCCD,
                ItemId = request.ItemId,
                RenterId = userId,
                Status = false,
            };
            _context.UnRentRequests.Add(newUnRent);
            var rs = await _notificationService.PostNotificatonForAccount(2, new CreateNotificationRequest { Title = "Gửi yêu cầu hủy thuê thành công", Content = $"Chúng tôi sẽ liên lạc với bạn trong vòng 7 ngày" }, userId);
            if (rs.IsSuccess == true)
                return new BaseResponse { IsSuccess = true, Message = "Yêu cầu thành công" };
            return new BaseResponse { IsSuccess = false, Message = "Yêu cầu thất bại" };
        }

        public async Task<DataResponse<PagedList<UnRentRequest>>> GetUnRentRequest(RentRequestParams param)
        {
            var query = await _context.UnRentRequests.Status(param.Status).OrderBy(ur => ur.CreateDate).ToListAsync();

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
