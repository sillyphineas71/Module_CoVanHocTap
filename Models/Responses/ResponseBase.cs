using System.Text.Json;
using System.Text.Json.Serialization;

namespace Models
{
    /// <summary>
    /// Response wrapper chuẩn cho toàn bộ API.
    /// Dùng System.Text.Json thay cho Newtonsoft để không phụ thuộc package ngoài trong Models.
    /// </summary>
    [Serializable]
    public class ResponseBase<T>
    {
        public bool   is_success { get; set; } = true;
        public string code       { get; set; } = ResponseCode.SUCCESS;
        public string message    { get; set; } = ResponseDetail.SUCCESSDETAIL;
        public T?     data       { get; set; }

        private static readonly JsonSerializerOptions _auditExcludeOptions = new()
        {
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            // Lọc thủ công các trường audit ở tầng Controller nếu cần
        };

        private static readonly JsonSerializerOptions _defaultOptions = new()
        {
            PropertyNamingPolicy = null,
        };

        public ResponseBase() { }

        public ResponseBase(T data)
        {
            this.data = data;
        }

        /// <summary>Serialize thành JSON string, lọc bỏ các trường audit (is_deleted, created_*, …).</summary>
        public string ToJson(bool excludeAuditFields = true)
        {
            if (excludeAuditFields)
                return JsonSerializer.Serialize(this, _auditExcludeOptions);
            return JsonSerializer.Serialize(this, _defaultOptions);
        }
    }

    public class ResponeBaseSuccess : ResponseBase<object>
    {
        public ResponeBaseSuccess(object data, string message = "")
        {
            is_success  = true;
            code        = ResponseCode.SUCCESS;
            this.message = string.IsNullOrEmpty(message) ? ResponseDetail.SUCCESSDETAIL : message;
            this.data   = data;
        }

        public ResponeBaseSuccess(string message = "")
        {
            is_success   = true;
            code         = ResponseCode.SUCCESS;
            this.message = message;
            data         = null;
        }
    }

    public class ResponeBaseErr : ResponseBase<object>
    {
        public ResponeBaseErr(string message = "")
        {
            is_success   = false;
            code         = ResponseCode.SYSTEM_ERROR;
            this.message = message;
        }
    }
}
