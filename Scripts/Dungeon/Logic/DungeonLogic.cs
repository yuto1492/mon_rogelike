using System.Collections.Generic;
using Consts;
using Consts.Enums;
using Dictionary;
using Dungeon.View;
using GameSystem;
using Serializable;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utils;
using View;

namespace Dungeon.Logic
{
    public static class DungeonLogic
    {
        public static void CreateMap(string dungeonId)
        {
            DungeonSerializable dungeonData = DungeonDictionary.GetDungeonMapData(dungeonId);
            var model = DungeonDataModel.Instance;
            model.Location = new Vector3Int(DungeonConsts.WIDTH_SIZE / 2, -1, 0);
            model.DungeonId = dungeonId;
            model.DungeonFloorSize = GetMapFloorSize(dungeonId);
            
            model.DungeonFloor = new DungeonDataModel.DungeonFloorStruct[DungeonConsts.WIDTH_SIZE][];
            for (int x = 0; x < DungeonConsts.WIDTH_SIZE; x++)
            {
                //model.DungeonFloor[x] = new DungeonEnum.Tiles[floorSize];
                model.DungeonFloor[x] = new DungeonDataModel.DungeonFloorStruct[model.DungeonFloorSize];
                DungeonEventCreateLogic choice = new DungeonEventCreateLogic();
                choice.Add(dungeonData.map.battle, DungeonEnum.Tiles.Battle);
                choice.Add(dungeonData.map.elite, DungeonEnum.Tiles.Elite);
                choice.Add(dungeonData.map.evt, DungeonEnum.Tiles.Evt);
                choice.Add(dungeonData.map.treasure, DungeonEnum.Tiles.Treasure);
                for (int y = 0; y < model.DungeonFloorSize; y++)
                {
                    //最初の左右、ボス前の左右、ボス以外はブランク
                    if (IsBlank(x, y, dungeonId))
                    {
                        //model.DungeonFloor[x][y] = DungeonEnum.Tiles.Blank;
                        model.DungeonFloor[x][y] = new DungeonDataModel.DungeonFloorStruct()
                        {
                            Tile = DungeonEnum.Tiles.Blank,
                            Enabled = false
                        };
                    }
                    else
                    {
                        model.DungeonFloor[x][y] = new DungeonDataModel.DungeonFloorStruct()
                        {
                            Tile = choice.ChoicePickOut(),
                            Enabled = true
                        };
                        //model.DungeonFloor[x][y] = choice.ChoicePickOut();
                    }

                    /*//最初は雑魚バトル確定
                    if (y == 1 && x != 0 && x != DungeonConsts.WIDTH_SIZE - 1)
                        model.DungeonCell[x][y] = DungeonEnum.Events.Battle;*/
                    //最終フロアの中央はボス
                    /*if (y == floorSize && x == (int) (DungeonConsts.WIDTH_SIZE / 2))
                        model.DungeonCell[x][y] = DungeonEnum.Events.Boss;*/
                }
            }
        }

        private static int GetMapFloorSize(string dungeonId)
        {
            int count = 0;
            var map = DungeonDictionary.GetDungeonMapData(dungeonId).map;
            count += map.battle;
            count += map.boss;
            count += map.elite;
            count += map.evt;
            count += map.treasure;
            return count;
        }
        
        public static bool IsBlank(int x, int y,string dungeonId)
        {
            var floorSize = GetMapFloorSize(dungeonId);
            if (x == 0 && y == 0 || x == DungeonConsts.WIDTH_SIZE - 1 && y == 0 || x == 0 && y == floorSize - 2 ||
                x == DungeonConsts.WIDTH_SIZE - 1 && y == floorSize - 2 ||
                (x == 0 || x == 1 || x == DungeonConsts.WIDTH_SIZE - 2 || x == DungeonConsts.WIDTH_SIZE - 1) &&
                y == floorSize - 1)
            {
                return true;
            }

            return false;
        }

        public static AsyncSubject<Unit> DungeonEvent(Vector3Int vec, DungeonEffectView dungeonEffectView)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var model = DungeonDataModel.Instance;
            var evt = model.DungeonFloor[vec.x][vec.y];
            switch (evt.Tile)
            {
                //戦闘の場合
                case DungeonEnum.Tiles.Battle:
                    BattleEvent(subject, dungeonEffectView);
                    break;
                //トレジャーイベントの場合
                case DungeonEnum.Tiles.Treasure:
                    DungeonEventLogic.TreasureEvent(subject);
                    break;
            }

            return subject;
        }

        /// <summary>
        /// 戦闘イベント処理
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="dungeonEffectView"></param>
        private static void BattleEvent(AsyncSubject<Unit> subject, DungeonEffectView dungeonEffectView)
        {
            dungeonEffectView.OpenTransition().Subscribe(_ =>
            {
                var dungeon = GameObject.Find("Dungeon"); 
                dungeon.SetActive(false);
                //戦闘画面を立ち上げる
                GameSceneManager.BootBattle().Subscribe(loots =>
                {
                    dungeonEffectView.OpenTransition().Subscribe(___ =>
                    {
                        //戦闘画面を消す
                        GameSceneManager.EndBattle();
                        dungeon.SetActive(true);
                        dungeonEffectView.HideTransition().Subscribe(____ =>
                        {
                            //報酬ログを流す
                            List<string> texts = new List<string>();
                            loots.ForEach(loot =>
                            {
                                texts.Add(ItemDictionary.GetAnnounceText(loot));
                            });
                            AnnounceTextView.Instance.AddText(texts);
                            
                            subject.OnNext(Unit.Default);
                            subject.OnCompleted();
                        });
                    });
                });
                dungeonEffectView.HideTransition();
            });
        }

        public static void DisableNextFloor()
        {
            var model = DungeonDataModel.Instance;
            var y = model.Location.y + 1;
            if (y < model.DungeonFloorSize)
            {
                for (var x = 0; x < DungeonConsts.WIDTH_SIZE; x++)
                {
                    if (x == model.Location.x + 1 || x == model.Location.x || x == model.Location.x - 1)
                    {
                        model.DungeonFloor[x][y].Enabled = true;
                    }
                    else
                    {
                        model.DungeonFloor[x][y].Enabled = false;
                    }
                }
            }
        }

        public static void DisableFloor()
        {
            var model = DungeonDataModel.Instance;
            for (var x = 0; x < DungeonConsts.WIDTH_SIZE; x++)
            {
                if (x != model.Location.x)
                {
                    model.DungeonFloor[x][model.Location.y].Enabled = false;
                }
            }
        }
    }
}