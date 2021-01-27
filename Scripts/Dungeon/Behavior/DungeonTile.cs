using System;
using UnityEngine;

namespace Dungeon.Behavior
{
    public class DungeonTile : MonoBehaviour
    {
        public void OnMouseUp()
        {
            DungeonPresenter.GetInstance().DungeonTileClick();
        }

        public void OnMouseEnter()
        {
            DungeonPresenter.GetInstance().DungeonTileHover();
        }

        public void OnMouseExit()
        {
            DungeonPresenter.GetInstance().DungeonTileHoverOut();
        }
    }
}