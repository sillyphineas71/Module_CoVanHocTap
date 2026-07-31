using ApiCoVanHocTap.Repositories;   // ICoVanHocTapRepositoryWrapper
using ApiCoVanHocTap.Services;        // ICoVanHocTapServiceWrapper, IJwtTokenService
using Models.Base;                    // ModifyInfo

namespace ApiCoVanHocTap.Services.Base
{
    public class BaseService : IBaseService
    {
        protected IServiceProvider _serviceProvider;
        protected IServiceWrapper _serviceWrapper;
        protected IRepositoryWrapper _repositoryWrapper;
        private IJwtTokenService _jwtTokenService;

        public BaseService(IServiceProvider serviceProvider)
        {
            // KHÁC ApiMarkMan: KHÔNG dùng serviceProvider.CreateScope().
            // Bản gốc tạo scope mới không bao giờ dispose → hỏng lifetime scoped + rò rỉ.
            // Dùng thẳng serviceProvider được inject.
            this._serviceProvider = serviceProvider;
            this._serviceWrapper = _serviceProvider.GetRequiredService<IServiceWrapper>();
            this._repositoryWrapper = _serviceProvider.GetRequiredService<IRepositoryWrapper>();
            this._jwtTokenService = _serviceProvider.GetRequiredService<IJwtTokenService>();
        }

        public void SetModifyUser(ModifyInfo info)
        {
            try
            {
                var context = _serviceWrapper.HttpContextAccessor?.HttpContext;
                if (context == null) return;
                _jwtTokenService.SetModifyInfo(context, info);
            }
            catch { }
        }

        public string GetUserName()
        {
            try
            {
                var context = _serviceWrapper.HttpContextAccessor?.HttpContext;
                if (context == null) return "";
                return _jwtTokenService.GetUserName(context);
            }
            catch { }
            return "";
        }

        public string GetUserID()
        {
            try
            {
                var context = _serviceWrapper.HttpContextAccessor?.HttpContext;
                if (context == null) return "";
                return _jwtTokenService.GetUserID(context);
            }
            catch { }
            return "";
        }

        public string GetIP()
        {
            try
            {
                var context = _serviceWrapper.HttpContextAccessor?.HttpContext;
                if (context == null) return "";
                return _jwtTokenService.GetIP(context);
            }
            catch { }
            return "";
        }
    }
}