using API.Payloads.Response.BaseResponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IPhotoService
    {
        public Task<ListDataResponse<string>> GetPhotoUrlsByItemId(int itemId);
        public Task<ListDataResponse<string>> GetReportPhotoUrlsByReportId(int reportId);
    }
}
