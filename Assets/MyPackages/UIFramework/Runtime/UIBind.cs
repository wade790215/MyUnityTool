using UnityEngine;

public class UIBind : MonoBehaviour
{
    static bool isBind = false;

    public static void Bind()
    {
        if (!isBind)
        {
            isBind = true;
            UIPage.delegateSyncLoadUI = Resources.Load;
            //TTUIPage.delegateAsyncLoadUI = UILoader.Load;
        }
    }
}
