using System.Collections.Generic;
using Consts;
using Consts.Dictionary;
using Consts.Enums;
using Dictionary;
using Model;
using Serializable;

namespace Battler
{
    public class BattlerLogic
    {
        /// <summary>
        /// バトラーを作成する
        /// </summary>
        /// <param name="monsterId"></param>
        /// <param name="level"></param>
        public static BattlerSerializable Create(string monsterId,int level = 1)
        {
            MonsterSerializable monster = MonsterDicionary.GetMonsterData(monsterId);
            BattlerSerializable battler = new BattlerSerializable();
            battler.level = level;
            battler.monsterId = monsterId;
            battler.name = monster.data.name;
            battler.potential = monster.potential;
            battler.parameter = CalcParameter(monster, level);
            LevelUp(level, battler);
            battler.learning = new List<string>();
            battler.learning = SkillLearning(battler);
            battler.skills = new List<string>();
            battler.skills = AllSkillEquipment(battler);
            battler.resistRate = new BattlerSerializableResistRate();
            battler.status = new List<BattlerSerializableStatus>();
            battler.ai = monster.ai;
            battler.uniqId = CreateUniqId();

            return battler;
        }
        
        /// <summary>
        /// 一括でのレベルアップ処理
        /// </summary>
        /// <param name="amount"></param>
        public static void LevelUp(int amount)
        {
            var actors = MemberDataModel.Instance.GetActorData();
            actors.ForEach(x =>
            {
                LevelUp(amount, x);
            });
        }

        /// <summary>
        /// 指定したバトラーのレベルアップ処理
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="battler"></param>
        private static void LevelUp(int amount, BattlerSerializable battler)
        {
            battler.level += amount;
            var hp = (int) (GetPotential(StatusTypeEnum.StatusType.HP, battler.potential.hp) * amount);
            battler.parameter.maxHp += hp;
            battler.parameter.hp += hp;
            var mp = (int) (GetPotential(StatusTypeEnum.StatusType.MP, battler.potential.mp) * amount);
            battler.parameter.maxMp += mp;
            battler.parameter.mp += mp;
            battler.parameter.atk +=
                (int) (GetPotential(StatusTypeEnum.StatusType.ATK, battler.potential.atk) * amount);
            battler.parameter.def +=
                (int) (GetPotential(StatusTypeEnum.StatusType.DEF, battler.potential.def) * amount);
            battler.parameter.mat +=
                (int) (GetPotential(StatusTypeEnum.StatusType.MAT, battler.potential.mat) * amount);
            battler.parameter.mdf +=
                (int) (GetPotential(StatusTypeEnum.StatusType.MDF, battler.potential.mdf) * amount);
            battler.parameter.spd +=
                (int) (GetPotential(StatusTypeEnum.StatusType.SPD, battler.potential.spd) * amount);
        }

        private static float GetPotential(StatusTypeEnum.StatusType type, int rank)
        {
            switch (type)
            {
                case StatusTypeEnum.StatusType.HP:
                    return PotentialDictionary.Hp[rank];
                case StatusTypeEnum.StatusType.MP:
                    return PotentialDictionary.Mp[rank];
                case StatusTypeEnum.StatusType.SPD:
                    return PotentialDictionary.Spd[rank];
                default:
                    return PotentialDictionary.Param[rank];
            }
        }

        /// <summary>
        /// バトラーのポテンシャルとレベルからバトラーのパラメーターを計算する
        /// </summary>
        /// <returns></returns>
        private static BattlerSerializableParameter CalcParameter(MonsterSerializable monster, int level)
        {
            var battlerParameter = monster.parameter;
            BattlerSerializableParameter parameter = new BattlerSerializableParameter();
            parameter.maxHp = monster.parameter.hp;
            parameter.maxMp = monster.parameter.mp;
            parameter.atk = monster.parameter.atk;
            parameter.def = monster.parameter.def;
            parameter.mat = monster.parameter.mat;
            parameter.mdf = monster.parameter.mdf;
            parameter.spd = monster.parameter.spd;
            //Crit等ポテンシャルに左右されないパラメーター(今後左右されるかも) TODO ハードコード
            parameter.crit = 5;
            parameter.dodge = 5;
            parameter.magicDodge = 0;
            parameter.hit = 100;
            parameter.peneAtk = 0;
            parameter.peneMat = 0;
            parameter.dropRate = 0;
            parameter.goldRate = 0;
            parameter.expRate = 0;
            parameter.hostile = battlerParameter.hostile;
            //状態異常的中
            parameter.efc = battlerParameter.efc;
            if (parameter.efc == 0)
                parameter.efc = 50;
            //状態異常抵抗
            parameter.res = battlerParameter.res;
            if (parameter.res == 0)
                parameter.res = 25;
            if (parameter.hostile == 0)
                parameter.hostile = 100;
            return parameter;
        }
        
        /// <summary>
        /// スキルの習得
        /// </summary>
        /// <param name="battler"></param>
        /// <returns></returns>
        public static List<string> SkillLearning(BattlerSerializable battler)
        {
            var monster = MonsterDicionary.GetMonsterData(battler.monsterId);
            int level = battler.level;
            if (battler.learning.Contains(SkillIds.ATTACK) == false)
            {
                battler.learning.Add(SkillIds.ATTACK);
            }
            monster.learning.ForEach(x =>
            {
                if (level >= x.learnLevel)
                {
                    if (battler.learning.Contains(x.skillId) == false)
                    {
                        battler.learning.Add(x.skillId);
                    }
                }
            });
            return battler.learning;
        }
        
        /// <summary>
        /// 覚えているスキルをすべて装備する
        /// TODO 仕様がふわふわ
        /// </summary>
        /// <param name="battler"></param>
        /// <returns></returns>
        public static List<string> AllSkillEquipment(BattlerSerializable battler)
        {
            battler.learning.ForEach(x =>
            {
                battler.skills.Add(x);
            });
            return battler.skills;
        }

        /// <summary>
        /// バトラーを識別するユニークIDを作成する
        /// TODO もっとマシな生成方法を考える
        /// </summary>
        /// <returns></returns>
        public static int CreateUniqId()
        {
            return ActorDataModel.Instance.Data.Count + EnemyDataModel.Instance.Data.Count;
        }
        
    }
}