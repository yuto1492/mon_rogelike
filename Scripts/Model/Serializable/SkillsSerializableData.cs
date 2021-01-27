using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serializable
{
    [Serializable]
    public class SkillsSerializableList
    {
        public List<SkillsSerializable> list;
    }
    
    [Serializable]
    public class SkillsSerializable
    {
        public string skillId;
        public string name;
        public SkillSerializableEffect effect;
        public string description;
        public string iconSpritePath;
        public string effectAnimationId;
        public string damageColor;
        public SkillSerializableStrikeSize strikeSize;
        public int mp;
        public int coolTime;
        public string target;
    }

    [Serializable]
    public class SkillSerializableEffect
    {
        public List<SkillSerializableEffectDamage> value;
        public string type;
        public List<string> elements;
        public float hitRate = 100;
    }

    [Serializable]
    public class SkillSerializableEffectDamage
    {
        public string valueTarget;
        public List<SkillSerializableEffectDamageRates> rates;
        public int turn;
    }

    [Serializable]
    public class SkillSerializableEffectDamageRates
    {
        public string dependence;
        public float rate;
        public int addValue;
    }

    [Serializable]
    public class SkillSerializableStrikeSize
    {
        public int min;
        public int max;
    }
    
    [Serializable]
    public class SkillDamage
    {
        public int damage;
        public string valueTarget;
        public int turn;
        public bool isResist;
    }

    [Serializable]
    public class SkillDamages
    {
        public bool isHit;
        public List<SkillDamage> SkillDamage;
        public int targetUniqId;
        public bool isDead;
    }

}