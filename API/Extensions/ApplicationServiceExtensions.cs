using API.Data;
using API.Helpers;
using API.Services.Implements;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddDbContext<BuildingContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                if (env == "Development")
                {
                    // Use connection string from file.
                    //connStr = config.GetConnectionString("DefaultConnection");
                    //var connUrl = config.GetConnectionString("DefaultConnection");

                    //connUrl = connUrl.Replace("postgres://", string.Empty);
                    //var pgUserPass = connUrl.Split("@")[0];
                    //var pgHostPortDb = connUrl.Split("@")[1];
                    //var pgHostPort = pgHostPortDb.Split("/")[0];
                    //var pgDb = pgHostPortDb.Split("/")[1];
                    //var pgUser = pgUserPass.Split(":")[0];
                    //var pgPass = pgUserPass.Split(":")[1];
                    //var pgHost = pgHostPort.Split(":")[0];
                    //var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server=ec2-44-210-36-247.compute-1.amazonaws.com;Port=5432;User Id=bihbecwnmyyqce;Password=32f1b027b634fb4a638ff905723220418029dbd10be6a09ef429328ea3a1f2bc;Database=d1eipdobcleqf8;SSL Mode=Require;Trust Server Certificate=true";
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};SSL Mode=Require;Trust Server Certificate=true";
                }
                options.UseNpgsql(connStr);
                //options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });

            services.AddAuthorization();
            services.AddScoped<ITokenService, TokenServiceCustom>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITypeService, TypeService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRentRequestService, RentRequestService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBillService, BillService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IStatisticService, StatisticService>();
            services.AddScoped<IExportDataService, ExportDataService>();
            return services;
        }
    }
}
