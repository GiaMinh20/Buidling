using API.Entities;
using API.Helpers;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IBillService
    {
        Bill createBill(int userId, int itemId, string title, int itemPrice, int electricPrice, int waterPrice, int vehiclePrice, int otherPrice);
        Task<DataResponse<PagedList<BillForAdminResponse>>> GetBillsByAdmin(BillForAdminParam param);
    }
}
