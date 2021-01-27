using UniRx;
using System.Collections.Generic;
using Battle.Logic;
using Battler.Enemy;
using Consts;
using Dictionary;
using Dungeon;
using Model;
using Structs;
using UnityEngine;
using Utils;

namespace GameSystem
{
    
    /// <summary>
    /// TODO 
    /// </summary>
    public class GameSceneManager
    {
        private static GameObject _battleScene;
        
        /// <summary>
        /// バトル開始
        /// </summary>
        /// <returns></returns>
        public static AsyncSubject<List<LootItemStruct>> BootBattle()
        {
            AsyncSubject<List<LootItemStruct>> subject = new AsyncSubject<List<LootItemStruct>>();
            
            var dungeon = DungeonDictionary.GetDungeonMapData(DungeonDataModel.Instance.DungeonId);

            var enemies = BattleLogic.EnemiesChoice();
            var enemyDataModel = EnemyDataModel.Instance;
            enemyDataModel.Initialize();
            enemies.Enemies.ForEach(enemyId =>
            {
                float level = Random.Range(dungeon.enemyLevel.min, dungeon.enemyLevel.max);
                level = level * DungeonDataModel.Instance.Location.y + 1;
                level += enemies.AddLevel;
                enemyDataModel.Add(EnemyLogic.Create(enemyId, (int) level));
            });

            _battleScene  = Object.Instantiate((GameObject) Resources.Load("Prefabs/Scene/Battle"), Vector3.zero,
                Quaternion.identity);
            _battleScene.transform.Find("FrontCanvas").GetComponent<Canvas>().worldCamera =
                GameObject.Find("MainCamera").GetComponent<Camera>();
            
            var battleManager = _battleScene.transform.Find("System").GetComponent<BattleManager>();
            battleManager.Initialize();
            battleManager.EndBattle.Subscribe(loots =>
            {
                subject.OnNext(loots);
                subject.OnCompleted();
            });
            
            return subject;
        }
        
        public static void EndBattle()
        {
            GameObject.Destroy(_battleScene);
        }
        
    }
}