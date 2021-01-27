using System.Collections.Generic;
using Serializable;

namespace Model
{
    public class EnemyDataModel
    {
        private static EnemyDataModel _instance = new EnemyDataModel();

        private List<BattlerSerializable> _data = new List<BattlerSerializable>();

        public void Initialize()
        {
            _data.Clear();
        }

        /// <summary>
        /// 敵を追加
        /// </summary>
        /// <param name="enemyData"></param>
        public void Add(BattlerSerializable enemyData)
        {
            enemyData.index = _data.Count;
            _data.Add(enemyData);
        }

        public List<int> UniqIds()
        {
            List<int> uniqIds = new List<int>();
            _data.ForEach(x =>
            {
                uniqIds.Add(x.uniqId);
            });
            return uniqIds;
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
        
        public static EnemyDataModel Instance => _instance;
    }
}