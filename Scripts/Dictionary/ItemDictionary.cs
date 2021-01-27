using System;
using Consts;
using Consts.Enums;
using Model;
using Model.Master;
using Model.Structs;
using Serializable;
using Structs;
using UnityEngine;

namespace Dictionary
{
    public class ItemDictionary
    {
        public static ItemSerializable GetItem(string itemId)
        {
            return MasterItemDataModel.Instance.Data.Find(x => x.data.itemId == itemId);
        }

        public static Sprite GetLootSprite(LootItemStruct loot)
        {
            switch (loot.ItemId)
            {
                case ItemConsts.GOLD:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/Gold");
                case ItemConsts.LEVEL:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/level-1");
            }
            switch (loot.Reality)
            {
                case RealityEnum.Reality.Common:
                case RealityEnum.Reality.UnCommon:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/root-1");
                case RealityEnum.Reality.Rare:
                case RealityEnum.Reality.Magic:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/root-2");
                case RealityEnum.Reality.Ancient:
                case RealityEnum.Reality.Epic:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/root-3");
                case RealityEnum.Reality.Miracle:
                case RealityEnum.Reality.Legend:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/root-4");
                case RealityEnum.Reality.God:
                    return Resources.Load<Sprite>("Sprites/Icons/Misc/root-5");
            }
            return Resources.Load<Sprite>("Sprites/Items/" + GetItem(loot.ItemId).imageData.spritePath);
        }

        public static String GetText(LootItemStruct loot)
        {
            string text;
            switch (loot.ItemId)
            {
                case ItemConsts.GOLD:
                    //text = loot.Amount + $"({InventoryDictionary.GetAmount(loot.ItemId)})";
                    text = $"ゴールド獲得 + {loot.Amount}";
                    break;
                case ItemConsts.LEVEL:
                    text = "レベル上昇 + " + loot.Amount;
                    break;
                default:
                    /*var item = GetItem(loot.ItemId);
                    text = item.data.name;
                    if (loot.Amount > 1)
                        text += $" × {loot.Amount}";
                    var hasAmount = InventoryDictionary.GetAmount(loot.ItemId);
                    if (hasAmount > 0)
                        text += $" ({hasAmount})";
                        */
                    text = "ルートボックス";
                    break;
            }
            return text;
        }

        public static String GetAnnounceText(LootItemStruct loot)
        {
            string text;
            switch (loot.ItemId)
            {
                case ItemConsts.GOLD:
                    text = $"ゴールドを {loot.Amount} 獲得した";
                    break;
                case ItemConsts.LEVEL:
                    text = $"レベルが {loot.Amount} 上昇した";
                    break;
                default:
                    text = $"{GetText(loot)}を獲得した";
                    break;
            }
            return text;
        }

        /// <summary>
        /// アイテムの種類によってカラーを変更する
        /// TODO デザイン的に必要？
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static Color GetColor(string itemId)
        {
            return Color.white;
            /*
            switch (itemId)
            {
                case ItemConsts.GOLD:
                    return Color.white;
                case ItemConsts.LEVEL:
                    return Color.white;
            }
            var item = GetItem(itemId);
            switch (item.data.type)
            {
                case ItemTypeConst.MATERIAL:
                    switch (item.data.reality)
                    {
                        case RealityEnum.Reality.Common:
                            return Color.white;
                        case RealityEnum.Reality.UnCommon:
                            return new Color(159,255,126); 
                    }
                    break;
                case ItemTypeConst.SOUL:
                    return new Color(255,126,252);
                case ItemTypeConst.RELIC:
                    return new Color(235,255,0);
            }
            return Color.white;*/
        }

        /// <summary>
        /// アイテムの種類を表示させる
        /// TODO 必要なくなった
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public static string GetCategoryName(string itemId)
        {
            /*switch (itemId)
            {
                case ItemConsts.GOLD:
                    return "ゴールド";
                case ItemConsts.LEVEL:
                    return "";
            }
            var item = GetItem(itemId);
            switch (item.data.type)
            {
                case ItemTypeConst.SOUL:
                    return "ソウルクリスタル";
                case ItemTypeConst.RELIC:
                    return "レリック";
                case ItemTypeConst.MATERIAL:
                    return "マテリアル";
            }*/

            return "";
        }
        
    }
}