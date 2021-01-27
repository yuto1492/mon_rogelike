using Consts;
using Extensions;
using Model;
using Model.Master;
using Serializable;

namespace Dictionary
{
    public class SkillsDicionary
    {
        public static SkillsSerializable GetSkillById(string id)
        {
            return MasterSkillDataModel.Instance.Data.Find(skill => skill.skillId == id);
        }

        public static SkillsSerializable GetEquippedSkillByIndex(BattlerSerializable battler,int index)
        {
            var eqSkills = battler.skills;
            if (IListExtensions.IsDefinedAt(eqSkills, index))
            {
                return GetSkillById(eqSkills[index]);
            }

            return null;
        }

        public static string SkillColor(string target)
        {
            switch (target)
            {
                case SkillValueTarget.HP:
                    return SkillConsts.COLOR_DAMAGE;
                case SkillValueTarget.MP:
                    return SkillConsts.COLOR_MP_DAMAGE;
                case SkillValueTarget.TARGET_POISON:
                    return SkillConsts.COLOR_POISON;
                case SkillValueTarget.TARGET_REGENERATION_HP:
                case SkillValueTarget.TARGET_HEAL_HP:
                    return SkillConsts.COLOR_HEAL;
            }

            return "#FFF";
        }

        public static string HitDamageColor(string target)
        {
            switch (target)
            {
                case SkillValueTarget.HP:
                    return SkillConsts.COLOR_DAMAGE;
                case SkillValueTarget.MP:
                    return SkillConsts.COLOR_MP_DAMAGE;
                case SkillValueTarget.TARGET_POISON:
                    return "#A2BE5F";
                case SkillValueTarget.TARGET_REGENERATION_HP:
                case SkillValueTarget.TARGET_HEAL_HP:
                    return SkillConsts.COLOR_HEAL;
            }

            return "#F00";
        }

        public static string SkillTargetText(string target)
        {
            switch (target)
            {
                case SkillConsts.ENEMY:
                    return "敵単体";
                case SkillConsts.MEMBER:
                    return "味方単体";
                case SkillConsts.ALL_ENEMY:
                    return "敵全体";
                case SkillConsts.ALL_MEMBER:
                    return "味方全体";
                case SkillConsts.RANDOM_ENEMY:
                    return "ランダムな敵";
                case SkillConsts.SELF:
                    return "使用者";
            }

            return "";
        }
        public static string SkillDependenceText(string dependence)
        {
            switch (dependence)
            {
                case SkillValueTarget.ATK:
                    return "物理攻撃力";
                case SkillValueTarget.DEF:
                    return "物理防御力";
                case SkillValueTarget.MAT:
                    return "魔法攻撃力";
                case SkillValueTarget.MDF:
                    return "魔法防御力";
                case SkillValueTarget.SPD:
                    return "速度";
                case SkillValueTarget.HP:
                    return "残りHP";
                case SkillValueTarget.MP:
                    return "残りMP";
                case SkillValueTarget.MAX_HP:
                    return "最大HP";
                case SkillValueTarget.MAX_MP:
                    return "最大MP";
                case SkillValueTarget.EFC:
                    return "効果的中率";
            }

            return "";
        }

        public static string SkillDamageDescriptionTargetText(string damageTarget)
        {
            switch (damageTarget)
            {
                case SkillValueTarget.HP:
                    return "HPダメージ";
                case SkillValueTarget.POISON:
                    return "毒ダメージ";
                case SkillValueTarget.SLEEP:
                    return "睡眠付与";
            }

            return "";
        }
        
        public static string SkillAnnounceValueTargetText(string damageTarget)
        {
            switch (damageTarget)
            {
                case SkillConsts.HP:
                    return "ダメージ";
                case SkillValueTarget.TARGET_POISON:
                    return "毒を追加";
                case SkillValueTarget.TARGET_REGENERATION_HP:
                    return "再生";
                case SkillValueTarget.TARGET_HEAL_HP:
                    return "回復";
                case SkillValueTarget.SLEEP:
                    return "眠った";
            }
            return "";
        }

        public static string SkillAnnounceStateMissText(string damageTarget)
        {
            switch (damageTarget)
            {
                case SkillValueTarget.SLEEP:
                    return "眠らなかった"; 
            }

            return "";
        }

        public static string SkillDamagePopUpText(SkillDamage damage)
        {
            switch (damage.valueTarget)
            {
                case SkillValueTarget.HP:
                case SkillValueTarget.MP:
                case SkillValueTarget.POISON:
                    return damage.damage.ToString();
                case SkillValueTarget.SLEEP:
                    if (damage.damage == 0)
                    {
                        return "ていこう";
                    }

                    return "すいみん";
            }

            return "";
        }

        public static string SkillElementsText(string effect)
        {
            switch (effect)
            {
                case SkillConsts.EFFECT_PHYSICS:
                    return "物理";
                case SkillConsts.EFFECT_MAGIC:
                    return "魔法";
                case SkillConsts.EFFECT_FIRE:
                    return "炎";
                case SkillConsts.EFFECT_ICE:
                    return "冷気";
                case SkillConsts.EFFECT_THUNDER:
                    return "雷";
                case SkillConsts.EFFECT_EARTH:
                    return "大地";
                case SkillConsts.EFFECT_WIND:
                    return "風";
                case SkillConsts.EFFECT_POISON:
                    return "毒";
                case SkillConsts.EFFECT_HEAL:
                    return "回復";
            }
            return "";
        }

        public static bool IsDamageSkill(string valueTarget)
        {
            switch (valueTarget)
            {
                case SkillValueTarget.SLEEP:
                    return false;
            }

            return true;
        }
        
        public static bool IsAll(SkillsSerializable skill)
        {
            switch (skill.target)
            {
                case SkillConsts.ALL_ENEMY:
                case SkillConsts.ALL_MEMBER:
                case SkillConsts.ALL:
                    return true;
            }

            return false;
        }

        public static bool IsRandom(SkillsSerializable skill)
        {
            switch (skill.target)
            {
                case SkillConsts.RANDOM_ENEMY:
                case SkillConsts.RANDOM_MEMBER:
                    return true;
            }

            return false;
        }

        public static bool IsSingle(SkillsSerializable skill)
        {
            switch (skill.target)
            {
                case SkillConsts.MEMBER:
                case SkillConsts.ENEMY:
                case SkillConsts.SELF:
                    return true;
            }

            return false;
        }

    }
}