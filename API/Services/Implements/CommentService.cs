using API.Data;
using API.Entities;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class CommentService : ICommentService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public CommentService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BaseResponse> CommentItem(int userId, int itemId, string content)
        {
            var account = await _context.Users.FindAsync(userId);
            var item = await _context.Items.FindAsync(itemId);
            var comment = new Comment
            {
                Account = account,
                AccountId = account.Id,
                Item = item,
                ItemId = item.Id,
                Content = content
            };
            _context.Comments.Add(comment);
            if (await _context.SaveChangesAsync() > 0)
            {
                return new BaseResponse { IsSuccess = true, Message = "Bình luận thành công" };
            }
            return new BaseResponse { IsSuccess = false, Message = "Bình luận thất bại" };
        }

        public async Task<List<CommentResponse>> GetCommentsByItemId(int itemId)
        {
            List<CommentResponse> commentResponses = new List<CommentResponse>();
            var comments = await _context.Comments
                .Include(c => c.Account)
                .Where(p => p.ItemId == itemId).ToListAsync();

            foreach (var comment in comments)
            {
                commentResponses.Add(_mapper.Map<CommentResponse>(comment));
            }
            return commentResponses;
        }
    }
}
