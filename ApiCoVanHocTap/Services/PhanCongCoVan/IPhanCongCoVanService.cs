using Models;

namespace ApiCoVanHocTap.Services.PhanCongCoVan
{
    public interface IPhanCongCoVanService
    {
        Task<Response> GetList(string? id_cb, string? id_sv);
    }
}