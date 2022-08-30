using API.Entities;
using API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace API.Extensions
{
    public static class ItemExtensions
    {
        public static IQueryable<Item> Sort(this IQueryable<Item> query, string orderBy)
        {
            if (string.IsNullOrEmpty(orderBy)) return query.OrderByDescending(p => p.Id);

            query = orderBy switch
            {
                "price" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),

                _ => query.OrderByDescending(p => p.Id)
            };

            return query;
        }

        public static IQueryable<Item> Search(this IQueryable<Item> query, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm)
            || p.Type.Name.ToLower().Contains(lowerCaseSearchTerm)
            || p.Location.ToLower().Contains(lowerCaseSearchTerm)
            || p.Description.ToLower().Contains(lowerCaseSearchTerm));
        }

        public static IQueryable<Item> Filter(this IQueryable<Item> query, int? type)
        {
            if (type != null)
                query = query.Where(i => i.Type.Id == type);
            return query;
        }

        public static IQueryable<Item> Status(this IQueryable<Item> query, EItemStatus? status)
        {
            if (status == null) return query;
            query = query.Where(p => p.Status == status);
            return query;
        }

        private static string utf8Convert3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}
