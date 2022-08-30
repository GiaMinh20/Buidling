using API.Entities;
using System.Collections.Generic;

namespace API.Payloads.Response
{
    public class StatisticDetailsResponse : StatisticResponse
    {
        public int TotalAmount { get; set; }

    }
}
