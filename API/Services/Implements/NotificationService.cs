using API.Data;
using API.Entities;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Account> _userManager;

        public NotificationService(BuildingContext context, IMapper mapper, UserManager<Account> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<Notification> CreateNotification(string title, string content, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var notification = new Notification
            {
                Content = content,
                Title = title,
                //Seen = false
            };
            return notification;
        }

        public async Task<ListDataResponse<NotificationResponse>> GetAllNotifyOfAccount(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.Id == userId);
            List<Notification> notifications = user.Notifications.OrderByDescending(n => n.CreateDate).ToList();
            var response = _mapper.Map<List<NotificationResponse>>(notifications);
            return new ListDataResponse<NotificationResponse>
            {
                IsSuccess = true,
                Datas = response
            };
        }

        public async Task<BaseResponse> PostNotificatonForAccount(int adminId, CreateNotificationRequest request, int? userId)
        {
            var admin = await _context.Users.Include(u => u.Notifications).FirstOrDefaultAsync(u => u.Id == adminId);
            var notification = _mapper.Map<Notification>(request);

            notification.CreateBy = admin.UserName;
            if (userId == null)
            {
                var accounts = await _context.Users.ToListAsync();
                foreach (var account in accounts)
                {
                    var roles = await _userManager.GetRolesAsync(account);
                    if (roles.Contains("Member"))
                    {
                        account.Notifications.Add(notification);
                    }
                }
                if (await _context.SaveChangesAsync() <= 0)
                    return new BaseResponse { IsSuccess = false, Message = "Gửi thông báo thất bại" };
            }
            else
            {
                var account = await _context.Users.Include(u => u.Notifications).FirstOrDefaultAsync(u => u.Id == userId);
                account.Notifications.Add(notification);
                if (await _context.SaveChangesAsync() <= 0)
                    return new BaseResponse { IsSuccess = false, Message = "Gửi thông báo thất bại" };
            }
            return new BaseResponse { IsSuccess = true, Message = "Gửi thông báo thành công" };
        }
    }
}
