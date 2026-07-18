using Models.Base;

namespace ApiCoVanHocTap.Services
{
    /// <summary>
    /// Interface xác thực và đọc thông tin người dùng từ JWT token.
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>Lấy UserName từ JWT trong HttpContext.</summary>
        string GetUserName(HttpContext httpContext);

        /// <summary>Lấy UserID từ JWT trong HttpContext.</summary>
        string GetUserID(HttpContext httpContext);

        /// <summary>Lấy địa chỉ IP của client.</summary>
        string GetIP(HttpContext httpContext);

        /// <summary>Lấy raw Bearer token từ Authorization header.</summary>
        string GetUserToken(HttpContext httpContext);

        /// <summary>
        /// Set thông tin người tạo/sửa vào ModifyInfo từ JWT claims.
        /// </summary>
        void SetModifyInfo(HttpContext httpContext, ModifyInfo info);

        // ── Các phương thức liên quan đến OIDC / Cache token ──────────────
        // Uncommment khi đã có OidcUserInfo và ICacheService:
        //
        // Task<bool> SaveAcessTokenInfoAsync(string access_token, OidcUserInfo userInfo);
        // Task<bool> DeleteAcessTokenInfoAsync(string access_token);
        // Task<FunctionResult<JwtTokenInfo>> ReadAccessTokenAsync(string token);
    }
}
