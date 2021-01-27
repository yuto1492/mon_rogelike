using Consts.Enums;
using Dictionary;
using Model.Structs;
using Serializable;
using Structs;
using UnityEngine;

namespace Item.Logic
{
    public class ItemLogic
    {
        public static ItemStruct CreateItemFromLoot(LootItemStruct loot)
        {
            var itemData = ItemDictionary.GetItem(loot.ItemId);
            if (itemData.parameter != null)
                CalcParameter(ref itemData.parameter, loot.Reality);
            ItemStruct item = new ItemStruct()
            {
                Reality = loot.Reality,
                Amount = loot.Amount,
                Data = itemData
            };
            return item;
        }

        private static void CalcParameter(ref ItemParameterSerializableData itemParameter, RealityEnum.Reality reality)
        {
            if (itemParameter?.hp != null)
                itemParameter.hp = (int) (itemParameter.hp * ParameterRate(reality));
            if (itemParameter?.mp != null)
                itemParameter.mp = (int) (itemParameter.mp * ParameterRate(reality));
            if (itemParameter?.atk != null)
                itemParameter.atk = (int) (itemParameter.atk * ParameterRate(reality));
            if (itemParameter?.def != null)
                itemParameter.def = (int) (itemParameter.def * ParameterRate(reality));
            if (itemParameter?.mat != null)
                itemParameter.mat = (int) (itemParameter.mat * ParameterRate(reality));
            if (itemParameter?.mdf != null)
                itemParameter.mdf = (int) (itemParameter.mdf * ParameterRate(reality));
            if (itemParameter?.spd != null)
                itemParameter.spd = (int) (itemParameter.spd * ParameterRate(reality));
            if (itemParameter?.potentialHp != null)
                itemParameter.potentialHp = (int) (itemParameter.potentialHp * PotentialRate(reality));
            if (itemParameter?.potentialMp != null)
                itemParameter.potentialMp = (int) (itemParameter.potentialMp * PotentialRate(reality));
            if (itemParameter?.potentialAtk != null)
                itemParameter.potentialAtk = (int) (itemParameter.potentialAtk * PotentialRate(reality));
            if (itemParameter?.potentialDef != null)
                itemParameter.potentialDef = (int) (itemParameter.potentialDef * PotentialRate(reality));
            if (itemParameter?.potentialMat != null)
                itemParameter.potentialMat = (int) (itemParameter.potentialMat * PotentialRate(reality));
            if (itemParameter?.potentialMdf != null)
                itemParameter.potentialMdf = (int) (itemParameter.potentialMdf * PotentialRate(reality));
            if (itemParameter?.potentialSpd != null)
                itemParameter.potentialSpd = (int) (itemParameter.potentialSpd * PotentialRate(reality));
        }
        
        private static float ParameterRate(RealityEnum.Reality reality)
        {
            switch (reality)
            {
                case RealityEnum.Reality.Common:
                    return 1;
                case RealityEnum.Reality.UnCommon:
                    return 1.2f;
                case RealityEnum.Reality.Rare:
                    return 1.4f;
                case RealityEnum.Reality.Magic:
                    return 1.6f;
                case RealityEnum.Reality.Ancient:
                    return 2f;
                case RealityEnum.Reality.Epic:
                    return 2.8f;
                case RealityEnum.Reality.Miracle:
                    return 3.5f;
                case RealityEnum.Reality.Legend:
                    return 5f;
                case RealityEnum.Reality.God:
                    return 20f;
            }
            return 1;
        }
        
        private static float PotentialRate(RealityEnum.Reality reality)
        {
            switch (reality)
            {
                case RealityEnum.Reality.Common:
                    return 1;
                case RealityEnum.Reality.UnCommon:
                    return 1.1f;
                case RealityEnum.Reality.Rare:
                    return 1.2f;
                case RealityEnum.Reality.Magic:
                    return 1.4f;
                case RealityEnum.Reality.Ancient:
                    return 1.6f;
                case RealityEnum.Reality.Epic:
                    return 1.8f;
                case RealityEnum.Reality.Miracle:
                    return 2f;
                case RealityEnum.Reality.Legend:
                    return 2.5f;
                case RealityEnum.Reality.God:
                    return 4f;
            }
            return 1;
        }
    }
}