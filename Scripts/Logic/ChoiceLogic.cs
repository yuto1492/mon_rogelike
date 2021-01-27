using System.Collections.Generic;
using System.Linq;
using Dictionary;
using Dungeon;
using Model.Structs;
using UnityEngine;
using Utils;

namespace Logic
{
    public class ChoiceLogic : MonoBehaviour
    {
        public static EnemiesStruct EnemiesChoice()
        {
            var dungeonData = DungeonDictionary.GetDungeonMapData(DungeonDataModel.Instance.DungeonId);
            var floor = DungeonDataModel.Instance.Floor;
            
            //敵の数
            int amountMin = 0;
            int amountMax = 0;
            var enemyAmountData = dungeonData.enemyAmount.FirstOrDefault(x => x.border > floor);
            if (enemyAmountData == null)
                enemyAmountData = dungeonData.enemyAmount.Last();
            amountMin = enemyAmountData.min;
            amountMax = enemyAmountData.max;
                
            //敵の種類パターン
            var enemiesData = dungeonData.enemies.FirstOrDefault(x => x.border > floor);
            if (enemiesData == null)
                enemiesData = dungeonData.enemies.Last();
            
            WeightChoice<string> choice = new WeightChoice<string>();
            enemiesData.monsters.ForEach(enemy =>
            {
                choice.Add(1, enemy);
            });
            
            var amount = Random.Range(amountMin, amountMax);
            List<string> enemies = new List<string>();
            for (var x = 0; x < amount; x++)
            {
                enemies.Add(choice.ChoiceOne());
            }

            return new EnemiesStruct()
            {
                Enemies = enemies,
                AddLevel = enemiesData.addLevel
            };
        }
    }
}