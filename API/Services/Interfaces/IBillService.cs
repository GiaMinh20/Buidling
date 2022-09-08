using API.Entities;
using API.Helpers;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IBillService
    {
        Bill createBill(int userId, int itemId, string title, int itemPrice, int electricPrice, string electricBillUrl, int waterPrice, string waterBillUrl, int vehiclePrice, int otherPrice);
        Task<DataResponse<PagedList<BillResponse>>> GetBillsByAdmin(BillForAdminParam param);
        Task<DataResponse<PagedList<BillResponse>>> GetBillsByUser(int userId, BillForAccountParams param);
    }
}
