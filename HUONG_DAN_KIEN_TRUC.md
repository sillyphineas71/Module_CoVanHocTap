# Hướng dẫn kiến trúc & boilerplate — ApiCoVanHocTap

> File này giải thích **toàn bộ khung code (boilerplate)** của service để người mới vào
> đọc là hiểu: mỗi folder/file để làm gì, các pattern hoạt động ra sao, và cách thêm
> một chức năng mới. Base này được bê từ service `ApiMarkMan` (quản lý điểm) rồi dọn lại
> cho module **Cố vấn học tập** (chat cố vấn–sinh viên + thông báo).

---

## 1. Bức tranh tổng thể — một request đi qua đâu?

```
        HTTP request (từ React / service khác)
                    │
      ┌─────────────▼───────────────────────────────────────────┐
      │  MIDDLEWARE PIPELINE (khai báo trong Program.cs)         │
      │  ForwardedHeaders → ExceptionHandler → RequestLogging    │
      │  → Swagger → StaticFiles → CORS → Auth → Routing         │
      └─────────────┬───────────────────────────────────────────┘
                    │
              ┌─────▼──────┐   [Filters] chạy quanh Action:
              │ CONTROLLER │   MustLogged / AllowApiKeyOrJwt / SecurityHeaders...
              │ (API layer)│
              └─────┬──────┘
                    │  gọi qua ServiceWrapper.<TênService>
              ┌─────▼──────┐
              │  SERVICE   │   ← nơi viết LOGIC nghiệp vụ + KIỂM QUYỀN
              │ (nghiệp vụ)│
              └─────┬──────┘
                    │  gọi qua RepositoryWrapper.<TênRepo>
              ┌─────▼──────┐
              │ REPOSITORY │   ← chỉ lo TRUY CẬP DỮ LIỆU (gọi Stored Procedure)
              └─────┬──────┘
                    │  Common/Connection.cs (Dapper)
              ┌─────▼──────┐
              │ SQL SERVER │   ← Stored Procedures + bảng TB_*
              └────────────┘

  SignalR (chat real-time) đi đường riêng: client ⇄ Hubs/ChatHub.cs
```

**Nguyên tắc vàng:** Controller không chứa logic, Service không chứa SQL, Repository không
chứa nghiệp vụ. Mỗi tầng chỉ làm đúng việc của mình → dễ đọc, dễ test, dễ mở rộng.

---

## 2. Hai project trong solution

| Project | Loại | Vai trò |
|---|---|---|
| **ApiCoVanHocTap** | Web API (.NET) | Toàn bộ code chạy: controller, service, repository, hub, middleware... |
| **Models** | Class Library | Chỉ chứa **class dữ liệu** (DTO, enum, kết quả). Không phụ thuộc ASP.NET → tái dùng được, build nhẹ. |

`ApiCoVanHocTap` tham chiếu `Models`, không có chiều ngược lại.

---

## 3. Các pattern cốt lõi (đây là "quy ước team", đã chốt)

### 3.1. Wrapper + Service Locator
Thay vì Controller inject 5–10 service qua constructor, ta chỉ có **một** `IServiceWrapper`
gom tất cả service lại. Controller lấy nó ra và gọi `ServiceWrapper.HoiThoai.LayDanhSach()`.
Tương tự, Service lấy `IRepositoryWrapper` để gọi Repository.

- **Lợi:** thêm service mới không phải sửa constructor ở nhiều nơi.
- `BaseController` lấy wrapper qua `HttpContext.RequestServices.GetRequiredService<IServiceWrapper>()`
  (Service Locator) nên constructor của controller con luôn **trống**.

### 3.2. Lazy Loading với `??=`
Trong ServiceWrapper/RepositoryWrapper, mỗi service chỉ được `new` ra **khi gọi lần đầu**:
```csharp
public IHoiThoaiService HoiThoai =>
    _hoiThoaiService ??= new HoiThoaiService(_serviceProvider);
```
`??=` nghĩa là: "nếu đang null thì tạo mới rồi gán, ngược lại dùng lại cái cũ". → không tạo
thừa object cho những service không dùng tới trong request đó.

