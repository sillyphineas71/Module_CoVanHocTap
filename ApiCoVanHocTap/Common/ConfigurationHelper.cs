using ApiCoVanHocTap.Repositories;
using ApiCoVanHocTap.Repositories.Base;
using ApiCoVanHocTap.Services;

namespace ApiCoVanHocTap.Common
{
    public static class ConfigurationHelper
    {
        /// <summary>Đăng ký tầng Repository + connection.</summary>
        public static void RepositorysConfig(IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddSingleton<IDbConnectionQuerry, DbConnectionQuerry>();
        }

        /// <summary>Đăng ký tầng Service.</summary>
        public static void ServicesConfig(IServiceCollection services)
        {
            services.AddScoped<IServiceWrapper, ServiceWrapper>();
            services.AddScoped<ICoreServiceWrapper, CoreServiceWrapper>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
        }
    }
}