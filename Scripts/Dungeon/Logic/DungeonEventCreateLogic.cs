using System.Collections.Generic;
using System.Linq;
using Consts.Enums;
using UnityEngine;

namespace Dungeon.Logic
{
    public class DungeonEventCreateLogic
    {
        struct ChoiceStruct
        {
            public int Weight;
            public DungeonEnum.Tiles Item;
        }
        
        private List<ChoiceStruct> _choiceStruct;
        private int _count = 0;

        public DungeonEventCreateLogic()
        {
            _choiceStruct = new List<ChoiceStruct>();
        }
        
        public void Add(int weight,DungeonEnum.Tiles item)
        {
            var choiceStruct = new ChoiceStruct();
            choiceStruct.Weight = weight;
            choiceStruct.Item = item;
            _choiceStruct.Add(choiceStruct);
        }

        private ChoiceStruct Choice()
        {
            //最初の一回は戦闘固定
            if (_count == 0)
            {
                return _choiceStruct.Find(x => x.Item == DungeonEnum.Tiles.Battle);
            }
            int totalWeight = 0;
            _choiceStruct.ForEach(x =>
            {
                totalWeight += x.Weight;
            });
            foreach (var x in _choiceStruct)
            {
                if (Random.Range(0, totalWeight) < x.Weight)
                {
                    return x;
                }

                totalWeight -= x.Weight;
            }
            return _choiceStruct.First();
        }
        
        /// <summary>
        /// 一つを選択し、重みを１減らす
        /// </summary>
        /// <returns></returns>
        public DungeonEnum.Tiles ChoicePickOut()
        {
            var choice = Choice();
            _count++;
            choice.Weight--;
            return choice.Item;
        }

        public int Count
        {
            get => _choiceStruct.Count;
        }
    }
}