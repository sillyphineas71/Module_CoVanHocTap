// =============================================================================
// FILE: RepositoryWrapper.cs  (cài đặt cụ thể của IRepositoryWrapper)
// -----------------------------------------------------------------------------
// Cùng pattern Hybrid DI + Lazy Loading (??=) như ServiceWrapper.cs, nhưng cho
// tầng Repository. Do DI tạo (Scoped ở Program.cs); các Repository bên trong tạo
// thủ công bằng `new` khi được gọi lần đầu.
// =============================================================================
using System.Data;
using ApiCoVanHocTap.Repositories.Base;
using ApiCoVanHocTap.Repositories.PhanCongCoVan;

namespace ApiCoVanHocTap.Repositories
{
    /// <summary>
    /// Concrete implementation của <see cref="IRepositoryWrapper"/>.
    /// </summary>
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        private IPhanCongCoVanRepository? _phanCongCoVan;


        // =========================================================================
        // Backing fields – khởi tạo lazy khi property được truy cập lần đầu
        // =========================================================================
        // private IHoiThoaiRepository? _hoiThoaiRepository;

        public RepositoryWrapper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // =========================================================================
        // Public properties – Lazy Loading với ??=. Ví dụ:
        // -------------------------------------------------------------------------
        // public IHoiThoaiRepository HoiThoai =>
        //     _hoiThoaiRepository ??= new HoiThoaiRepository(_serviceProvider);
        // =========================================================================
        public IPhanCongCoVanRepository PhanCongCoVan =>
        _phanCongCoVan ??= new PhanCongCoVanRepository(
            _serviceProvider.GetRequiredService<IDbConnectionQuerry>());
    }
}