### 3.3. Vì sao truyền `IServiceProvider`?
Service/Repository nhận `IServiceProvider` để **tự lấy** dependency khi cần (thay vì khai báo
qua constructor). Đây là cách team đã chọn — bạn cứ theo, không cần đổi.

> 3 pattern trên là quy ước đã chốt của team. Đừng thay bằng constructor injection thuần,
> EF Core, MediatR... — sẽ lệch với các service khác trong hệ thống.

---

## 4. Chi tiết từng folder trong project `ApiCoVanHocTap`

| Folder / File | Để làm gì |
|---|---|
| **Program.cs** | Điểm khởi động. 2 phần: (1) *đăng ký service vào DI*, (2) *dựng middleware pipeline*. Muốn bật/tắt gì toàn cục thì sửa ở đây. |
| **Controllers/** | Tầng API. `BaseController.cs` = lớp cha mọi controller: cung cấp `ServiceWrapper`, các hàm trả về chuẩn (`OK`, `BadRequest`), lấy `GetUserId()` từ JWT. Controller con kế thừa nó, để **trống constructor**. |
| **Services/** | Tầng nghiệp vụ. `IServiceWrapper`/`ServiceWrapper` = danh mục service. `Services/Core/` = service hạ tầng dùng chung: `JwtTokenService` (đọc user từ token), gói trong `CoreServiceWrapper`. |
| **Repositories/** | Tầng truy cập dữ liệu. `IRepositoryWrapper`/`RepositoryWrapper` = danh mục repository. Repository gọi Stored Procedure qua `Common/Connection.cs`. *(Hiện để trống, có sẵn ví dụ mẫu.)* |
| **Hubs/** | SignalR (real-time). `ChatHub.cs` = các hàm client gọi để chat; `BaseHub.cs` chứa `CustomUserIdProvider` (định danh client theo `?userId=` trên URL) + helper gửi tin. |
| **Filters/** | Các "chốt chặn" chạy quanh Action (xem bảng 4.1). Xác thực + điều khiển ghi log. |
| **Middleware/** | Các lớp xử lý xuyên suốt mọi request (xem bảng 4.2): bắt lỗi, ghi log, cấu hình Serilog. |
| **Common/** | Tiện ích dùng chung (xem bảng 4.3). |
| **Properties/launchSettings.json** | Cấu hình **chỉ dùng khi chạy dev** (F5 / `dotnet run`): port (5041/7110), biến môi trường `ASPNETCORE_ENVIRONMENT=Development`. Không ảnh hưởng khi deploy. |
| **Assets/** | Thư mục file tĩnh: `Template/` (mẫu Word/Excel), `Upload/` (file người dùng tải lên). Được serve qua `/Assets/...`. File `.gitkeep` chỉ để giữ folder rỗng trong git. |
| **log/** | Nơi Serilog ghi file log. Bị `.gitignore` bỏ qua (không commit). |
| **bin/**, **obj/** | **Sản phẩm build tự sinh** — KHÔNG sửa tay, KHÔNG commit (đã ignore). Xoá đi build lại là có. |

### 4.1. Bảng Filters
| File | Tác dụng |
|---|---|
| `MustLogged.cs` | Cổng gác: chặn nếu không có JWT/API Key hợp lệ. Bật toàn cục bằng dòng `options.Filters.Add<MustLogged>()` trong Program.cs. |
| `AllowApiKeyOrJwtAttribute.cs` | Đặt lên Action cho phép vào bằng JWT **hoặc** API Key. |
| `ApiKeyAuthenticationAttribute.cs` | Đặt lên Action **bắt buộc** API Key (dùng cho service-to-service, vd module điểm bắn thông báo sang). |
| `ForceLogging.cs` / `IgnoreLogging.cs` | Ép ghi log / bỏ ghi log cho 1 Action. |
| `SensitivePayload.cs` | Đánh dấu Action có dữ liệu nhạy cảm (token, mật khẩu) → log sẽ **che** body. |
| `SecurityHeadersAttribute.cs` | Tự gắn HTTP security header vào response. Đã bật sẵn ở `BaseController`. |

### 4.2. Bảng Middleware
| File | Tác dụng |
|---|---|
| `ExceptionMiddlewareExtensions.cs` | Bắt mọi lỗi chưa xử lý → trả JSON lỗi thống nhất + ghi log. |
| `RequestLoggingMiddleware.cs` | Ghi log mọi request `/api/*` (thời gian, status, user, IP...). |
| `SerilogConfig.cs` | Cấu hình Serilog (hiện ghi ra file trong `/log`). |

### 4.3. Bảng Common
| File | Tác dụng |
|---|---|
| `Connection.cs` | Helper Dapper: mở kết nối + chạy Stored Procedure. Repository dựa vào file này. |
| `AppSettings.cs` | Đọc cấu hình từ `appsettings.json` một lần, giữ ở biến tĩnh để lấy nhanh mọi nơi. |
| `AppCommon.cs` | Hàm xử lý họ tên tiếng Việt (tách tên, bỏ dấu, chữ cái avatar). |
| `Extentions.cs` | Kho extension method: ép kiểu an toàn, đổi object ↔ DataTable ↔ tham số bảng cho SP, Map/Copy model, bọc kết quả SP thành `Response`. |

---

## 5. Chi tiết các folder trong project `Models`

| Folder | Chứa gì |
|---|---|
| **Base/** | Class nền: `BaseModel` (5 trường audit khớp mọi bảng `TB_*`: `is_deleted`, `created_time`...), `ModifyInfo`, `PagingResult<T>` (kết quả phân trang), `FunctionResult/SuccessResult/ErrorResult` (kết quả tầng Service). |
| **Responses/** | Kiểu trả về cho client: `ResponseBase`/`Response` (gói is_success + code + message + data), `ResponseCode`/`ResponseDetail` (danh mục mã lỗi + text). |
| **Request/** | Kiểu nhận từ client: `IdsRequest` (danh sách id int), `StrIdsRequest` (danh sách id chuỗi/GUID). |
| **Enum/** | `eMimeType` (loại file docx/pdf/xlsx), `eNotifyHubEvent` (tên sự kiện SignalR: CHAT_MESSAGE...). |
| **Table/**, **Static/** | Đang trống (chỉ có `.gitkeep`). **Table/** là nơi sẽ đặt model map với các bảng `TB_*` sau này. |

> **Lưu ý 2 kiểu "kết quả" đừng nhầm:**
> `FunctionResult<T>` = kết quả **nội bộ** giữa các tầng (Service trả về).
> `Response` = kết quả **trả cho client** (API trả ra JSON). Xem comment trong 2 file đó.

---

## 6. File cấu hình

| File | Vai trò |
|---|---|
| `appsettings.json` | Cấu hình chính: chuỗi kết nối DB, CORS, JWT, API Key, bật/tắt log. **KHÔNG bỏ mật khẩu thật vào đây** (file bị commit) — dùng `dotnet user-secrets` hoặc biến môi trường. |
| `appsettings.Development.json` | Ghi đè cấu hình khi chạy môi trường Development. |
| `*.csproj` | Khai báo package NuGet + phiên bản .NET của project. |
| `CoVanHocTap.slnx` | File solution (gom 2 project). |
| `.http` | File để test nhanh API ngay trong Visual Studio. |
| `.gitignore` | Danh sách file/thư mục KHÔNG commit (bin, obj, log, .vs, secret...). |
| `.vs/` | Cache của Visual Studio — tự sinh, không đụng, không commit. |

---

## 7. Cách THÊM một chức năng mới (ví dụ: API lấy danh sách hội thoại)

Đi từ dưới DB lên trên API:

1. **Stored Procedure** trong SQL Server, ví dụ `TB_HoiThoai_select_by_user`.
2. **Model** trong `Models/Table/HoiThoai.cs` — kế thừa `BaseModel` để có sẵn khối audit:
   ```csharp
   public class HoiThoai : BaseModel {
       public int id { get; set; }
       public int loai_hoi_thoai { get; set; }
       public string? tieu_de { get; set; }
       // ... map đúng cột bảng TB_HoiThoai
   }
   ```
3. **Repository**: tạo `IHoiThoaiRepository` + `HoiThoaiRepository`, gọi
   `Connection.SelectAsync<HoiThoai>("TB_HoiThoai_select_by_user", param)`.
   Khai property trong `IRepositoryWrapper` + `RepositoryWrapper` (theo mẫu comment sẵn).
4. **Service**: tạo `IHoiThoaiService` + `HoiThoaiService` chứa logic + **kiểm quyền**
   (cố vấn chỉ xem hội thoại của sinh viên mình phụ trách — tra `TB_PhanCongCoVan`).
   Khai property trong `IServiceWrapper` + `ServiceWrapper`.
5. **Controller**: tạo `HoiThoaiController : BaseController`, gọi
   `ServiceWrapper.HoiThoai.LayDanhSach(userId)` rồi `return OK(data)`.
6. Nếu cần **real-time** (báo có tin mới): gọi hàm đẩy tin trong `ChatHub` sau khi đã lưu DB.

> **Bảo mật nghiệp vụ (tài liệu nhấn mạnh):** luôn KIỂM QUYỀN ở **tầng Service** trước khi
> lưu/đẩy tin — không tin dữ liệu client gửi lên (client có thể giả `userId`).

---

## 8. Các bảng DB của module (tham chiếu nhanh)

Tất cả nằm trong DB chung `ESS_HOCVIENTAICHINH_DAOTAO`, tiền tố `TB_`, liên kết mềm qua ID
(không dùng khóa ngoại). `id_cb` = cố vấn (nvarchar 36, trỏ `PLAN_GiaoVien`), `id_sv` = sinh
viên (nvarchar 50, trỏ `STU_HoSoSinhVien`).

- **TB_PhanCongCoVan** — cố vấn ↔ sinh viên (nền tảng kiểm quyền).
- **TB_HoiThoai** / **TB_ThanhVienHoiThoai** — cuộc trò chuyện + thành viên.
- **TB_TinNhan** / **TB_TrangThaiDoc** / **TB_FileDinhKem** — tin nhắn, trạng thái đọc, file.
- **TB_ThongBao** / **TB_ThongBao_NguoiNhan** / **TB_LoaiThongBao** — thông báo.
- **TB_ChucVuCoVan**, **TB_Nhan** / **TB_HoiThoai_Nhan**, **TB_CauHinhWebsite** — danh mục & mở rộng.

Chi tiết cột xem file thiết kế DB (`ThietKe_DB_CoVanHocTap_v2.pdf`).

---

## 9. Những chỗ CHƯA hoàn thiện (cần làm khi vào nghiệp vụ)

- **Xác thực JWT:** hiện chỉ *đọc* claim từ token, **chưa kiểm chữ ký**. Nếu hệ thống cần
  ASP.NET tự validate token thì thêm `AddAuthentication().AddJwtBearer(...)` ở Program.cs.
- **API Key:** đã thống nhất đọc từ `ApiSettings:ApiKey` — nhớ sinh key thật (đang là placeholder).
- **Redis / Cache:** chưa cài (JwtTokenService có sẵn phần code cache đang comment).
- **Ghi log vào SQL:** đang ghi ra file; bật SQL sink khi cài `Serilog.Sinks.MSSqlServer`.
- **TargetFramework:** đang `net10.0`, checklist ghi `.NET 8` — **hỏi lead** để chốt cho khớp môi trường deploy.
