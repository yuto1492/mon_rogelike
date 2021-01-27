using System;
using Consts;
using Consts.Enums;
using DG.Tweening;
using Dictionary;
using Dungeon.Logic;
using Dungeon.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Dungeon
{
    public class DungeonPresenter
    {
        private static DungeonPresenter _singleton = new DungeonPresenter();
        
        public static DungeonPresenter GetInstance()
        {
            return _singleton;
        }
        
        private DungeonMapView _dungeonMapView = new DungeonMapView();
        private DungeonEffectView _dungeonEffectView = new DungeonEffectView();
        private Tilemap _tilemap = GameObject.Find("Map/Grid/Tile").GetComponent<Tilemap>();

        public void Initialize(string dungeonId)
        {
            DungeonLogic.CreateMap(dungeonId);
            _dungeonMapView.Refresh();
            DungeonDataModel.Instance.DungeonFloorSelectSubject = new Subject<Vector3Int>();
            //DungeonEventSubscribe();
        }

        /// <summary>
        /// タイルをクリックした処理
        /// TODO 
        /// </summary>
        public void DungeonTileClick()
        {
            Vector3Int mousePosition = GetTilePosition(_tilemap);
            var model = DungeonDataModel.Instance;
            if (model.Location.y + 1 == mousePosition.y && model.DungeonFloor[mousePosition.x][mousePosition.y].Enabled &&
                model.IsEvent == false)
            {
                model.Location = mousePosition;
                model.IsEvent = true;
                DungeonLogic.DungeonEvent(mousePosition,_dungeonEffectView).Subscribe(_ =>
                {
                    model.Floor++;
                    _dungeonMapView.NextFloor().Subscribe(__ =>
                    {
                        //イベント後
                        model.IsEvent = false;
                    });
                    DungeonLogic.DisableFloor();
                    DungeonLogic.DisableNextFloor();
                    _dungeonMapView.DisableFloor();
                    _dungeonMapView.NowLocationFloor();
                    model.Location = mousePosition;
                });
            }
        }

        /// <summary>
        /// タイルマウスホバー時の処理
        /// </summary>
        public void DungeonTileHover()
        {
            Vector3Int mousePosition = GetTilePosition(_tilemap);
            var model = DungeonDataModel.Instance;
            //現フロアの+1のみ反応する
            if (model.Location.y + 1 == mousePosition.y && model.DungeonFloor[mousePosition.x][mousePosition.y].Enabled &&
                model.IsEvent == false)
            {
                _dungeonMapView.FocusTile(mousePosition);
            }
        }

        public void DungeonTileHoverOut()
        {
            _dungeonMapView.UnFocusTile();
        }

        private Vector3Int GetTilePosition(Tilemap tilemap)
        {
            var position = Input.mousePosition;
            position.z = 10f;
            var screenToWorldPointPosition = Camera.main.ScreenToWorldPoint(position);
            Vector3Int mousePosition = tilemap.WorldToCell(screenToWorldPointPosition);
            return mousePosition;
        }

    }
}