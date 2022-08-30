using API.Data;
using API.Helpers;
using API.Services.Implements;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
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
