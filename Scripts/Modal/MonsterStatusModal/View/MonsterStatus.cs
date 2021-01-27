using System;
using Consts.Enums;
using Dictionary;
using Serializable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modal.MonsterStatusModal.View
{
    public class MonsterStatus : MonoBehaviour
    {
        public GameObject monsterName;
        public GameObject monsterImage;
        public GameObject monsterLevel;

        public GameObject statusContent;
        public GameObject description;

        public void Refresh(BattlerSerializable battler)
        {
            monsterName.GetComponent<TextMeshProUGUI>().text = battler.name;
            var sprite = MonsterDicionary.GetSprite(battler.monsterId);
            monsterImage.GetComponent<Image>().sprite = sprite;
            monsterLevel.GetComponent<TextMeshProUGUI>().text = "レベル " + battler.level;
            CreateStatusContent(battler);
        }

        private void CreateStatusContent(BattlerSerializable battler)
        {
            foreach (Transform child in statusContent.transform)
            {
                Destroy(child.gameObject);
            }

            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.HP, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.MP, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.ATK, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.DEF, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.MAT, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.MDF, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.SPD, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.HIT, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.DODGE, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.CRIT, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.EFC, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.RES, battler);
            CreateTextContentInstantiate().GetComponent<StatusContentBehaviour>()
                .Create(StatusTypeEnum.StatusType.HOS, battler);
        }

        private GameObject CreateTextContentInstantiate()
        {
            var text = Instantiate(
                (GameObject) Resources.Load("Prefabs/Modal/MonsterStatus/TextContent"),
                statusContent.transform);
            text.GetComponent<StatusContentBehaviour>().description = description;
            return text;
        }
    }

}