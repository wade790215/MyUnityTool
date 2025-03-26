using MyPackages.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class UIBattle : UIPage {

        public UIBattle() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
        {
            uiPath = "Prefab/UIBattle";
        }

        protected override void Awake(GameObject go)
        {
            transform.Find("btn_skill").GetComponent<Button>().onClick.AddListener(OnClickSkillGo);
            transform.Find("btn_battle").GetComponent<Button>().onClick.AddListener(OnClickGoBattle);
        }

        private void OnClickSkillGo()
        {
            ShowPage<UISkillPage>();
        }

        private void OnClickGoBattle()
        {
            Debug.Log("should load your battle scene!");
        }
    }
}
