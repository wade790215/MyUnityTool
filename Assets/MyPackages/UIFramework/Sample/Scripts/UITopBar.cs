using MyPackages.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class UITopBar : UIPage
    {

        public UITopBar() : base(UIType.Fixed, UIMode.DoNothing, UICollider.None)
        {
            uiPath = "Prefab/Topbar";
        }

        protected override void Awake(GameObject go)
        {
            this.gameObject.transform.Find("btn_back").GetComponent<Button>().onClick.AddListener(() =>
            {
                ClosePage();
            });

            this.gameObject.transform.Find("btn_notice").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPage<UINotice>();
            });
        }


    }
}
