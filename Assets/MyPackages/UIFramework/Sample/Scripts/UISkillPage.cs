﻿using System.Collections.Generic;
using DG.Tweening;
using MyPackages.UIFramework.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MyPackages.UIFramework.Sample.Scripts
{
    public class UISkillPage : UIPage
    {
        GameObject skillList = null;
        GameObject skillDesc = null;
        GameObject skillItem = null;
        List<UISkillItem> skillItems = new();
        UISkillItem currentItem = null;

        public UISkillPage() : base(UIType.Normal, UIMode.HideOther, UICollider.None)
        {
            uiPath = "Prefab/UISkill";
        }

        protected override void Awake(GameObject go)
        {
            skillList = this.transform.Find("list").gameObject;
            skillDesc = this.transform.Find("desc").gameObject;
            skillDesc.transform.Find("btn_upgrade").GetComponent<Button>().onClick.AddListener(OnClickUpgrade);

            skillItem = this.transform.Find("list/Viewport/Content/item").gameObject;
            skillItem.SetActive(false);
        }

        protected override void Refresh()
        {
            skillDesc.SetActive(false);
            skillList.transform.localScale = Vector3.zero;
            skillList.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            UDSkill skillData = this.data != null ? this.data as UDSkill : GameData.Instance.playerSkill;
            for (int i = 0; i < skillData.skills.Count; i++)
            {
                CreateSkillItem(skillData.skills[i]);
            }
        }

        protected override void Hide()
        {
            for (int i = 0; i < skillItems.Count; i++)
            {
                GameObject.Destroy(skillItems[i].gameObject);
            }

            skillItems.Clear();
            gameObject.SetActive(false);
        }

        private void CreateSkillItem(UDSkill.Skill skill)
        {
            GameObject go = GameObject.Instantiate(skillItem) as GameObject;
            go.transform.SetParent(skillItem.transform.parent);
            go.transform.localScale = Vector3.one;
            go.SetActive(true);

            UISkillItem item = go.AddComponent<UISkillItem>();
            item.Refresh(skill);
            skillItems.Add(item);

            //add click btn
            go.AddComponent<Button>().onClick.AddListener(OnClickSkillItem);
        }

        private void OnClickSkillItem()
        {
            UISkillItem item = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject
                .GetComponent<UISkillItem>();
            ShowDesc(item);
        }

        private void ShowDesc(UISkillItem skill)
        {
            currentItem = skill;
            skillDesc.SetActive(true);
            skillDesc.transform.localPosition = new Vector3(300f, skillDesc.transform.localPosition.y,
                skillDesc.transform.localPosition.z);
            RectTransform rt = skillDesc.GetComponent<RectTransform>();
            Vector2 endPos = new Vector2(-289.28f, -44.05f);

            DOTween.To(() => rt.anchoredPosition, x => rt.anchoredPosition = x, endPos, 0.25f)
                .SetEase(Ease.OutQuad)
                .SetOptions(true);     

            RefreshDesc(skill);
        }

        private void RefreshDesc(UISkillItem skill)
        {
            skillDesc.transform.Find("content").GetComponent<Text>().text =
                skill.data.desc + "\n名称:" + skill.data.name + "\n等级:" + skill.data.level;
        }

        private void OnClickUpgrade()
        {
            currentItem.data.level++;
            currentItem.Refresh(currentItem.data);
            RefreshDesc(currentItem);
        }
    }
}