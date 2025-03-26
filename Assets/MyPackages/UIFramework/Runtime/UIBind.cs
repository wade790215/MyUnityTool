using UnityEngine;

namespace MyPackages.UIFramework.Runtime
{
    public class UIBind
    {
        static bool isBind = false;

        public static void Bind()
        {
            if (isBind) return;
            isBind = true;
            UIPage.delegateSyncLoadUI = Resources.Load;
        }
    }
}