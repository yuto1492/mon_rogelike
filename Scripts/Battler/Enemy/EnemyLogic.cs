using Consts.Enums;
using Serializable;
using UnityEngine;

namespace Battler.Enemy
{
    public class EnemyLogic
    {
        public static BattlerSerializable Create(string monsterId,int level = 1)
        {
            BattlerSerializable enemy = new BattlerSerializable();
            enemy = BattlerLogic.Create(monsterId, level);
            enemy.battlerType = BattlerEnum.BattlerType.Enemy;
            return enemy;
        }
    }
}