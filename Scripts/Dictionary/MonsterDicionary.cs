using Model;
using Model.Master;
using Serializable;
using UnityEngine;

namespace Dictionary
{
    public class MonsterDicionary
    {
        public static MonsterSerializable GetMonsterData(string id)
        {
            return MasterMonsterDataModel.Instance.Data.Find(x => x.data.monsterId == id);
        }

        public static Sprite GetSprite(string id)
        {
            return Resources.Load<Sprite>(GetMonsterData(id).imageData.spritePath);
            //return AssetDatabase.LoadAssetAtPath<Sprite>();
        }

        public static Vector3 GetScale(string id)
        {
            if (GetMonsterData(id).imageData.actorScale != 0)
            {
                return new Vector3(GetMonsterData(id).imageData.actorScale, GetMonsterData(id).imageData.actorScale, 1);
            }
            return Vector3.one;
        }
    }
}