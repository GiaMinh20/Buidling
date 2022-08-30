using System.Collections.Generic;

namespace API.Payloads.Response.BaseResponses
{
    public class ListDataResponse<T> : BaseResponse
    {
        public List<T> Datas { get; set; }
    }
}
