using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Utils
{
    public class WeightChoice<T>
    {
        struct ChoiceStruct
        {
            public float Weight;
            public T Item;
        }

        private List<ChoiceStruct> _choiceStruct;

        public WeightChoice()
        {
            _choiceStruct = new List<ChoiceStruct>();
        }
        
        public void Add(float weight,T item)
        {
            var choiceStruct = new ChoiceStruct();
            choiceStruct.Weight = weight;
            choiceStruct.Item = item;
            _choiceStruct.Add(choiceStruct);
        }

        private ChoiceStruct Choice()
        {
            float totalWeight = 0;
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
        /// 一つを選択する
        /// </summary>
        /// <returns></returns>
        public T ChoiceOne()
        {
            return Choice().Item;
        }

        /// <summary>
        /// 一つを選択し、重みを１減らす
        /// </summary>
        /// <returns></returns>
        public T ChoicePickOut()
        {
            var choice = Choice();
            choice.Weight--;
            return choice.Item;
        }

        public int Count
        {
            get => _choiceStruct.Count;
        }
    }
}