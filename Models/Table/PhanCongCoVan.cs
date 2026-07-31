using Models.Base;

namespace Models.Table
{
    // Kế thừa ModifyInfo → có sẵn 5 field audit (is_deleted, created_time...)
    public class PhanCongCoVan : ModifyInfo
    {
        public int id { get; set; }
        public string id_cb { get; set; } = string.Empty;
        public string id_sv { get; set; } = string.Empty;
        public int id_chuc_vu { get; set; }
        public int? tu_ky { get; set; }
        public string? nam_bat_dau { get; set; }
        public int? den_ky { get; set; }
        public string? nam_ket_thuc { get; set; }
        public string? ghi_chu { get; set; }
    }
}