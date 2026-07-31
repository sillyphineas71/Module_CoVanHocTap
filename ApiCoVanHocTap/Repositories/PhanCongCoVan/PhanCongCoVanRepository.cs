using ApiCoVanHocTap.Repositories.Base;
using Dapper;
using Models.Table;

namespace ApiCoVanHocTap.Repositories.PhanCongCoVan
{
    public class PhanCongCoVanRepository : BaseRepository, IPhanCongCoVanRepository
    {
        public PhanCongCoVanRepository(IDbConnectionQuerry dbConnection)
            : base(dbConnection) { }

        public async Task<IEnumerable<Models.Table.PhanCongCoVan>> GetList(string? id_cb, string? id_sv)
        {
            var param = new DynamicParameters();
            param.Add("@id_cb", id_cb);
            param.Add("@id_sv", id_sv);

            return await _dbConnection.SelectAsync<Models.Table.PhanCongCoVan>(
                "TB_PhanCongCoVan_GetList", param);
        }
    }
}