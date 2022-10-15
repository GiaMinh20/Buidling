using API.Entities;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace API.Data
{
    public class InitData
    {
        public static async Task SeedUsersAsync(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var config = serviceScope.ServiceProvider.GetRequiredService<IConfiguration>();
                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Account>>();
                if (!userManager.Users.Any())
                {
                    var user = new Account
                    {
                        UserName = "minh",
                        Email = "nngiaminh@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        AvatarUrl = "https://res.cloudinary.com/minh20/image/upload/v1656074408/VisitorHouse/default_avatar_m5uoke.png"
                    };
                    await userManager.CreateAsync(user, "Giaminh2001@");
                    await userManager.AddToRoleAsync(user, "Member");
                    for (int i = 1; i < 6; i++)
                    {
                        string adminEmail = config[$"Admin0{i}:Email"];
                        var adminUser = await userManager.FindByEmailAsync(adminEmail);
                        if (adminUser == null)
                        {
                            var newAdminUser = new Account()
                            {
                                UserName = config[$"Admin0{i}:Username"],
                                Email = adminEmail,
                                EmailConfirmed = true,
                                PhoneNumberConfirmed = true,
                                AvatarUrl = config[$"Admin0{i}:AvatarUrl"]
                            };
                            await userManager.CreateAsync(newAdminUser, config[$"Admin0{i}:AvatarUrl"]);
                            await userManager.AddToRoleAsync(newAdminUser, "Admin");
                        }
                    }
                    var context = serviceScope.ServiceProvider.GetService<BuildingContext>();
                    context.Database.EnsureCreated();
                    context.SaveChanges();
                }
            }
        }
    }
}
