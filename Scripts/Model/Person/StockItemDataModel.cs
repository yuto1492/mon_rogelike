using System.Collections.Generic;
using Model.Structs;
using Structs;

namespace Model.Person
{
    /// <summary>
    /// ダンジョン道中の一時的なアイテム保管モデル
    /// ダンジョン攻略時にインベントリに入る
    /// </summary>
    public sealed class StockItemDataModel
    {
        private static StockItemDataModel _singleton = new StockItemDataModel();
        
        public static StockItemDataModel GetInstance()
        {
            return _singleton;
        }
        
        private List<ItemStruct> _data = new List<ItemStruct>();
        private int _gold =
            0;
        public void AddGold(int amount)
        {
            _gold = amount;
        }
        
        public void AddItems(List<ItemStruct> items)
        {
            _data.AddRange(items);
        }
        
        public List<ItemStruct> Data
        {
            get => _data;
        }
    }
}