using System;
using Consts.Enums;
using UnityEngine;

namespace Serializable
{
    [Serializable]
    public class TimelineSerializableData
    {
        public BattlerEnum.BattlerType battlerType;
        public int uniqId;
        public int timeLine;
        /// <summary>
        /// タイムラインのID
        /// スケジュールのIDと紐づく
        /// </summary>
        public int id;
    }
}