namespace Models.Enum
{
    /// <summary>
    /// Tên các sự kiện server đẩy xuống client qua SignalR.
    /// Dùng enum thay cho chuỗi "magic string" để tránh gõ sai giữa Hub và client.
    /// Client lắng nghe theo đúng tên: connection.on("CHAT_MESSAGE", ...).
    /// </summary>
    public enum eNotifyHubEvent
    {
        /// <summary>Có tin nhắn chat mới trong hội thoại.</summary>
        CHAT_MESSAGE,

        /// <summary>Tin nhắn đã được người nhận đọc.</summary>
        MESSAGE_READ,

        /// <summary>Có thông báo mới (học vụ / hành chính...).</summary>
        NOTIFICATION_NEW
    }
}
