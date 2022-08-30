using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<Account> userManager,
            RoleManager<Role> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<Account>>(userData);
            if (users == null) return;

            var roles = new List<Role>
            {
                new Role{Name = "Member"},
                new Role{Name = "Admin"},
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            var admin = new Account
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin" });
        }

        public static async Task Initialize(BuildingContext context, UserManager<Account> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new Account
                {
                    UserName = "minh",
                    Email = "nngiaminh@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                await userManager.CreateAsync(user, "Giaminh2001@");
                await userManager.AddToRoleAsync(user, "Member");
                context.Users.Add(user);
                var admin01 = new Account
                {
                    UserName = "admin02",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    AvatarUrl = "https://res.cloudinary.com/minh20/image/upload/v1656074408/VisitorHouse/default_avatar_m5uoke.png"
                };
                await userManager.CreateAsync(admin01, "Giaminh2001@");
                await userManager.AddToRolesAsync(admin01, new[] { "Admin" });
                context.Users.Add(admin01);
                var admin02 = new Account
                {
                    UserName = "admin02",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    AvatarUrl = "https://res.cloudinary.com/minh20/image/upload/v1656074408/VisitorHouse/default_avatar_m5uoke.png"
                };
                await userManager.CreateAsync(admin02, "Giaminh2001@");
                await userManager.AddToRolesAsync(admin02, new[] { "Admin" });
                context.Users.Add(admin02);
                context.SaveChanges();
                //var admin01 = new Account
                //{
                //    UserName = config["Admin:Admin01:Username"],
                //    Email = config["Admin:Admin01:Email"],
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    AvatarUrl = config["Admin:Admin01:AvatarUrl"]
                //};
                //await userManager.CreateAsync(admin01, config["Admin:Admin01:Password"]);
                //await userManager.AddToRolesAsync(admin01, new[] { "Admin" });


                //var admin02 = new Account
                //{
                //    UserName = config["Admin:Admin02:Username"],
                //    Email = config["Admin:Admin02:Email"],
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    AvatarUrl = config["Admin:Admin02:AvatarUrl"]
                //};
                //await userManager.CreateAsync(admin02, config["Admin:Admin02:Password"]);
                //await userManager.AddToRolesAsync(admin02, new[] { "Admin" });

                //var admin03 = new Account
                //{
                //    UserName = config["Admin:Admin03:Username"],
                //    Email = config["Admin:Admin03:Email"],
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    AvatarUrl = config["Admin:Admin03:AvatarUrl"]
                //};
                //await userManager.CreateAsync(admin03, config["Admin:Admin03:Password"]);
                //await userManager.AddToRolesAsync(admin03, new[] { "Admin" });

                //var admin04 = new Account
                //{
                //    UserName = config["Admin:Admin04:Username"],
                //    Email = config["Admin:Admin04:Email"],
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    AvatarUrl = config["Admin:Admin04:AvatarUrl"]
                //};
                //await userManager.CreateAsync(admin04, config["Admin:Admin04:Password"]);
                //await userManager.AddToRolesAsync(admin04, new[] { "Admin" });

                //var admin05 = new Account
                //{
                //    UserName = config["Admin:Admin05:Username"],
                //    Email = config["Admin:Admin05:Email"],
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    AvatarUrl = config["Admin:Admin05:AvatarUrl"]
                //};
                //await userManager.CreateAsync(admin05, config["Admin:Admin05:Password"]);
                //await userManager.AddToRolesAsync(admin05, new[] { "Admin" });

            }
        }
    }
}