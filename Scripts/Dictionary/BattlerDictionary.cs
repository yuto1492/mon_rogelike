using Consts;
using Model;
using Serializable;

namespace Dictionary
{
    public class BattlerDictionary
    {

        public static BattlerSerializable GetBattlerByUniqId(int uniqId)
        {
            var actor = ActorDataModel.Instance.Data.Find(x => x.uniqId == uniqId);
            if (actor != null)
                return actor;
            var enemy = EnemyDataModel.Instance.Data.Find(x => x.uniqId == uniqId);
            if (enemy != null)
                return enemy;
            return null;
        }

        public static bool IsDead(int uniqId)
        {
            return IsDead(GetBattlerByUniqId(uniqId));
        }

        public static bool IsDead(BattlerSerializable battler)
        {
            if (battler.status.Find(x => x.type == StatusConsts.DEAD) != null)
                return true;
            return false;
        }

        /// <summary>
        /// ユニークIDから対象が死んでいるか確認する
        /// 死んでいる場合はTrue
        /// </summary>
        /// <param name="uniqId"></param>
        /// <returns></returns>
        public static bool IsDeadByUniqId(int uniqId)
        {
            var battler = GetBattlerByUniqId(uniqId);
            return IsDead(battler);
        }

        public static BattlerSerializableStatus GetStatus(BattlerSerializable battler, string type)
        {
            var status = battler.status.Find(x => x.type == type);
            if (status != null)
                return status;
            return null;
        }
        
    }
}