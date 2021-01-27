using Model;
using Model.Master;
using Serializable;

namespace Dictionary
{
    public class DungeonDictionary
    {
        public static DungeonSerializable GetDungeonMapData(string dungeonId)
        {
            return MasterDungeonDataModel.Instance.Data.Find(x => x.data.id == dungeonId);
        }
    }
}