using System;
using System.Collections.Generic;
using Consts.Enums;

namespace Serializable
{
    [Serializable]
    public class ItemSerializableList
    {
        public List<ItemSerializable> list;
    }
    
    [Serializable]
    public class ItemSerializable
    {
        /// <summary>
        /// 取得順
        /// </summary>
        public ItemSerializableData data;
        public ItemSpriteSerializableData imageData;
        public ItemEffectSerializableData effect;
        public ItemParameterSerializableData parameter;
    }

    [Serializable]
    public class ItemSerializableData
    {
        public string name;
        public string itemId;
        public string type;
    }

    [Serializable]
    public class ItemParameterSerializableData
    {
        public int hp;
        public int mp;
        public int atk;
        public int def;
        public int mat;
        public int mdf;
        public int spd;
        public int crit;
        public int hit;
        public int dodge;
        public int hostile;
        
        public float potentialHp;
        public float potentialMp;
        public float potentialAtk;
        public float potentialDef;
        public float potentialMat;
        public float potentialMdf;
        public float potentialSpd;
    }
    
    [Serializable]
    public class ItemEffectSerializableData
    {
        public int regenHp;
        public int regenMp;

        public int dropRate;
        public int boostBlow;
        public int boostSlash;
        public int boostFire;
        public int boostIce;
        public int boostThunder;

        public int effectAttackFire;
        public int effectAttackIce;
        public string monsterId;
    }
    
    [Serializable]
    public class ItemSpriteSerializableData
    {
        public string spritePath;
    }
    
}