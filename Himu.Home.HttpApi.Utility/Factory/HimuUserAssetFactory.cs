using Himu.EntityFramework.Core.Entity;

namespace Himu.HttpApi.Utility
{
    public static class HimuUserAssetFactory
    {
        //public static HimuUserAssetList CreateDefaultAssets(HimuHomeUser user)
        //{
        //    throw new NotImplementedException();
        //}

        public static string CreateDefaultAvatar(HimuHomeUser user) => "images/default/default_avatar.png";

        public static string CreateDefaultBackground(HimuHomeUser user) => string.Empty;
    }
}