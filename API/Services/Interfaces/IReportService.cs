using API.Helpers;
using API.Payloads.Requests;
using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IReportService
    {
        Task<BaseResponse> SendReport(int userId, ReportRequest request);
        Task<DataResponse<PagedList<ReportResponse>>> GetReports(PaginationParams param);
        Task<DataResponse<ReportDetailResponse>> GetReporDetail(int id);
    }
}
