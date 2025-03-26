using MyPackages.UIFramework.Runtime;
using UnityEngine;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class GameMain : MonoBehaviour
    {
        private void Start()
        {
            UIPage.ShowPage<UITopBar>();
            UIPage.ShowPage<UIMainPage>();
        }
    }
}