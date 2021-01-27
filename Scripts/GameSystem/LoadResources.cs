using System.Collections.Generic;
using System.IO;
using Model.Master;
using Serializable;
using UnityEngine;

namespace GameSystem
{
    /// <summary>
    /// マスターデータ類を読み込むクラス
    /// ゲーム開始前に実行する
    /// </summary>
    public class LoadResources
    {
        public void Initialize()
        {
            Texture2D cursorTexture = Resources.Load<Texture2D>("Texture/Mouse");
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
            ReadMonsterData();
            ReadSkillData();
            ReadItemData();
            ReadDungeonData();
        }

        /// <summary>
        /// モンスターデータを読み込む
        /// </summary>
        private void ReadMonsterData()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Data/Monsters");
            FileInfo[] info = dir.GetFiles("*.json");

            List<MonsterSerializable> monsterData = new List<MonsterSerializable>();
            foreach (FileInfo f in info)
            {
                var json = File.ReadAllText(Application.streamingAssetsPath + "/Data/Monsters/" + f.Name);
                monsterData.Add(JsonUtility.FromJson<MonsterSerializable>(json));
            }
            MasterMonsterDataModel.Instance.CreateData(monsterData);
        }

        /// <summary>
        /// ダンジョンデータを読み込む
        /// </summary>
        private void ReadDungeonData()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Data/Dungeons");
            FileInfo[] info = dir.GetFiles("*.json");

            List<DungeonSerializable> dungeonData = new List<DungeonSerializable>();
            foreach (FileInfo f in info)
            {
                var json = File.ReadAllText(Application.streamingAssetsPath + "/Data/Dungeons/" + f.Name);
                dungeonData.Add(JsonUtility.FromJson<DungeonSerializable>(json));
            }
            MasterDungeonDataModel.Instance.CreateData(dungeonData);
        }

        /// <summary>
        /// スキルデータを読み込む
        /// </summary>
        private void ReadSkillData()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Data/Skills");
            FileInfo[] info = dir.GetFiles("*.json");
            
            List<SkillsSerializable> skillData = new List<SkillsSerializable>();
            foreach (FileInfo f in info)
            {
                var json = File.ReadAllText(Application.streamingAssetsPath + "/Data/Skills/" + f.Name);
                skillData.Add(JsonUtility.FromJson<SkillsSerializable>(json));
            }
            
            Read<SkillsSerializableList>("/Data/Skills/SkillSets/").ForEach(skills => { skillData.AddRange(skills.list); });
            
            MasterSkillDataModel.Instance.CreateData(skillData);
        }

        /// <summary>
        /// パッシブデータを読み込む
        /// </summary>
        private void ReadPassiveData()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath + "/Data/Passives");
            List<PassiveSerializableData> passiveData = new List<PassiveSerializableData>();
            Read<PassiveSerializableDataList>("/Data/Passives/").ForEach(passive => { passiveData.AddRange(passive.list); });
            
            //MasterSkillDataModel.Instance.CreateData(skillData);
        }

        /// <summary>
        /// アイテムデータを読み込む
        /// </summary>
        private void ReadItemData()
        {
            var itemList = Read<ItemSerializable>("/Data/Items/");
            Read<ItemSerializableList>("/Data/Items/Equipment/").ForEach(items => { itemList.AddRange(items.list); });
            Read<ItemSerializableList>("/Data/Items/SoulCrystal/").ForEach(items => { itemList.AddRange(items.list); });
            MasterItemDataModel.Instance.CreateData(itemList);
        }

        private List<T> Read<T>(string dir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Application.streamingAssetsPath + dir);
            FileInfo[] info = dirInfo.GetFiles("*.json");
            
            List<T> list = new List<T>();
            foreach (FileInfo f in info)
            {
                var json = File.ReadAllText(Application.streamingAssetsPath + dir + f.Name);
                list.Add(JsonUtility.FromJson<T>(json));
            }

            return list;
        }
    }
}