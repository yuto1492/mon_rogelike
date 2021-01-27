using System.Collections.Generic;
using Consts;
using Dictionary;
using Model;
using Model.Structs;
using Serializable;
using Structs;

namespace Model.Person
{
    public sealed class InventoryDataModel
    {
        private static InventoryDataModel _singleton = new InventoryDataModel();
        
        public static InventoryDataModel GetInstance()
        {
            return _singleton;
        }

        private List<ItemStruct> _data = new List<ItemStruct>();
        private int _gold = 0;

        public void StartUp()
        {
        }

        public void AddGold(int amount)
        {
            _gold += amount;
        }
        
        public void AddItem(ItemStruct item)
        {
            _data.Add(item);
        }

        public List<ItemStruct> Data
        {
            get => _data;
        }

        public int Gold
        {
            get => _gold;
        }
    }
}