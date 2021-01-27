using System;
using System.Collections.Generic;
using Serializable;
using UniRx;
using UnityEngine;

namespace Model
{
    public class BattleBattlerSpriteModel
    {
        public int SelectUniqId;
        public Subject<Unit> SelectedSubject = new Subject<Unit>();
        public Subject<Unit> AllSelectSubject = new Subject<Unit>();
        public Subject<Unit> SkillSelectSubject = new Subject<Unit>(); 
        private List<BattleBattlerSpriteModelSerializable> _data = new List<BattleBattlerSpriteModelSerializable>();
        public int HoverUniqId;

        public BattleBattlerSpriteModel()
        {
            List<int> uniqIds = MemberDataModel.Instance.UniqIds();
            uniqIds.AddRange(EnemyDataModel.Instance.UniqIds());
            uniqIds.ForEach(uniqId =>
            {
                _data.Add(new BattleBattlerSpriteModelSerializable()
                {
                    uniqId = uniqId
                });
            });
        }
        public BattleBattlerSpriteModelSerializable GetData(int uniqId)
        {
            return _data.Find(x => x.uniqId == uniqId);
        }
        
    }

    [Serializable]
    public class BattleBattlerSpriteModelSerializable
    {
        public int uniqId;
        public Subject<int> Hp = new Subject<int>();
        public Subject<bool> Active = new Subject<bool>();
        public Subject<bool> Dead = new Subject<bool>();
        public Subject<Unit> Status = new Subject<Unit>(); 
    }
}