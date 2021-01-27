using System.Collections.Generic;
using Serializable;
using UnityEngine;
using System.Linq;
using Serializable;
using UniRx;

namespace Model.Master
{
    public sealed class MasterMonsterDataModel
    {
        private static MasterMonsterDataModel _instance = new MasterMonsterDataModel();

        public static MasterMonsterDataModel Instance => _instance;

        private List<MonsterSerializable> _data;
        public void CreateData(List<MonsterSerializable> data)
        {
            _data = data;
        }
        
        public List<MonsterSerializable> Data => _data;
    }
}