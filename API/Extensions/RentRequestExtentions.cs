using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Extensions
{
    public static class RentRequestExtentions
    {

        public static IQueryable<RentRequest> Status(this IQueryable<RentRequest> query, bool? status)
        {
            if (status == null) return query;
            query = query.Where(p => p.status == status);
            return query;
        }

        public static IQueryable<UnRentRequest> Status(this IQueryable<UnRentRequest> query, bool? status)
        {
            if (status == null) return query;
            query = query.Where(p => p.status == status);
            return query;
        }

    }
}
