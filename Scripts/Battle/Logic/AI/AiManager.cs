using Battle.AI.Scripts;
using Consts;
using Dictionary;
using UniRx;
using Microsoft.CodeAnalysis;
using Serializable;
using UnityEngine;

namespace Battle.AI
{
    public class AiManager
    {
        public static AiSelectSkillResultSerializableData Action(int uniqId)
        {
            var batter = BattlerDictionary.GetBattlerByUniqId(uniqId);
            switch (batter.ai)
            {
                case AiConsts.AI_NORMAL:
                    var ai = new AiNormal();
                    return ai.SelectSkill(uniqId);
            }

            return new AiSelectSkillResultSerializableData();
        }
    }
}