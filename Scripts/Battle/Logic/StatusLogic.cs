using System.Collections.Generic;
using Consts;
using Container;
using Dictionary;
using Serializable;
using UniRx;
using UnityEngine;
using Utils;
using View;
using View.Battle.GUI;

namespace Battle.Logic
{
    public struct StatusUpdateResult
    {
        public string StatusType;
        public int Value;
        public bool IsDead;
    }
    public class StatusLogic : MonoBehaviour
    {
        public static AsyncSubject<Unit> StatusEffect(List<StatusUpdateResult> results, int uniqId, int activeUniqId)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            int count = 0;
            var subjects = new SubjectContainer();
            var fromBattler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            results.ForEach(x =>
            {
                switch (x.StatusType)
                {
                    case StatusConsts.POISON:
                        AnnounceTextView.Instance.PoisonText(fromBattler, x.Value, x.IsDead);
                        List<SkillDamage> damage = new List<SkillDamage>();
                        damage.Add(new SkillDamage()
                        {
                            damage = x.Value,
                            valueTarget = SkillConsts.HP
                        });
                        List<SkillDamages> damageses = new List<SkillDamages>();
                        damageses.Add(new SkillDamages()
                        {
                            isHit = true,
                            SkillDamage = damage,
                            targetUniqId = uniqId,
                            isDead = x.IsDead
                        });
                        
                        subjects.Add(EffectManager.Instance.PoisonEffect(uniqId, damageses));
                        break;
                    case StatusConsts.SLEEP:
                        if (uniqId == activeUniqId)
                        {
                            AnnounceTextView.Instance.SleepText(fromBattler);
                            subjects.Add(EffectManager.Instance.SleepEffect(uniqId));
                        }
                        break;
                }
            });

            subjects.Play().Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });

            return subject;
        }

        /// <summary>
        /// ターン毎の状態異常処理
        /// </summary>
        public static List<StatusUpdateResult> StatusUpdate(BattlerSerializable battler)
        {
            List<StatusUpdateResult> results = new List<StatusUpdateResult>();
            //ダメージ系の異常処理を先に行う
            //毒
            PoisonDamageLogic(battler, ref results);
            //
            SleepLogic(battler, ref results);
            //ターン系の状態異常の削除処理
            StateTurnDelete(battler.status);
            BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Status.OnNext(Unit.Default);

            return results;
        }

        private static void PoisonDamageLogic(BattlerSerializable battler, ref List<StatusUpdateResult> results)
        {
            if (BattleLogic.DeadCheck(battler) == false)
            {
                var state = battler.status.Find(x => x.type == StatusConsts.POISON);
                if (state != null)
                {
                    BattleLogic.SustainDamage(state.value, battler);
                    results.Add(new StatusUpdateResult()
                    {
                        StatusType = state.type,
                        Value = state.value,
                        IsDead = BattleLogic.DeadCheck(battler)
                    });
                    StateTurnCount(state);
                }
            }
        }

        private static void SleepLogic(BattlerSerializable battler, ref List<StatusUpdateResult> results)
        {
            if (BattleLogic.DeadCheck(battler) == false)
            {
                var state = battler.status.Find(x => x.type == StatusConsts.SLEEP);
                if (state != null)
                {
                    results.Add(new StatusUpdateResult()
                    {
                        StatusType = state.type
                    });
                    StateTurnCount(state);
                }
            }
        }

        public static bool TurnSkipCheck(int uniqId)
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            foreach (var state in battler.status)
            {
                switch (state.type)
                {
                    case SkillValueTarget.SLEEP:
                        return true;
                }
            }

            return false;
        }

        public static void RemoveAnnounce(string type, BattlerSerializable battler)
        {
            switch (type)
            {
                case StatusConsts.SLEEP:
                    AnnounceTextView.Instance.WakeUpText(battler);
                    break;
            }
        }

        private static void StateTurnCount(BattlerSerializableStatus state)
        {
            state.turn -= 1;
        }

        private static void StateTurnDelete(List<BattlerSerializableStatus> statuses)
        {
            statuses.RemoveAll(x => StatusDictionary.IsTurnStatus(x.type) && x.turn <= 0);
        }
        
    }
}