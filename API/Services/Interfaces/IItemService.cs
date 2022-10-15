using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IItemService
    {
        Task<BaseResponse> CreateItem(CreateItemRequest request);
        Task<DataResponse<ItemResponse>> EditItem(EditItemRequest request);
        Task<BaseResponse> DeleteItem(int id);
        Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForSystem(ItemForSystemParams itemParams);
        Task<DataResponse<ItemDetailForSystemResponse>> GetItem(int id);
        Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForAccount(int userId, ItemForSystemParams itemParams);
        Task<BaseResponse> AssignUserForItem(AssignUserForItemRequest request);
        Task<DataResponse<PagedList<ItemUnpaiedResponse>>> GetUnpaiedItems(PaginationParams itemParams);
        Task<DataResponse<PagedList<ItemForAdminResponse>>> GetItemsForAdmin(ItemForAdminParams itemParams);
        Task<DataResponse<ItemDetailForAdminResponse>> GetItemDetailForAdmin(int id);
        Task<BaseResponse> UnAssignUserForItem(int userId, int itemId, int adminId, bool hasRequest);
        Task<DataResponse<byte[]>> SendMonthlyBillForUser(int adminId, int itemId, CreateMonthlyBillRequest request);
    }
}
