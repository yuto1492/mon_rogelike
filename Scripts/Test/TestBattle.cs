using Battler.Actor;
using Battler.Enemy;
using GameSystem;
using Model;
using Model.Person;
using UnityEngine;

namespace Test
{
    public class TestBattle : MonoBehaviour
    {
        private void Start()
        {
            LoadResources resources = new LoadResources();
            resources.Initialize();

            //アクターの作成とメンバーの追加
            ActorDataModel actorData = ActorDataModel.Instance;
            MemberDataModel memberData = MemberDataModel.Instance;
            memberData.Add(ActorLogic.Create("Mandrake", 3));
            memberData.Add(ActorLogic.Create("Slime", 3));
            memberData.Add(ActorLogic.Create("MechanicSoldier", 3));
            memberData.Add(ActorLogic.Create("Kyubi", 3));

            //エネミーの作成
            EnemyDataModel enemyData = EnemyDataModel.Instance;
            enemyData.Initialize();
            enemyData.Add(EnemyLogic.Create("Goblin", 3));
            enemyData.Add(EnemyLogic.Create("GreenDragon", 3));
            enemyData.Add(EnemyLogic.Create("Goblin", 4));

            //アイテム
            InventoryDataModel.GetInstance().StartUp();
            
            var manager = gameObject.GetComponent<BattleManager>();
            manager.Initialize();
        }
    }
}