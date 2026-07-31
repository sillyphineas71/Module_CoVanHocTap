// =============================================================================
// FILE: ServiceWrapper.cs  (cài đặt cụ thể của IServiceWrapper)
// -----------------------------------------------------------------------------
// Pattern "Hybrid DI":
//   - Bản thân ServiceWrapper do DI container tạo (đăng ký Scoped ở Program.cs).
//   - Các service nghiệp vụ bên trong KHÔNG inject qua constructor, mà tạo thủ
//     công bằng `new ...(_serviceProvider)` + Lazy Loading (toán tử ??=): chỉ tạo
//     khi property được gọi lần đầu, các lần sau tái dùng instance đã tạo.
//   - _serviceProvider được truyền xuống để service tự resolve dependency khi cần.
//
// Vì sao dùng cách này: Controller/Service chỉ phụ thuộc vào 1 wrapper, thêm
// service mới không phải sửa constructor ở nhiều nơi. (Đây là quy ước team.)
// =============================================================================
using ApiCoVanHocTap.Repositories.PhanCongCoVan;
using ApiCoVanHocTap.Services.PhanCongCoVan;

namespace ApiCoVanHocTap.Services
{
    /// <summary>
    /// Concrete implementation của <see cref="IServiceWrapper"/>.
    /// </summary>
    public class ServiceWrapper : IServiceWrapper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IPhanCongCoVanService _phanCongCoVan;

        // =========================================================================
        // Backing fields – khởi tạo lazy khi property được truy cập lần đầu
        // =========================================================================
        // private IHoiThoaiService? _hoiThoaiService;

        public ServiceWrapper(
            IServiceProvider serviceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        public IHttpContextAccessor HttpContextAccessor => _httpContextAccessor;

        // =========================================================================
        // Public properties – Lazy Loading với ??= (null-coalescing assignment).
        // Ví dụ thêm service mới:
        // -------------------------------------------------------------------------
        // public IHoiThoaiService HoiThoai =>
        //     _hoiThoaiService ??= new HoiThoaiService(_serviceProvider);
        // =========================================================================
        public IPhanCongCoVanService PhanCongCoVan =>
                _phanCongCoVan ??= new PhanCongCoVanService(_serviceProvider);
    }
}
