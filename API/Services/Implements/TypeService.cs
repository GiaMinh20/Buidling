using API.Data;
using API.Entities;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class TypeService : ITypeService
    {
        private readonly BuildingContext _context;
        public TypeService(BuildingContext context)
        {
            _context = context;
        }
        public async Task<DataResponse<TypeItem>> CreateType(CreateTypeRequest request)
        {
            var alreadyInDB = await _context.TypeItems.FirstOrDefaultAsync(t => t.Name == request.Name);
            if (alreadyInDB != null)
                return new DataResponse<TypeItem>
                {
                    IsSuccess = false,
                    Message = "Danh mục đã tồn tại"
                };
            var type = new TypeItem { Name = request.Name };
            _context.TypeItems.Add(type);
            var rs = await _context.SaveChangesAsync() > 0;
            if (rs)
            {
                return new DataResponse<TypeItem>
                {
                    IsSuccess = true,
                    Message = "Tạo danh mục thành công",
                    Data = type
                };
            }
            else
            {
                return new DataResponse<TypeItem>
                {
                    IsSuccess = false,
                    Message = "Tạo danh mục thất bại"
                };
            }
        }

        public async Task<ListDataResponse<TypeItem>> GetTypes()
        {
            var itemTypes = await _context.TypeItems.ToListAsync();
            if (itemTypes != null)
            {
                return new ListDataResponse<TypeItem>
                {
                    IsSuccess = true,
                    Datas = itemTypes
                };
            }
            else
            {
                return new ListDataResponse<TypeItem>
                {
                    IsSuccess = false,
                    Message = "Có lỗi xảy ra khi lấy dữ liệu"
                };
            }
        }

        public async Task<DataResponse<TypeItem>> GetTypeById(int id)
        {
            var type = await _context.TypeItems.FindAsync(id);
            if (type != null)
            {
                return new DataResponse<TypeItem>
                {
                    IsSuccess = true,
                    Data = type
                };
            }
            else
            {
                return new DataResponse<TypeItem>
                {
                    IsSuccess = false,
                    Message = $"Không tìm thấy danh mục có id: {id}"
                };
            }
        }
    }
}
