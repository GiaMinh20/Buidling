using API.Entities;
using System.Linq;

namespace API.Extensions
{
    public static class BillExtentions
    {
        public static IQueryable<Bill> Paied(this IQueryable<Bill> query, bool? paied)
        {
            if (paied == null) return query;
            query = query.Where(p => p.Paied == paied);
            return query;
        }

        public static IQueryable<Bill> Account(this IQueryable<Bill> query, int? accountId)
        {
            if (accountId == null) return query;
            query = query.Where(p => p.AccountId == accountId);
            return query;
        }

        public static IQueryable<Bill> Item(this IQueryable<Bill> query, int? itemId)
        {
            if (itemId == null) return query;
            query = query.Where(p => p.ItemId == itemId);
            return query;
        }

        public static IQueryable<Bill> Bill(this IQueryable<Bill> query, int? billId)
        {
            if (billId == null) return query;
            query = query.Where(p => p.Id == billId);
            return query;
        }
    }
}
