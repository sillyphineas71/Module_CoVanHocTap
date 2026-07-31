using ApiCoVanHocTap.Repositories.Base;
using Models.Table;

namespace ApiCoVanHocTap.Repositories.PhanCongCoVan
{
    public interface IPhanCongCoVanRepository : IBaseRepository
    {
        Task<IEnumerable<Models.Table.PhanCongCoVan>> GetList(string? id_cb, string? id_sv);
    }
}