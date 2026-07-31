using ApiCoVanHocTap.Common;
using ApiCoVanHocTap.Services.Base;
using Models;

namespace ApiCoVanHocTap.Services.PhanCongCoVan
{
    public class PhanCongCoVanService : BaseService, IPhanCongCoVanService
    {
        public PhanCongCoVanService(IServiceProvider serviceProvider)
            : base(serviceProvider) { }

        public async Task<Response> GetList(string? id_cb, string? id_sv)
        {
            Response response = new Response();
            try
            {
                var data = await _repositoryWrapper.PhanCongCoVan.GetList(id_cb, id_sv);
                response.data = data;
            }
            catch (Exception ex)
            {
                ex.ErrorSysResponse(response);
            }
            return response;
        }
    }
}