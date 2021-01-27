using System;
using Battle;
using Extensions;
using UnityEngine;

namespace View.Battle.GUI
{
    public class BattleGuiManager : MonoSingleton<BattleGuiManager>
    {
        public GameObject resultGameObject;
        public GameObject skillUiGameObject;
        public GameObject timeLineGameObject;

        private BattlerResultView _result;
        private TimeLine _timeline;
        
        public void Initialize()
        {
            _result = new BattlerResultView();
            _timeline = new TimeLine();
            _timeline.Initialize();
        }

        public BattlerResultView Result
        {
            get => _result;
        }

        public TimeLine Timeline
        {
            get => _timeline;
        }
    }
}