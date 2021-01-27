using System.Collections.Generic;
using Serializable;

namespace Model.Master
{
    public class MasterDungeonDataModel
    {
        private static MasterDungeonDataModel _instance = new MasterDungeonDataModel();
        public static MasterDungeonDataModel Instance => _instance;
        
        private List<DungeonSerializable> _data;

        public void CreateData(List<DungeonSerializable> data)
        {
            _data = data;
        }

        public List<DungeonSerializable> Data => _data;

        
    }
}