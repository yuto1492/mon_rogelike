using Battle.AI.Logic;
using Consts;
using Dictionary;
using Serializable;
using UniRx;
using UnityEngine;
using Utils;

namespace Battle.AI.Scripts
{
    public class AiNormal : IAiInterface
    {
        
        public AiSelectSkillResultSerializableData SelectSkill(int uniqId)
        {
            BattlerSerializable battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            //スキルを無造作に選ぶ、アタックの重みは少ない
            var choice = new WeightChoice<string>();
            foreach (var x in battler.skills)
            {
                var _skill = SkillsDicionary.GetSkillById(x);
                if (_skill.skillId == SkillIds.ATTACK)
                {
                    choice.Add(SkillWeights.VERY_LOW,x);
                }
                else if(battler.parameter.mp >= _skill.mp)
                {
                    choice.Add(SkillWeights.NORMAL,x);
                }
            }
            
            AiSelectSkillResultSerializableData result = new AiSelectSkillResultSerializableData();
            var skill = SkillsDicionary.GetSkillById(choice.ChoiceOne());
            if (battler.parameter.mp >= skill.mp)
            {
                switch (skill.target)
                {
                    case SkillConsts.ENEMY:
                    case SkillConsts.MEMBER:
                        result.TargetUniqIds.Add(
                            AiLogic.ChoiceBattlerByHostile(BattleDictionary.GetAliveRivalBattlers(battler.battlerType)));
                        break;
                    case SkillConsts.ALL_ENEMY:
                    case SkillConsts.ALL_MEMBER:
                        result.TargetUniqIds = BattleDictionary.GetRivalUniqIds(battler.battlerType);
                        break;
                }
                result.SkillId = skill.skillId;
            }

            return result;
        }
    }
}