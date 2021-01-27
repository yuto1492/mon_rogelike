using System.Collections.Generic;
using Consts.Enums;
using UniRx;
using UnityEngine;

namespace Dungeon
{
    public class DungeonDataModel
    {
        private static DungeonDataModel _instance = new DungeonDataModel();

        public Vector3Int Location;
        public string DungeonId;
        public int DungeonFloorSize;
        public int Floor = 0;

        public Subject<Vector3Int> DungeonFloorSelectSubject;
        public bool IsEvent = false;
        
        //public DungeonEnum.Tiles[][] DungeonFloor;
        public struct DungeonFloorStruct
        {
            public DungeonEnum.Tiles Tile;
            public bool Enabled;
        };
        public DungeonFloorStruct[][] DungeonFloor;

        public static DungeonDataModel Instance => _instance;
    }
}