using ApiCoVanHocTap.Common;
using Microsoft.Net.Http.Headers;
using Models.Base;
using System.IdentityModel.Tokens.Jwt;

namespace ApiCoVanHocTap.Services
{
    /// <summary>
    /// Triển khai IJwtTokenService.
    /// Đọc thông tin người dùng trực tiếp từ JWT claims trong Authorization header.
    ///
    /// NOTE: Phiên bản này KHÔNG dùng ICacheService/OIDC.
    /// Nếu cần OIDC + Redis cache thì mở comment các method bên dưới và
    /// inject ICacheService + ICacheKeyService.
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        // Không cần constructor inject gì thêm — đọc trực tiếp từ JWT claims
        public JwtTokenService(IServiceProvider serviceProvider) { }

        // ====================================================================
        // PUBLIC INTERFACE
        // ====================================================================

        public void SetModifyInfo(HttpContext httpContext, ModifyInfo info)
        {
            try
            {
                var token = GetUserToken(httpContext);
                if (string.IsNullOrEmpty(token)) return;

                var jwtToken = ParseToken(token);
                if (jwtToken == null) return;

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "";
                info.created_user_id      = userId;
                info.last_modified_user_id = userId;
                info.created_time         = DateTime.Now;
                info.last_modified_times  = DateTime.Now;
            }
            catch { /* Silently ignore — không làm crash pipeline */ }
        }

        public string GetUserName(HttpContext httpContext)
        {
            try
            {
                var token = GetUserToken(httpContext);
                var jwt   = ParseToken(token);
                return jwt?.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? "";
            }
            catch { return ""; }
        }

        public string GetUserID(HttpContext httpContext)
        {
            try
            {
                var token = GetUserToken(httpContext);
                var jwt   = ParseToken(token);
                return jwt?.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value ?? "";
            }
            catch { return ""; }
        }

        public string GetIP(HttpContext httpContext)
        {
            try
            {
                return httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
            }
            catch { return ""; }
        }

        public string GetUserToken(HttpContext httpContext)
        {
            try
            {
                var auth = httpContext.Request.Headers[HeaderNames.Authorization]
                                              .ConvertToString()
                                              .Replace("Bearer ", "");
                return auth;
            }
            catch { return ""; }
        }

        // ====================================================================
        // PRIVATE HELPERS
        // ====================================================================

        private static JwtSecurityToken? ParseToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ReadJwtToken(token);
            }
            catch { return null; }
        }

        // ====================================================================
        // OIDC / CACHE — Uncommment khi có ICacheService + ICacheKeyService
        // ====================================================================

        // public async Task<bool> SaveAcessTokenInfoAsync(string access_token, OidcUserInfo userInfo)
        // {
        //     var sig      = GetSignature(access_token);
        //     var expiry   = GetExpirationTimeFromToken(access_token);
        //     if (expiry != null)
        //         await _cacheService.SetDataAsync(_cacheKeyService.GetAccessTokenCacheKey(sig), userInfo, expiry);
        //     return true;
        // }
        //
        // public async Task<bool> DeleteAcessTokenInfoAsync(string access_token)
        // {
        //     var sig = GetSignature(access_token);
        //     await _cacheService.RemoveDataAsync(_cacheKeyService.GetAccessTokenCacheKey(sig));
        //     return true;
        // }
        //
        // private string GetSignature(string token)
        // {
        //     var parts = token.Split('.');
        //     return parts.Length == 3 ? parts[2] : (parts.Length == 1 ? parts[0] : "");
        // }
        //
        // private DateTime? GetExpirationTimeFromToken(string token)
        // {
        //     var jwt = ParseToken(token);
        //     if (jwt?.Payload?.Expiration == null) return null;
        //     return DateTimeOffset.FromUnixTimeSeconds((long)jwt.Payload.Expiration).UtcDateTime;
        // }
    }
}
