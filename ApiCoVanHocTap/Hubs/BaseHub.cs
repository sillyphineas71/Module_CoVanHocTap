using Microsoft.AspNetCore.SignalR;

namespace ApiCoVanHocTap.Hubs
{
    /// <summary>
    /// Custom UserIdProvider – lấy userId từ query string.
    /// 
    /// Cách dùng từ client:
    ///   const connection = new HubConnectionBuilder()
    ///       .withUrl("/hubs/chat?userId=123")
    ///       .build();
    /// 
    /// Đăng ký trong Program.cs:
    ///   builder.Services.AddSingleton&lt;IUserIdProvider, CustomUserIdProvider&gt;();
    /// </summary>
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var httpContext = connection.GetHttpContext();
            return httpContext?.Request.Query["userId"].ToString();
        }
    }

    /// <summary>
    /// Base Hub dùng chung – wrapper helper để gửi message đến user hoặc broadcast.
    /// Tương đương BaseHub trong ApiMarkMan.
    /// </summary>
    public abstract class BaseHub : Hub
    {
        /// <summary>
        /// Gửi message đến một user cụ thể.
        /// Nếu <paramref name="userId"/> rỗng → broadcast đến tất cả client.
        /// </summary>
        /// <param name="userId">UserID đích (khớp với CustomUserIdProvider).</param>
        /// <param name="eventName">Tên event client lắng nghe (ví dụ: "CHAT_MESSAGE").</param>
        /// <param name="payload">Dữ liệu gửi kèm.</param>
        protected async Task<bool> SendMessageToUserAsync(
            string userId,
            string eventName,
            object payload)
        {
            try
            {
                if (Clients is null) return false;

                if (string.IsNullOrWhiteSpace(userId))
                    await Clients.All.SendAsync(eventName, payload);
                else
                    await Clients.User(userId).SendAsync(eventName, payload);

                return true;
            }
            catch (Exception ex)
            {
                // TODO: Thay thế bằng ILogger hoặc Serilog
                Console.Error.WriteLine($"[SignalR][BaseHub] Error sending to user '{userId}': {ex.Message}");
                return false;
            }
        }
    }
}
