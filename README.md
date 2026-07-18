# ApiCoVanHocTap

Microservice **Cố vấn học tập (Academic Advisor)** — kênh trao đổi 2 chiều giữa cố vấn học
tập và sinh viên (chat real-time) và gửi thông báo, nằm trong hệ thống quản trị đào tạo.
Đây là use case đầu tiên; định hướng dài hạn là nền tảng quản lý thông báo tổng quát.

## Công nghệ

| Thành phần | Dùng |
|---|---|
| Backend | .NET (ASP.NET Core Web API) |
| Truy cập dữ liệu | Dapper + Stored Procedure |
| Database | SQL Server (DB chung `ESS_HOCVIENTAICHINH_DAOTAO`, bảng tiền tố `TB_`) |
| Real-time | SignalR |
| Log | Serilog |
| Frontend (repo khác) | React + TypeScript |

## Kiến trúc

Phân tầng **Controller → Service → Repository**, dùng pattern Wrapper + Service Locator +
Lazy Loading (quy ước chung của hệ thống).

```
Request → Middleware → Controller → Service (logic + kiểm quyền) → Repository → Stored Procedure → SQL Server
Chat real-time đi qua SignalR Hub (Hubs/ChatHub.cs).
```

> 📘 **Chi tiết toàn bộ khung code, từng folder/file và cách thêm chức năng mới:
> xem [HUONG_DAN_KIEN_TRUC.md](HUONG_DAN_KIEN_TRUC.md).**

## Cấu trúc thư mục

```
ApiCoVanHocTap/          # Project Web API
  Controllers/           # Tầng API (BaseController + controller nghiệp vụ)
  Services/              # Tầng nghiệp vụ (IServiceWrapper/ServiceWrapper, Core/)
  Repositories/          # Tầng truy cập dữ liệu (IRepositoryWrapper/RepositoryWrapper)
  Hubs/                  # SignalR (ChatHub, BaseHub)
  Filters/               # Xác thực + điều khiển ghi log
  Middleware/            # Bắt lỗi, ghi log, cấu hình Serilog
  Common/                # Tiện ích dùng chung (Connection/Dapper, AppSettings...)
  Assets/                # File tĩnh (Template, Upload)
Models/                  # Class Library: DTO, Enum, kết quả trả về
```

## Chạy ở máy local

Yêu cầu: **.NET SDK** (xem `TargetFramework` trong `.csproj`) và **SQL Server**.

```bash
# 1. Cấu hình chuỗi kết nối DB trong ApiCoVanHocTap/appsettings.json (mục ConnectionStrings)
# 2. Build & chạy
dotnet build CoVanHocTap.slnx
dotnet run --project ApiCoVanHocTap
```

Mặc định chạy tại `http://localhost:5041`. Mở Swagger: `http://localhost:5041/swagger`.

## Cấu hình & bảo mật

- Cấu hình trong `appsettings.json`: `ConnectionStrings`, `Jwt`, `ApiSettings:ApiKey`, `CorsWithOrigins`, `EnableRequestLog`.
- **Không commit mật khẩu/secret thật** vào `appsettings.json`. Dùng `dotnet user-secrets`
  hoặc biến môi trường; các giá trị `CHANGE_ME_...` là placeholder cần thay khi deploy.

## Trạng thái

Scaffold (bộ khung) đã sẵn sàng để phát triển nghiệp vụ. Các phần cần hoàn thiện khi vào
tính năng được liệt kê ở cuối [HUONG_DAN_KIEN_TRUC.md](HUONG_DAN_KIEN_TRUC.md) (mục 9).
