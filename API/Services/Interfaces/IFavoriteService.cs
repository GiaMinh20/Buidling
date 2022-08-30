using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<BaseResponse> AddItemToFavorites(int itemId, int accountId);
        Task<ListDataResponse<ItemForFavoriteResponse>> GetFavoritesByUserId(int accountId);
    }
}
