using System;
using System.Collections.Generic;
using Consts;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Serializable
{
    [Serializable]
    public class DungeonSerializable
    {
        public DungeonSerializableData data;
        public List<DungeonEnemiesSerializableData> enemies;
        public List<DungeonEliteEnemiesSerializableData> eliteEnemies;
        public DungeonBossesEnemiesSerializableData bosses;
        public DungeonSerializableMapData map;
        public List<DungeonSerializableEnemyAmount> enemyAmount;
        public DungeonSerializableEnemyLevel enemyLevel;
        public List<DropSerializableData> drops;
        public List<DropSerializableData> bossDrops;
        public List<DropSerializableData> keyDrops;
        public int dropPower;
    }

    [Serializable]
    public class DungeonEnemiesSerializableData
    {
        public int border = 0;
        public List<string> monsters;
        public int addLevel = 0;
    }

    [Serializable]
    public class DungeonEliteEnemiesSerializableData
    {
        public int border = 0;
        public List<List<string>> monsters;
        public int addLevel = 0;
    }

    [Serializable]
    public class DungeonBossesEnemiesSerializableData
    {
        public List<string> monsters;
        public int addLevel = 0;
    }
    
    [Serializable]
    public class DungeonSerializableData
    {
        public string name;
        public string id;
    }

    [Serializable]
    public class DungeonSerializableEnemyAmount
    {
        public int border = 0;
        public int min;
        public int max;
    }

    [Serializable]
    public class DungeonSerializableEnemyLevel
    {
        public int min;
        public int max;
        public float addingLevelPerFloor;
    }

    [Serializable]
    public class DungeonSerializableMapData
    {
        public int battle;
        public int elite;
        public int evt;
        public int treasure;
        public int boss;
    }

}