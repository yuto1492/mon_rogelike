using System;
using Dungeon.Logic;
using UnityEngine;

namespace Modal
{
    public class ModalManager : MonoBehaviour
    {
        public void Start()
        {
        }

        public void OpenMonsterModal()
        {
            MonsterStatusModal.MonsterStatusModal.GetInstance().Open();
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        public void OpenTestModal()
        {
            //DungeonEventLogic.TreasureEvent();
        }
        
    }
}