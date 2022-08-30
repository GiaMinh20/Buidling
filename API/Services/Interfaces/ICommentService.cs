using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface ICommentService
    {
        public Task<List<CommentResponse>> GetCommentsByItemId(int itemId);
        public Task<BaseResponse> CommentItem(int userId, int itemId, string content);
    }
}
