using System.Collections.Generic;
using Serializable;

namespace Model
{
    /// <summary>
    /// 待機中含む全仲間データ
    /// </summary>
    public sealed class ActorDataModel
    {
        private static ActorDataModel _instance = new ActorDataModel();
        
        private List<BattlerSerializable> _data = new List<BattlerSerializable>();

        public BattlerSerializable GetActorData(int uniqId)
        {
            return _data.Find(x => x.uniqId == uniqId);
        }
        
        public int GetLastIndex()
        {
            return _data.Count;
        }

        public void Add(BattlerSerializable actorData)
        {
            actorData.index = _data.Count;
            _data.Add(actorData);
        }
        
        public BattlerSerializable ByIndex(int index)
        {
            return _data.Find(x => x.index == index);            
        }
        
        public List<BattlerSerializable> Data
        {
            get => _data;
            set => _data = value;
        }

        public static ActorDataModel Instance => _instance;
    }
}