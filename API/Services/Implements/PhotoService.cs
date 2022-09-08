using API.Data;
using API.Payloads.Response.BaseResponses;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Implements
{
    public class PhotoService : IPhotoService
    {
        private readonly BuildingContext _context;

        public PhotoService(BuildingContext context)
        {
            _context = context;
        }

        public async Task<ListDataResponse<string>> GetPhotoUrlsByItemId(int itemId)
        {
            List<string> photoUrls = new List<string>();
            var phottos = await _context.ItemPhotos.Where(p => p.ItemId == itemId).ToListAsync();
            foreach (var photo in phottos)
            {
                photoUrls.Add(photo.Url);
            }
            return new ListDataResponse<string>
            {
                IsSuccess = true,
                Datas = photoUrls
            };
        }

        public async Task<ListDataResponse<string>> GetReportPhotoUrlsByReportId(int reportId)
        {
            List<string> photoUrls = new List<string>();
            var phottos = await _context.ReportPhotos.Where(p => p.ReportId == reportId).ToListAsync();
            foreach (var photo in phottos)
            {
                photoUrls.Add(photo.Url);
            }
            return new ListDataResponse<string>
            {
                IsSuccess = true,
                Datas = photoUrls
            };
        }
    }
}
