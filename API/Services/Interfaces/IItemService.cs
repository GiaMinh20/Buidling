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
        Task<DataResponse<ItemResponse>> EditItem(int id, EditItemRequest request);
        Task<BaseResponse> DeleteItem(int id);
        Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForSystem(ItemForSystemParams itemParams);
        Task<DataResponse<ItemDetailForSystemResponse>> GetItem(int id);
        Task<DataResponse<PagedList<ItemForSystemResponse>>> GetItemsForAccount(int userId, ItemForSystemParams itemParams);
        Task<BaseResponse> AssignUserForItem(int userId, int itemId);
        Task<DataResponse<PagedList<ItemUnpaiedResponse>>> GetUnpaiedItems(PaginationParams itemParams);
        Task<DataResponse<PagedList<ItemForAdminResponse>>> GetItemsForAdmin(ItemForAdminParams itemParams);
        Task<DataResponse<ItemDetailForAdminResponse>> GetItemDetailForAdmin(int id);
        Task<BaseResponse> UnAssignUserForItem(int userId, int itemId, int adminId);
        Task<BaseResponse> SendMonthlyBillForUser(int itemId, CreateMonthlyBillRequest request);
    }
}
