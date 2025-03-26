using UnityEngine;

public class GameMain : MonoBehaviour
{
    void Start()
    {
        UIPage.ShowPage<UITopBar>();
        UIPage.ShowPage<UIMainPage>();
    }
}