using System.Collections.Generic;
using Model;
using Serializable;

namespace Model.Master
{
    public sealed class MasterItemDataModel
    {
        private static MasterItemDataModel _instance = new MasterItemDataModel();
        public static MasterItemDataModel Instance => _instance;
        
        private List<ItemSerializable> _data;

        public void CreateData(List<ItemSerializable> data)
        {
            _data = data;
        }

        public List<ItemSerializable> Data => _data;

    }
}