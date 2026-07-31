// =============================================================================
// FILE: IServiceWrapper.cs  (tầng Service — cửa vào duy nhất cho toàn bộ service)
// -----------------------------------------------------------------------------
// Đây là "danh mục" tất cả service nghiệp vụ. Controller KHÔNG inject từng service
// riêng lẻ, mà chỉ inject 1 IServiceWrapper rồi gọi ServiceWrapper.HoiThoai,
// ServiceWrapper.ThongBao... (Service Locator Pattern — quy ước của team).
//
// Khi thêm 1 service mới: khai báo property ở ĐÂY (interface) và cài đặt phần
// khởi tạo lazy bên ServiceWrapper.cs.
// =============================================================================
using ApiCoVanHocTap.Services.PhanCongCoVan;

namespace ApiCoVanHocTap.Services
{
    /// <summary>
    /// Wrapper chứa tất cả service nghiệp vụ của hệ thống Cố vấn học tập.
    /// Là entry-point duy nhất để Controller truy cập Services.
    /// </summary>
    public interface IServiceWrapper
    {
        IHttpContextAccessor HttpContextAccessor { get; }

        // =========================================================================
        // Khai báo các service nghiệp vụ theo từng domain. Ví dụ:
        // -------------------------------------------------------------------------
        // IHoiThoaiService HoiThoai { get; }
        // ITinNhanService TinNhan { get; }
        // IPhanCongCoVanService PhanCongCoVan { get; }
        // IThongBaoService ThongBao { get; }
        // =========================================================================
        IPhanCongCoVanService PhanCongCoVan { get; }
    }
}
