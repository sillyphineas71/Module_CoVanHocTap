using Models.Enum;

namespace ApiCoVanHocTap.Hubs
{
    /// <summary>
    /// Hub xử lý tính năng chat real-time giữa sinh viên và cố vấn học tập.
    ///
    /// Route: /hubs/chat  (đăng ký trong Program.cs: app.MapHub&lt;ChatHub&gt;("/hubs/chat"))
    ///
    /// Cách client gửi message:
    ///   connection.invoke("SendChatMessageAsync", "receiverUserId", "Hello!");
    ///
    /// Cách client nhận message:
    ///   connection.on("CHAT_MESSAGE", (payload) => { ... });
    ///
    /// LƯU Ý NGHIỆP VỤ: Hub hiện chỉ đẩy tin real-time. Việc KIỂM TRA QUYỀN
    /// (cố vấn chỉ được chat với sinh viên mình phụ trách) và LƯU tin nhắn vào DB
    /// phải làm ở tầng Service trước khi gọi hàm đẩy tin — không tin tưởng client.
    /// </summary>
    public class ChatHub : BaseHub
    {
        /// <summary>
        /// Gửi tin nhắn từ client hiện tại đến user đích.
        /// </summary>
        /// <param name="toUserId">UserID người nhận.</param>
        /// <param name="message">Nội dung tin nhắn.</param>
        public async Task<bool> SendChatMessageAsync(string toUserId, string message)
        {
            var payload = new
            {
                fromUserId = Context.UserIdentifier,
                message,
                sentAt = DateTime.UtcNow
            };
            return await SendMessageToUserAsync(toUserId, nameof(eNotifyHubEvent.CHAT_MESSAGE), payload);
        }

        /// <summary>
        /// Broadcast thông báo hệ thống đến toàn bộ client.
        /// Chỉ dùng nội bộ (server-side call).
        /// </summary>
        public async Task<bool> BroadcastSystemNoticeAsync(string eventName, object payload)
        {
            return await SendMessageToUserAsync(string.Empty, eventName, payload);
        }
    }
}
