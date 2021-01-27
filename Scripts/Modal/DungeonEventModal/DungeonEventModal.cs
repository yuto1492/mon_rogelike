using System.Collections.Generic;
using Dictionary;
using Model.Structs;
using Structs;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Modal.DungeonEventModal
{
    public class DungeonEventModal : ModalBase
    {
        public DungeonEventModal()
        {
            if (Modal == null)
            {
                Modal = Object.Instantiate(
                    (GameObject) Resources.Load("Prefabs/Modal/DungeonEvent/DungeonEventModal"));
                Modal.GetComponent<Canvas>().worldCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
            }
        }

        public void EventText(string text)
        {
            Modal.transform.Find("Content/Text").GetComponent<TextMeshProUGUI>().text = text;
        }

        /// <summary>
        /// 選択肢を追加する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public AsyncSubject<Unit> AddChoice(string text)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            
            var choicesTransform = Modal.transform.Find("Content/Choices");
            var choice = Object.Instantiate(
                (GameObject) Resources.Load("Prefabs/Modal/DungeonEvent/Parts/Choice"),choicesTransform);
            choice.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
            choice.OnMouseUpAsButtonAsObservable().Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
                Close();
            });
            return subject;
        }

        /// <summary>
        /// 獲得したアイテムを表示する
        /// </summary>
        /// <param name="loots"></param>
        public void AddLootImages(List<LootItemStruct> loots)
        {
            var lootTransform = Modal.transform.Find("Content/LootItems");
            loots.ForEach(loot =>
            {
                var lootObject = Object.Instantiate(
                    (GameObject) Resources.Load("Prefabs/Modal/DungeonEvent/Parts/LootItem"), lootTransform);
                lootObject.GetComponent<Image>().sprite = ItemDictionary.GetLootSprite(loot);
            });
        }

        /// <summary>
        /// イベント画像を表示する
        /// </summary>
        /// <param name="sprite"></param>
        public void AddImage(Sprite sprite)
        {
        }

    }
}