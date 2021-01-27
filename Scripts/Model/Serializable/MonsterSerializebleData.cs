using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serializable
{
    [Serializable]
    public class MonsterSerializable
    {
        public MonsterSerializableData data;
        public MonsterSerializableParameter parameter;
        public MonsterSerializablePotential potential;
        public MonsterSerializableBoost boost;
        public MonsterSerializableResistRate resistRate;
        public List<MonsterSerializableLearning> learning;
        public MonsterSerializableImage imageData;
        public List<DropSerializableData> drops;
        public string ai;
    }

    [Serializable]
    public class MonsterSerializableParameter
    {
        public int hp;
        public int mp;
        public int atk;
        public int def;
        public int mat;
        public int mdf;
        public int spd;
        public int hostile;
        public int efc;    //Effective
        public int res;    //Resist
    }

    [Serializable]
    public class MonsterSerializableData
    {
        public string monsterId;
        public string name;
        public string race;
        public int rank;
        public int needExp;
        public int hasExp;
    }

    [Serializable]
    public class MonsterSerializablePotential
    {
        public int hp;
        public int mp;
        public int atk;
        public int def;
        public int mat;
        public int mdf;
        public int spd;
    }

    [Serializable]
    public class MonsterSerializableBoost
    {
        public float fire;
        public float ice;
        public float thunder;
        public float earth;
        public float poison;
        public float blow;
        public float slash;
        public float pierce;
        public float physics;
        public float magic;
        public float bless;
    }

    [Serializable]
    public class MonsterSerializableResistRate
    {
        public float fire = 0;
        public float ice = 0;
        public float thunder = 0;
        public float earth = 0;
        public float poison = 0;
        public float sleep = 0;
        public float fireBurn= 0;
        public float frostbite= 0;
        public float electricShock= 0;
        public float corruption= 0;
        public float stun= 0;
        public float stagger= 0;
    }

    [Serializable]
    public class MonsterSerializableLearning
    {
        public string skillId;
        public int learnLevel;
    }

    [Serializable]
    public class MonsterSerializableImage
    {
        public string spritePath;
        public MonsterSerializableImageTimelineCard timelineCard;
        public float actorScale;
    }

    [Serializable]
    public class MonsterSerializableImageTimelineCard
    {
        public float x;
        public float y;
        public float scaleX;
        public float scaleY;
    }
}