using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Extensions
{
    public static class MemberExtentions
    {

        public static IQueryable<Member> AccountName(this IQueryable<Member> query, string accountName)
        {
            if (string.IsNullOrEmpty(accountName)) return query;
            var lowerCaseSearchTerm = accountName.Trim().ToLower();
            return query.Where(p => p.Account.UserName.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Member> Gender(this IQueryable<Member> query, string gender)
        {
            if (string.IsNullOrEmpty(gender)) return query;
            var lowerCaseSearchTerm = gender.Trim().ToLower();
            return query.Where(p => p.Gender.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Member> CCCD(this IQueryable<Member> query, string cccd)
        {
            if (string.IsNullOrEmpty(cccd)) return query;
            var lowerCaseSearchTerm = cccd.Trim().ToLower();
            return query.Where(p => p.CCCD.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Member> FullName(this IQueryable<Member> query, string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return query;
            var lowerCaseSearchTerm = fullName.Trim().ToLower();
            return query.Where(p => p.FullName.ToLower().Contains(lowerCaseSearchTerm));
        }
    }
}
