using System;
using System.Collections.Generic;

namespace Serializable
{
    [Serializable]
    public class PassiveSerializableDataList
    {
        public List<PassiveSerializableData> list;
    }

    [Serializable]
    public class PassiveSerializableData
    {
        public string passiveId;
        public string name;
        public PassiveSerializableEffectData effect;
        public string iconSpritePath;
        public string description;
    }

    [Serializable]
    public class PassiveSerializableEffectData
    {
        public PassiveSerializableEffectParameterData parameter;
        public List<string> special;
    }

    [Serializable]
    public class PassiveSerializableEffectParameterData
    {
        public float hitRate;
    }

}