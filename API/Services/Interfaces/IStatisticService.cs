using API.Payloads.Response;
using API.Payloads.Response.BaseResponses;
using System;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IStatisticService
    {
        Task<DataResponse<StatisticResponse>> GetStatisticOfBuilding();
        Task<DataResponse<StatisticResponse>> GetStatictisByTime(DateTime from, DateTime to);
    }
}
