using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<DataResponse<PaymentResponse>> PayRentMoney(int userId, PayRentMoney payRentMoney);
    }
}
