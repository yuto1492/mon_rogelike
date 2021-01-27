using System;
using System.Collections.Generic;
using System.Linq;
using Consts;
using Consts.Enums;
using DG.Tweening;
using Dictionary;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utils;

namespace Dungeon.View
{
    public class DungeonMapView
    {
        private Dictionary<DungeonEnum.Tiles,Tile> _tiles = new Dictionary<DungeonEnum.Tiles,Tile>();
        private Tilemap _objectTile;
        private Tilemap _tile;
        private Tilemap _overlayTile;
        private Tilemap _focusTile;
        private Vector3Int? _currentLocationVec;
        private Vector3Int? _currentFocusVec;

        public DungeonMapView()
        {
            foreach (DungeonEnum.Tiles value in Enum.GetValues(typeof(DungeonEnum.Tiles)))
            {
                _tiles.Add(value,Resources.Load<Tile>(GetMapTileResourcePath(value)));
            }
            _objectTile = GameObject.Find("Map/Grid/Object").GetComponent<Tilemap>();
            _tile = GameObject.Find("Map/Grid/Tile").GetComponent<Tilemap>();
            _overlayTile = GameObject.Find("Map/Grid/Overlay").GetComponent<Tilemap>();
            _focusTile = GameObject.Find("Map/Grid/Focus").GetComponent<Tilemap>();
        }

        private string GetMapTileResourcePath(DungeonEnum.Tiles tile)
        {
            switch (tile)
            {
                case DungeonEnum.Tiles.Battle:
                    return "Sprites/Dungeon/TileIcons/btn_icon_battle";
                case DungeonEnum.Tiles.Boss:
                    return "Sprites/Dungeon/TileIcons/btn_icon_boss";
                case DungeonEnum.Tiles.Elite:
                    return "Sprites/Dungeon/TileIcons/btn_icon_battle_elite";
                case DungeonEnum.Tiles.Evt:
                    return "Sprites/Dungeon/TileIcons/btn_icon_question";
                case DungeonEnum.Tiles.Treasure:
                    return "Sprites/Dungeon/TileIcons/btn_icon_chest_1";
                case DungeonEnum.Tiles.Blank:
                    return "Sprites/Dungeon/MapCell";
                case DungeonEnum.Tiles.Focus:
                    return "Sprites/Dungeon/MapCellFocus";
                case DungeonEnum.Tiles.Location:
                    return "Sprites/Dungeon/MapCellLocation";
                case DungeonEnum.Tiles.Disable:
                    return "Sprites/dungeon/MapCellOverlay";
            }

            return "";
        }

        public void Refresh()
        {
            var model = DungeonDataModel.Instance;
            foreach (var (x, xIndex) in model.DungeonFloor.Select((x, xIndex) => (x, xIndex)))
            {
                foreach (var (y, yIndex) in x.Select((y, yIndex) => (y, yIndex)))
                {
                    Vector3Int pos = new Vector3Int(xIndex, yIndex, 0);
                    _tile.SetTile(pos,_tiles[DungeonEnum.Tiles.Blank]);
                    Tile tile = _tiles[y.Tile];
                    _objectTile.SetTile(pos, tile);
                }
            }
            FloorTextUpdate();
            DisableFloor();
        }

        private void FloorTextUpdate()
        {
            var model = DungeonDataModel.Instance;
            GameObject.Find("UI/Floor/Floor").GetComponent<TextMeshProUGUI>().text = $"{model.Floor}/{model.DungeonFloorSize}";
        }

        public AsyncSubject<Unit> NextFloor()
        {
            var model = DungeonDataModel.Instance;
            FloorTextUpdate();
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            if (model.Location.y != 0 && model.Location.y + 5 < model.DungeonFloorSize)
            {
                var map = GameObject.Find("Map").GetComponent<Transform>();
                //map.DOLocalMoveY(map.localPosition.y - 0.75f, 0.5f).Play().OnComplete(() =>
                map.DOLocalMoveY(map.localPosition.y - 1.115f, 0.5f).Play().OnComplete(() =>
                {
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });
            }
            else
            {
                ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            }
            return subject;
        }

        public void DisableFloor()
        {
            var model = DungeonDataModel.Instance;
            foreach (var (x, xIndex) in model.DungeonFloor.Select((x, xIndex) => (x, xIndex)))
            {
                foreach (var (y, yIndex) in x.Select((y, yIndex) => (y, yIndex)))
                {
                    if (y.Enabled == false)
                    {
                        _overlayTile.SetTile(new Vector3Int(xIndex, yIndex, 0), _tiles[DungeonEnum.Tiles.Disable]);
                    }
                }
            }
        }

        public void NowLocationFloor()
        {
            var model = DungeonDataModel.Instance;
            if (_currentLocationVec != null)
            {
                _tile.SetTile(_currentLocationVec.Value, _tiles[DungeonEnum.Tiles.Disable]);
            }
            _tile.SetTile(model.Location, _tiles[DungeonEnum.Tiles.Location]);
            _currentLocationVec = model.Location;
        }

        public void SetTile(Vector3Int vec, DungeonEnum.Tiles tile)
        {
            _tile.SetTile(vec,_tiles[tile]);
        }

        public void FocusTile(Vector3Int vec)
        {
            _focusTile.ClearAllTiles();
            _focusTile.SetTile(vec, _tiles[DungeonEnum.Tiles.Focus]);
        }

        public void UnFocusTile()
        {
            _focusTile.ClearAllTiles();
        }

    }
}