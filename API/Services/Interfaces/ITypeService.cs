using API.Entities;
using API.Payloads.Requests;
using API.Payloads.Response.BaseResponses;
using System;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface ITypeService
    {
        Task<ListDataResponse<TypeItem>> GetTypes();
        Task<DataResponse<TypeItem>> GetTypeById(int id);
        Task<DataResponse<TypeItem>> CreateType(CreateTypeRequest request);
    }
}
