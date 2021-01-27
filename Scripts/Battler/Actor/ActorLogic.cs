using System;
using Consts.Enums;
using Dictionary;
using Model;
using Serializable;

namespace Battler.Actor
{
    public class ActorLogic
    {
        public static BattlerSerializable Create(string monsterId, int level = 1)
        {
            string name = MonsterDicionary.GetMonsterData(monsterId).data.name;
            return Create(monsterId, name, level);
        }

        public static BattlerSerializable Create(string monsterId, string name, int level = 1)
        {
            var actor = BattlerLogic.Create(monsterId, level);
            actor.battlerType = BattlerEnum.BattlerType.Actor;
            actor.needExp = CalcNeedExp(monsterId, level);
            actor.hasExp = 0;
            actor.name = name;
            ActorDataModel actorData = ActorDataModel.Instance;
            actorData.Add(actor);
            return actor;
        }

        public static int AddExp(int uniqId, int addExp)
        {
            var levelUpSize = 0;
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            battler.hasExp += addExp;
            while (true)
            {
                if (battler.hasExp >= battler.needExp)
                {
                    battler.hasExp -= battler.needExp;
                    levelUpSize++;
                    battler.level++;
                    battler.needExp = CalcNeedExp(battler.monsterId, battler.level);
                }
                else
                    break;
            }
            return levelUpSize;
        }

        public static int VirtualLevelUpSize(int uniqId, int addExp)
        {
            var levelUpSize = 0;
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var virtualHasExp = battler.hasExp; 
            virtualHasExp += addExp;
            var virtualNeedExp = battler.needExp;
            var virtualLevel = battler.level;
            
            while (true)
            {
                if (virtualHasExp >= virtualNeedExp)
                {
                    virtualHasExp -= virtualNeedExp;
                    levelUpSize++;
                    virtualLevel++;
                    virtualNeedExp = CalcNeedExp(battler.monsterId, virtualLevel);
                }
                else
                    break;
            }
            return levelUpSize;
        }
        
        public static int CalcNeedExp(string monsterId, int level)
        {
            return (int)Math.Pow(MonsterDicionary.GetMonsterData(monsterId).data.needExp * level, (float) level / 10);
        }
        
    }
}