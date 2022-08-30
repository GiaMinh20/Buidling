using API.Entities;
using System.Linq;

namespace API.Extensions
{
    public static class VehicleExtentions
    {
        public static IQueryable<Vehicle> AccountName(this IQueryable<Vehicle> query, string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) return query;
            var lowerCaseSearchTerm = accountName.Trim().ToLower();
            return query.Where(p => p.AccountName.ToLower().Contains(lowerCaseSearchTerm));
        }
        public static IQueryable<Vehicle> LicensePlates(this IQueryable<Vehicle> query, string licensePlates)
        {
            if (string.IsNullOrEmpty(licensePlates)) return query;
            var lowerCaseSearchTerm = licensePlates.Trim().ToLower();
            return query.Where(p => p.LicensePlates.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Vehicle> Transportation(this IQueryable<Vehicle> query, string transportation)
        {
            if (string.IsNullOrEmpty(transportation)) return query;
            var lowerCaseSearchTerm = transportation.Trim().ToLower();
            return query.Where(p => p.Transportation.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Vehicle> Status(this IQueryable<Vehicle> query, bool? status)
        {
            if (status == null) return query;
            query = query.Where(p => p.Status == status);
            return query;
        }
    }
}
