using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Extensions
{
    public static class AccountExtentions
    {

        public static IQueryable<Account> Username(this IQueryable<Account> query, string username)
        {
            if (string.IsNullOrEmpty(username)) return query;
            var lowerCaseSearchTerm = username.Trim().ToLower();
            return query.Where(p => p.UserName.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Account> Email(this IQueryable<Account> query, string email)
        {
            if (string.IsNullOrEmpty(email)) return query;
            var lowerCaseSearchTerm = email.Trim().ToLower();
            return query.Where(p => p.Email.ToLower().Contains(lowerCaseSearchTerm) && p.EmailConfirmed == true);
        }

    }
}
