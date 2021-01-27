using System.Collections.Generic;
using Battle;
using Consts;
using Consts.Enums;
using Model;
using Serializable;
using UnityEngine;
using Utils;

namespace Dictionary
{
    public class BattleDictionary
    {
        
        /// <summary>
        /// タイムラインデータシリアライズリストからIDでタイムラインシリアライズを取得
        /// </summary>
        /// <param name="timeline"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static TimelineSerializableData GetTimelineById(List<TimelineSerializableData> timeline, int id)
        {
            return timeline.Find(x => x.id == id);
        }
        
        public static Transform GetTransformByUniqId(int uniqId)
        {
            return BattlePresenter.GetInstance().BattlerSpriteView.GetSprite(uniqId).SpriteObject.transform;
        }

        public static List<BattlerSerializable> GetAllAliveBattlers()
        {
            List<BattlerSerializable> battlers = new List<BattlerSerializable>();
            battlers.AddRange(MemberDataModel.Instance.GetActorData());
            battlers.AddRange(EnemyDataModel.Instance.Data);
            return battlers;
        }
        
        /// <summary>
        /// 対象の中からランダムで生きているのを返す
        /// 敵対値が考慮される
        /// </summary>
        /// <returns></returns>
        public static BattlerSerializable GetAliveBattlerByRandom(List<BattlerSerializable> battlers)
        {
            WeightChoice<BattlerSerializable> weightChoice = new WeightChoice<BattlerSerializable>();
            battlers.ForEach(battler =>
            {
                if (BattlerDictionary.IsDeadByUniqId(battler.uniqId) == false)
                {
                    weightChoice.Add(battler.parameter.hostile,battler);
                }
            });
            if (weightChoice.Count == 0)
            {
                return null;
            }
            return weightChoice.ChoiceOne();
        }

        public static List<BattlerSerializable> GetMemberBattlers(BattlerEnum.BattlerType battlerType)
        {
            if (battlerType == BattlerEnum.BattlerType.Actor)
            {
                return MemberDataModel.Instance.GetActorData();
            }

            return EnemyDataModel.Instance.Data;
        }
        
        public static List<BattlerSerializable> GetRivalBattlers(BattlerEnum.BattlerType battlerType)
        {
            if (battlerType == BattlerEnum.BattlerType.Actor)
            {
                return EnemyDataModel.Instance.Data;
            }

            return MemberDataModel.Instance.GetActorData();
        }

        public static List<BattlerSerializable> GetAliveRivalBattlers(BattlerEnum.BattlerType battlerType)
        {
            var battlers = GetRivalBattlers(battlerType);
            battlers.RemoveAll(x => BattlerDictionary.IsDead(x.uniqId));
            return battlers;
        }
        
        public static List<int> GetRivalUniqIds(BattlerEnum.BattlerType battlerType)
        {
            if (battlerType == BattlerEnum.BattlerType.Actor)
            {
                return EnemyDataModel.Instance.UniqIds();
            }

            return MemberDataModel.Instance.UniqIds();
        }

        public static List<int> GetMemberUniqIds(BattlerEnum.BattlerType battlerType)
        {
            if (battlerType == BattlerEnum.BattlerType.Actor)
            {
                return MemberDataModel.Instance.UniqIds();
            }

            return EnemyDataModel.Instance.UniqIds();
        }

        public static bool IsActor(int uniqId)
        {
            if (BattlerDictionary.GetBattlerByUniqId(uniqId).battlerType == BattlerEnum.BattlerType.Actor)
            {
                return true;
            }

            return false;
        }

        public static bool IsAppropriate(string skillId, int fromUniqId, int toUniqId)
        {
            var skill = SkillsDicionary.GetSkillById(skillId);
            var fromBattler = BattlerDictionary.GetBattlerByUniqId(fromUniqId);
            var toBattler = BattlerDictionary.GetBattlerByUniqId(toUniqId);
            if ((skill.target == SkillConsts.ENEMY || skill.target == SkillConsts.ALL_ENEMY ||
                 skill.target == SkillConsts.RANDOM_ENEMY) && toBattler.battlerType != fromBattler.battlerType)
            {
                return true;
            }

            if ((skill.target == SkillConsts.MEMBER || skill.target == SkillConsts.ALL_MEMBER ||
                 skill.target == SkillConsts.RANDOM_MEMBER) && toBattler.battlerType == fromBattler.battlerType)
            {
                return true;
            }

            return false;
        }


    }
}