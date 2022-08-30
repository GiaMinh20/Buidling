using API.Data;
using API.Entities;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public NotificationService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Notification> CreateNotification(string title, string content, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var notification = new Notification
            {
                Account = user,
                Content = content,
                Title = title,
                //Seen = false
            };
            return notification;
        }

        public async Task<ListDataResponse<Notification>> GetAllNotifyOfAccount(int userId)
        {
            var notifications = await _context.Notifications.Where(n => n.Account.Id == userId).ToListAsync();
            return new ListDataResponse<Notification>
            {
                IsSuccess = true,
                Datas = notifications
            };
        }
    }
}
