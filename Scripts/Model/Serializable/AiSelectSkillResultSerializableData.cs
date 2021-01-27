using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serializable
{
    [Serializable]
    public class AiSelectSkillResultSerializableData
    {
        public string SkillId;
        public List<int> TargetUniqIds = new List<int>();
    }
}