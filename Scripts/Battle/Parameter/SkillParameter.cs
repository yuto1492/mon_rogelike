using Serializable;
using UnityEngine;

namespace Battle.Parameter
{
    /// <summary>
    /// スキルデータをゲームオブジェクトにコンポーネントとして追加されるもの
    /// スキルデータとゲームオブジェクトの値はバインドされてるっぽい
    /// </summary>
    public class SkillParameter : MonoBehaviour
    {
        public SkillsSerializable data;
    }
}