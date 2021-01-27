using System.Collections.Generic;
using System.Linq;
using Battle.Logic;
using Container;
using DG.Tweening;
using Dictionary;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Battle.view.GUI
{
    public struct SkillStruct
    {
        public string SkillId;
    }
    
    public class SkillsView
    {
        private GameObject _skillParent;
        private List<Transform> _icons = new List<Transform>();
        private Material _selectOutlineMaterial;
        private Material _selectedOutlineMaterial;
        
        private bool _skillSelected;

        private GameObject _descriptionTextObject;
        private IDisposableContainer _disposableContainer = new IDisposableContainer();
        
        public void Initialize()
        {
            _skillParent = GameObject.Find("SkillUI");
            foreach (Transform icon in _skillParent.transform.Find("SkillIcons"))
            {
                _icons.Add(icon);
            }

            _descriptionTextObject = _skillParent.transform.Find("Description/text").gameObject;
            _skillParent.GetComponent<CanvasGroup>().alpha = 0;
            _skillParent.SetActive(false);
            _selectOutlineMaterial = Resources.Load("Material/SelectOutline", typeof(Material)) as Material;
            _selectedOutlineMaterial = Resources.Load("Material/SelectedOutline", typeof(Material)) as Material;
        }
        
        public void SkillView(int uniqId)
        {
            _skillParent.SetActive(true);
            var canvasGroup = _skillParent.GetComponent<CanvasGroup>();
            DOTween.To(() => canvasGroup.alpha, (x) => canvasGroup.alpha = x, 1, 0.2f).Play();
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var eqSkills = battler.skills;
            foreach (var (x, i) in _icons.Select((x, i) => (x, i)))
            {
                var image = x.Find("Image").GetComponent<Image>();
                if (i < eqSkills.Count)
                {
                    var skill = SkillsDicionary.GetSkillById(eqSkills[i]);
                    
                    Sprite sprite = Resources.Load<Sprite>(skill.iconSpritePath);
                    //Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(skill.iconSpritePath);
                    image.sprite = sprite;
                    image.enabled = true;
                }
                else
                {
                    image.sprite = null;
                    image.enabled = false;
                }
            }
        }

        public void SkillHide()
        {
            SkillCellMatAllClear();
            _skillParent.SetActive(false);
        }
        
        public void Refresh(BattleSkillModel model)
        {
            foreach (var (x, index) in _icons.Select((x, index) => (x, index)))
            {
                var background = x.Find("background").GetComponent<Image>();
                if (model.ActiveIndex == index)
                    background.material = _selectedOutlineMaterial;
                else if (model.HoverIndex == index)
                    background.material = _selectOutlineMaterial;
                else
                    background.GetComponent<Image>().material = null;
            }
        }

        private void SkillCellMatAllClear()
        {
            foreach (var (x, index) in _icons.Select((x, index) => (x, index)))
            {
                var background = x.Find("background").GetComponent<Image>();
                background.GetComponent<Image>().material = null;
            }
        }

        public void ViewSkillDescription(string skillId, int uniqId)
        {
            //スキル説明の表示
            /*_descriptionTextObject.GetComponent<TextMeshProUGUI>()
                .SetText(SkillLogic.CreateSkillDescription(skillId, BattlerDictionary.GetBattlerByUniqId(uniqId)));*/
            _descriptionTextObject.GetComponent<Text>()
                .text = SkillLogic.CreateSkillDescription(skillId, BattlerDictionary.GetBattlerByUniqId(uniqId));
        }

        public void CleanSkillDescription()
        {
            _descriptionTextObject.GetComponent<TextMeshProUGUI>()
                .SetText("");
        }

        public void SkillSelected()
        {
            _disposableContainer.AllDispose();
        }

        public List<Transform> Icons
        {
            get => _icons;
        }
    }
}