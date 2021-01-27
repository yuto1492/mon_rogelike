﻿using System;
using System.Collections.Generic;
using System.Text;
using Consts;
using Dictionary;
using Serializable;
using UnityEngine;

namespace Battle.Logic
{

    public class SkillLogic
    {

        public static int GetDamage(SkillSerializableEffectDamageRates skillRate, BattlerSerializable battler)
        {
            float val = 0;
            float rate = skillRate.rate;
            switch (skillRate.dependence)
            {
                case SkillConsts.ATK:
                    val = battler.parameter.atk * rate;
                    break;
                case SkillConsts.DEF:
                    val = battler.parameter.def * rate;
                    break;
                case SkillConsts.MAT:
                    val = battler.parameter.mat * rate;
                    break;
                case SkillConsts.MDF:
                    val = battler.parameter.mdf * rate;
                    break;
                case SkillConsts.SPD:
                    val = battler.parameter.spd * rate;
                    break;
                case SkillConsts.HP:
                    val = battler.parameter.hp * rate;
                    break;
                case SkillConsts.MP:
                    val = battler.parameter.mp * rate;
                    break;
                case SkillConsts.MAX_HP:
                    val = battler.parameter.maxHp * rate;
                    break;
                case SkillConsts.MAX_MP:
                    val = battler.parameter.maxMp * rate;
                    break;
                case SkillConsts.EFC:
                    val = battler.parameter.efc * rate;
                    break;
            }

            if (skillRate.addValue != 0)
            {
                val += skillRate.addValue;
            }
            return (int) val;
        }

        public static int GetTotalDamageByTarget(SkillSerializableEffectDamage effectDamage,BattlerSerializable battler)
        {
            int totalDamage = 0;
            effectDamage.rates.ForEach(x =>
            {
                totalDamage += GetDamage(x, battler);
            });
            return totalDamage;
        }

        /// <summary>
        /// スキル説明テキストの作成
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="battler"></param>
        /// <returns></returns>
        public static string CreateSkillDescription(string skillId, BattlerSerializable battler)
        {
            var skill = SkillsDicionary.GetSkillById(skillId);
            StringBuilder sb = new StringBuilder();
            string name = skill.name;
            sb.Append("<b><size=18>");
            sb.Append(name);
            sb.Append("</size></b>");
            sb.Append("\n--------------------\n");
            sb.Append(skill.description);
            sb.Append("\n--------------------\n");
            sb.Append(SkillsDicionary.SkillTargetText(skill.target));
            if (skill.strikeSize.min == 0 && skill.strikeSize.max == 0)
            {
                //敵単体(敵全体)に
                sb.Append("に\n");
            }
            else
            {
                //敵単体をx回から
                sb.Append("を");
                if (skill.strikeSize.min == skill.strikeSize.max)
                    sb.Append(skill.strikeSize.min).Append("回\n");
                else
                    sb.Append(skill.strikeSize.min).Append("回から").Append(skill.strikeSize.max).Append("回\n");
            }
            skill.effect.value.ForEach(item =>
            {
                item.rates.ForEach(rate =>
                {
                    StringBuilder msb = new StringBuilder();
                    msb.Append(SkillsDicionary.SkillDependenceText(rate.dependence)).Append("(").Append(rate.rate * 100)
                        .Append("%)\n").Append(SkillsDicionary.SkillDamageDescriptionTargetText(item.valueTarget)).Append("\n");
                    sb.Append(msb);
                });
            });
            //属性
            sb.Append("\n--------------------\n");
            skill.effect.elements.ForEach(x =>
            {
                sb.Append(SkillsDicionary.SkillElementsText(x)).Append("\n");
            });
            return sb.ToString();
        }

    }
}