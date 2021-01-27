using Serializable;
using UniRx;
using UnityEngine;

namespace Battle.Parameter
{
    public class BattlerBehaviour : MonoBehaviour
    {
        private BattlerSerializable data;

        public int UniqId;
        public Subject<bool> Active = new Subject<bool>();
    }
}