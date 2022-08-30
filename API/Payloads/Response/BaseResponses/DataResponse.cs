namespace API.Payloads.Response.BaseResponses
{
    public class DataResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
