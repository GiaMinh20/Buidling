using API.Data;
using API.Entities;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class FavoriteService : IFavoriteService
    {
        private readonly BuildingContext _context;
        private readonly IMapper _mapper;

        public FavoriteService(BuildingContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<BaseResponse> AddItemToFavorites(int itemId, int accountId)
        {
            var favourite = await _context.Favorites
                .Include(f => f.Items)
                .FirstOrDefaultAsync(f => f.AccountId == accountId);
            var item = await _context.Items.FindAsync(itemId);
            if (favourite == null)
            {
                List<Item> items = new List<Item>();
                items.Add(item);
                _context.Favorites.Add(new Favorite { AccountId = accountId, Items = items });
            }
            else
            {
                favourite.Items.Add(item);
            }
            if (await _context.SaveChangesAsync() > 0)
            {
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "Lưu thành công"
                };
            }
            return new BaseResponse
            {
                IsSuccess = false,
                Message = "Lưu thất bại"
            };
        }

        public async Task<ListDataResponse<ItemForFavoriteResponse>> GetFavoritesByUserId(int accountId)
        {
            var favorite = await _context.Favorites
                .Include(f => f.Items)
                .ThenInclude(i => i.Type)
                .Where(f => f.AccountId == accountId)
                .FirstOrDefaultAsync();

            return new ListDataResponse<ItemForFavoriteResponse>
            {
                IsSuccess = true,
                Datas = _mapper.Map<List<ItemForFavoriteResponse>>(favorite.Items.ToList())
            };
        }
    }
}
