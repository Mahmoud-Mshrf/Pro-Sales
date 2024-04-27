using CRM.Core.Services.Implementations;
using CRM.Core.Services.Interfaces;

namespace CRM.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped<IAuthService, AuthService>();// Add IAuthService to the container
            services.AddScoped<IMailingService, MailingService>();// Add IMailingService to the container
            services.AddScoped<IUserProfileService, UserProfileService>();// Add IUserProfileService to the container
            services.AddScoped<IModeratorService, ModeratorService>(); // Add IModeratorService to the container
            services.AddScoped<ISalesRepresntative, SalesService>();
            services.AddScoped<ISharedService, SharedService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IFilterService, FilterService>();
            services.AddScoped<IReportingService, ReportingService>();

            return services;
        }


    }
}
