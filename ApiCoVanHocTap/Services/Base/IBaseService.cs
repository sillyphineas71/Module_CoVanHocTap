using Models.Base;

namespace ApiCoVanHocTap.Services.Base
{
    public interface IBaseService
    {
        void SetModifyUser(ModifyInfo info);
        string GetUserName();
        string GetUserID();
        string GetIP();
    }
}