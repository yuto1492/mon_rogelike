using Battler.Actor;
using Model;
using Dungeon;
using GameSystem;
using Model.Person;
using UnityEngine;

namespace Test
{
    public class TestDungeon: MonoBehaviour
    {
        private void Start()
        {
            LoadResources resources = new LoadResources();
            resources.Initialize();
            
            //アクターの作成とメンバーの追加
            ActorDataModel actorData = ActorDataModel.Instance;
            MemberDataModel memberData = MemberDataModel.Instance;
            memberData.Add(ActorLogic.Create("Mandrake","植物", 1));
            memberData.Add(ActorLogic.Create("Slime","すらりん", 1));
            memberData.Add(ActorLogic.Create("MechanicSoldier","ろぼ兵士", 100));
            //memberData.Add(ActorLogic.Create("Kyubi", 1));

            //アイテム
            InventoryDataModel.GetInstance().StartUp();
            
            //ダンジョン
            DungeonPresenter.GetInstance().Initialize("FirstForest");

        }
    }
}