using API.Entities;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotification(string title, string content, int userId);
        Task<ListDataResponse<Notification>> GetAllNotifyOfAccount(int userId);
    }
}
