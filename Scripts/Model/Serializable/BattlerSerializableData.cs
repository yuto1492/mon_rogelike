using System;
using System.Collections.Generic;
using Consts.Enums;
using UniRx;

namespace Serializable
{
    [Serializable]
    public class BattlerSerializable
    {
        public int uniqId;
        public string monsterId;
        public int level;
        /// <summary>
        /// 状態異常
        /// 数値とboolで管理する
        /// </summary>
        public List<BattlerSerializableStatus> status;
        public string name;
        public BattlerSerializableParameter parameter;
        /// <summary>
        /// 装備しているスキル
        /// 配列indexでindex管理
        /// </summary>
        public List<string> skills;
        /// <summary>
        /// 所持しているスキル
        /// </summary>
        public List<string> learning;
        public BattlerSerializableBoost boost;
        public BattlerSerializableResistRate resistRate;
        public MonsterSerializablePotential potential;

        public int index;
        public BattlerEnum.BattlerType battlerType;
        public string ai;

        public int hasExp;
        public int needExp;
    }

    [Serializable]
    public class BattlerSerializableStatus
    {
        /// <summary>
        /// 状態異常の種類
        /// </summary>
        public string type;
        public int value;
        public int turn;
    }

    [Serializable]
    public class BattlerSerializableParameter
    {
        public int maxHp;
        public int hp;
        public int maxMp;
        public int mp;
        public int atk;
        public int def;
        public int mat;
        public int mdf;
        public int spd;
        public int hit;
        public int dodge;
        public int magicDodge;
        public int crit;
        public float expRate;
        public float dropRate;
        public float goldRate;
        public float peneAtk;
        public float peneMat;
        public int hostile;
        public int efc;    //Effective
        public int res;    //Resist
    }
    
    [Serializable]
    public class BattlerSerializableBoost : MonsterSerializableBoost
    {
    }

    [Serializable]
    public class BattlerSerializableResistRate : MonsterSerializableResistRate
    {
    }

}