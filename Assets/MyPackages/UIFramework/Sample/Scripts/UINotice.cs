using MyPackages.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class UINotice : UIPage
    {
        public UINotice() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
        {
            uiPath = "Prefab/Notice";
        }

        protected override void Awake(GameObject go)
        {
            gameObject.transform.Find("content/btn_confim").GetComponent<Button>().onClick.AddListener(() =>
            {
                Hide();
            });
        }

        protected override void Refresh()
        {

        }
    }
}
