// =============================================================================
// FILE: IRepositoryWrapper.cs  (tầng Repository — cửa vào duy nhất cho data access)
// -----------------------------------------------------------------------------
// Giống IServiceWrapper nhưng dành cho tầng truy cập dữ liệu. Service gọi
// RepositoryWrapper.HoiThoai.GetById(...) thay vì tự viết SQL rải rác.
// Mỗi Repository bên trong gọi Stored Procedure qua helper Common/Connection.cs.
//
// Khi thêm 1 repository mới: khai báo property ở ĐÂY và cài đặt lazy bên
// RepositoryWrapper.cs.
// =============================================================================
using ApiCoVanHocTap.Repositories.PhanCongCoVan;

namespace ApiCoVanHocTap.Repositories
{
    /// <summary>
    /// Wrapper gom toàn bộ Repository của module (tầng truy cập dữ liệu).
    /// </summary>
    public interface IRepositoryWrapper
    {
        // =========================================================================
        // Khai báo repository theo từng bảng/nhóm nghiệp vụ. Ví dụ:
        // -------------------------------------------------------------------------
        // IHoiThoaiRepository HoiThoai { get; }
        // ITinNhanRepository TinNhan { get; }
        // IPhanCongCoVanRepository PhanCongCoVan { get; }
        // IThongBaoRepository ThongBao { get; }
        // =========================================================================
        IPhanCongCoVanRepository PhanCongCoVan { get; }
    }
}
