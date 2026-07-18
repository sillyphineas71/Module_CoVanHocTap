// =============================================================================
// FILE: RepositoryWrapper.cs  (cài đặt cụ thể của IRepositoryWrapper)
// -----------------------------------------------------------------------------
// Cùng pattern Hybrid DI + Lazy Loading (??=) như ServiceWrapper.cs, nhưng cho
// tầng Repository. Do DI tạo (Scoped ở Program.cs); các Repository bên trong tạo
// thủ công bằng `new` khi được gọi lần đầu.
// =============================================================================
namespace ApiCoVanHocTap.Repositories
{
    /// <summary>
    /// Concrete implementation của <see cref="IRepositoryWrapper"/>.
    /// </summary>
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly IServiceProvider _serviceProvider;

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
    }
}
