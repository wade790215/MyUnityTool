using MyPackages.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class UIMainPage : UIPage {

        public UIMainPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
        {
            uiPath = "Prefab/UIMain";
        }

        protected override void Awake(GameObject go)
        {
            this.transform.Find("btn_skill").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPage<UISkillPage>();
            });

            this.transform.Find("btn_battle").GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPage<UIBattle>();
            });
        }
    }
}
