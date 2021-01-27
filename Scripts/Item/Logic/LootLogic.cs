using System.Collections.Generic;
using Consts.Enums;
using Dictionary;
using Dungeon;
using Serializable;
using Structs;
using UnityEngine;
using Utils;

namespace Item.Logic
{
    /// <summary>
    /// アイテムのルート抽選関連の処理をするクラス
    /// </summary>
    public class LootLogic
    {
        /// <summary>
        /// モンスターからドロップを抽選する
        /// </summary>
        /// <param name="monsterId"></param>
        /// <returns></returns>
        public static List<LootItemStruct> MonsterLootChoice(string monsterId)
        {
            var monsterData = MonsterDicionary.GetMonsterData(monsterId);
            //ドロップアイテムを抽選する
            var drops = DropLottery(monsterData.drops);
            return drops;
        }

        /// <summary>
        /// ダンジョン固有アイテムからドロップを抽選する
        /// </summary>
        /// <returns></returns>
        public static List<LootItemStruct> DungeonLootChoice(float rate = 1)
        {
            //ダンジョン固有アイテムも抽選する
            var dungeonData = DungeonDictionary.GetDungeonMapData(DungeonDataModel.Instance.DungeonId);
            var drops = DropLottery(dungeonData.drops, rate);
            return drops;
        }

        /// <summary>
        /// ダンジョンからドロップを抽選する
        /// </summary>
        /// <returns></returns>
        public static List<LootItemStruct> DungeonTreasureLootChoice()
        {
            var dungeonData = DungeonDictionary.GetDungeonMapData(DungeonDataModel.Instance.DungeonId);
            
            //ドロップアイテムを抽選する
            var drops = DropLottery(dungeonData.drops);
            //最低一個は抽選する
            var choice = new WeightChoice<string>();
            dungeonData.drops.ForEach(drop =>
            {
                choice.Add(drop.rate, drop.itemId);
            });
            var itemId = choice.ChoiceOne();
            drops.Add(new LootItemStruct()
            {
                ItemId = itemId,
                Amount = 1
            });
            //レアリティの抽選を行う
            drops.ForEach(drop =>
            {
                DropLottery(ref drop);
            });
            return drops;
        }

        
        /// <summary>
        /// ドロップアイテムの抽選
        /// </summary>
        /// <param name="dropsData"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        private static List<LootItemStruct> DropLottery(List<DropSerializableData> dropsData, float rate = 1)
        {
            List<LootItemStruct> drops = new List<LootItemStruct>();
            //個別抽選、抽選に外れるまでループ抽選
            dropsData.ForEach(drop =>
            {
                while (true)
                {
                    if (LotteryOne(drop, rate))
                    {
                        drops.Add(new LootItemStruct()
                        {
                            ItemId = drop.itemId
                        });
                    }
                    else
                        break;
                }
            });
            return drops;
        }

        /// <summary>
        /// 単体の抽選
        /// </summary>
        /// <param name="dropsData"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        private static bool LotteryOne(DropSerializableData dropsData, float rate = 1)
        {
            if (Random.Range(0f, 100.0f) < dropsData.rate * rate)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// レアリティ抽選
        /// </summary>
        /// <param name="loot"></param>
        private static void DropLottery(ref LootItemStruct loot)
        {
            var choice = new WeightChoice<RealityEnum.Reality>();
            choice.Add(2000, RealityEnum.Reality.Common);
            choice.Add(1700, RealityEnum.Reality.UnCommon);
            choice.Add(1200, RealityEnum.Reality.Rare);
            choice.Add(1000, RealityEnum.Reality.Magic);
            choice.Add(500, RealityEnum.Reality.Ancient);
            choice.Add(250, RealityEnum.Reality.Epic);
            choice.Add(100, RealityEnum.Reality.Miracle);
            choice.Add(50, RealityEnum.Reality.Legend);
            choice.Add(1, RealityEnum.Reality.God);
            loot.Reality = choice.ChoiceOne();
        }

    }
}