using System;
using System.Collections.Generic;
using System.Linq;
using Serializable;
using TMPro;
using Random = UnityEngine.Random;

namespace Battle.AI.Logic
{
    public class AiLogic
    {
        /// <summary>
        /// 対象のバトラーの中から敵対心からチョイスしUniqIdを返す
        /// </summary>
        /// <param name="toBattlers"></param>
        /// <returns></returns>
        public static int ChoiceBattlerByHostile(List<BattlerSerializable> toBattlers)
        {
            int weight = 0;
            toBattlers.ForEach(battler =>
            {
                weight += battler.parameter.hostile;
            });
            foreach (var battler in toBattlers)
            {
                int rnd = Random.Range(0, weight);
                if (rnd < battler.parameter.hostile)
                {
                    return battler.uniqId;
                }
                weight -= battler.parameter.hostile;
            }

            return toBattlers.First().uniqId;
        }
        
    }
}