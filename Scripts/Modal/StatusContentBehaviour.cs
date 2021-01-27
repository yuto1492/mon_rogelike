using System;
using Consts.Enums;
using Serializable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Modal
{
    public class StatusContentBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private string _descriptionText;
        public GameObject description;


        public void Create(StatusTypeEnum.StatusType type, BattlerSerializable battler)
        {
            switch (type)
            {
                case StatusTypeEnum.StatusType.HP:
                    SetValue("HP", $"{battler.parameter.hp} / {battler.parameter.maxHp}");
                    break;
                case StatusTypeEnum.StatusType.MP:
                    SetValue("MP", $"{battler.parameter.mp} / {battler.parameter.maxMp}");
                    break;
                case StatusTypeEnum.StatusType.ATK:
                    SetValue("攻撃力", $"{battler.parameter.atk}");
                    break;
                case StatusTypeEnum.StatusType.DEF:
                    SetValue("防御力", $"{battler.parameter.def}");
                    break;
                case StatusTypeEnum.StatusType.MAT:
                    SetValue("魔法攻撃力", $"{battler.parameter.mat}");
                    break;
                case StatusTypeEnum.StatusType.MDF:
                    SetValue("魔法防御力", $"{battler.parameter.mdf}");
                    break;
                case StatusTypeEnum.StatusType.SPD:
                    SetValue("速度", $"{battler.parameter.spd}");
                    break;
                case StatusTypeEnum.StatusType.HIT:
                    SetValue("命中率", $"{battler.parameter.hit}");
                    break;
                case StatusTypeEnum.StatusType.DODGE:
                    SetValue("回避率", $"{battler.parameter.dodge}");
                    break;
                case StatusTypeEnum.StatusType.CRIT:
                    SetValue("クリティカル率", $"{battler.parameter.crit}");
                    break;
                case StatusTypeEnum.StatusType.EFC:
                    SetValue("効果的中率", $"{battler.parameter.efc}");
                    break;
                case StatusTypeEnum.StatusType.RES:
                    SetValue("効果抵抗率", $"{battler.parameter.res}");
                    break;
                case StatusTypeEnum.StatusType.HOS:
                    SetValue("敵対率", $"{battler.parameter.hostile}");
                    break;
            }

            SetDescription(type, battler);
        }

        private void SetValue(string type, string value)
        {
            gameObject.transform.Find("Text/Type").GetComponent<TextMeshProUGUI>().text = type;
            gameObject.transform.Find("Text/Value").GetComponent<TextMeshProUGUI>().text = value;
        }

        private void SetDescription(StatusTypeEnum.StatusType type, BattlerSerializable battler)
        {
            //TODO
            switch (type)
            {
                case StatusTypeEnum.StatusType.HP:
                    _descriptionText = $"最大HP\n{battler.parameter.maxHp}\n基礎値:{battler.parameter.maxHp}\n補正値:0";
                    break;
                case StatusTypeEnum.StatusType.MP:
                    _descriptionText = $"最大MP\n{battler.parameter.maxMp}\n基礎値:{battler.parameter.maxMp}\n補正値:0";
                    break;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.Find("OverLay").GetComponent<Animation>().Play();
            description.GetComponent<TextMeshProUGUI>().text = _descriptionText;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            var overLay = gameObject.transform.Find("OverLay");
            overLay.GetComponent<Animation>().Stop();
            overLay.GetComponent<CanvasGroup>().alpha = 0;
            description.GetComponent<TextMeshProUGUI>().text = "";
        }

    }
}