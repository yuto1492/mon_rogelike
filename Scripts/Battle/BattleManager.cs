using System.Collections.Generic;
using Battle;
using Battle.AI;
using Battle.Logic;
using Container;
using Dictionary;
using Model;
using Serializable;
using Structs;
using UniRx;
using UnityEngine;
using Utils;
using View.Battle.GUI;
using Serializable;

public class BattleManager : MonoBehaviour
{

    private AiManager _aiManager;
    private IDisposableContainer _disposableContainer;
    public AsyncSubject<List<LootItemStruct>> EndBattle;

    public void Initialize()
    {
        BattleGuiManager.Instance.Initialize();
        _aiManager = new AiManager();
        BattlePresenter.GetInstance().Initialize(this);
        EndBattle = new AsyncSubject<List<LootItemStruct>>();
        Invoke("TurnStart", 2f);
    }

    /// <summary>
    /// ターン開始前の処理
    /// </summary>
    /// <returns></returns>
    private SubjectContainer TurnAwake(int activeUniqId)
    {
        SubjectContainer subjects = new SubjectContainer();
        //色々初期化
        BattlePresenter.GetInstance().Refresh();
        
        //味方の状態異常処理
        BattleDictionary.GetAllAliveBattlers().ForEach(battler =>
        {
            var results = StatusLogic.StatusUpdate(battler);
            if (results.Count != 0)
            {
                subjects.Add(StatusLogic.StatusEffect(results, battler.uniqId, activeUniqId));
            }

            DeadCheck(battler);
        });
        
        subjects.Add(BattleGuiManager.Instance.Timeline.TimelineScheduleRemove());
        
        return subjects;
    }

    private void DeadCheck(BattlerSerializable battler)
    {
        if (BattleLogic.DeadCheck(battler) && BattlerDictionary.IsDead(battler) == false)
        {
            BattleLogic.Dead(battler);
            BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Dead.OnNext(true);     
        }
    }
    
    /// <summary>
    /// ターン開始
    /// </summary>
    private void TurnStart()
    {
        _disposableContainer = new IDisposableContainer();
        TimelineSerializableData timelineData =
            BattleDictionary.GetTimelineById(BattleGuiManager.Instance.Timeline.TimelineData, BattleGuiManager.Instance.Timeline.TimelineSchedule[0]);

        TurnAwake(timelineData.uniqId).Play().Subscribe(_ =>
        {
            ObservableUtils.Timer(300).Subscribe(__ =>
            {
                //行動不能判定
                if (StatusLogic.TurnSkipCheck(timelineData.uniqId))
                {
                    TurnEnd();
                }
                else
                {
                    //行動開始 アクターなら選択処理 エネミー、ミニオンならAI
                    BattlePresenter.GetInstance().BattlerSpriteModel.GetData(timelineData.uniqId).Active.OnNext(true);
                }
            });
        });
    }
    
    public void AiAction(int uniqId)
    {
        ObservableUtils.Timer(1000).Subscribe(__ =>
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var ai = AiManager.Action(uniqId);
            BattleLogic.SkillByAi(ai, battler).Subscribe(___ =>
            {
                TurnEnd();
            });
        });
    }

    /// <summary>
    /// ターン終了時の処理
    /// 死亡判定などを行う
    /// </summary>
    public void TurnEnd()
    {
        EnemyDataModel.Instance.Data.ForEach(battler =>
        {
            DeadCheck(battler);
        });
        MemberDataModel.Instance.GetActorData().ForEach(battler =>
        {
            DeadCheck(battler);
        });
        
        //タイムラインスケジュールから死んでいるキャラを削除する
        BattleGuiManager.Instance.Timeline.TimelineScheduleRemove().Subscribe(_ =>
        {
            ObservableUtils.Timer(400).Subscribe(__ =>
            {
                //敵がいない場合はリザルト画面に遷移
                if (BattleLogic.AllEnemyDeadCheck())
                {
                    var loots = BattleLogic.BattleResult();
                    BattleGuiManager.Instance.Result.ShowResult(loots).Subscribe(___ =>
                    {
                        End(loots);
                    });
                }
                else
                {
                    TimelineNext();
                }
            });
        });
    }

    /// <summary>
    /// 戦闘終了処理
    /// </summary>
    private void End(List<LootItemStruct> loots)
    {
        EndBattle.OnNext(loots);
        EndBattle.OnCompleted();
    }

    /// <summary>
    /// タイムラインを次に進行する
    /// </summary>
    private void TimelineNext()
    {
        BattleGuiManager.Instance.Timeline.TimelineNext().Subscribe(_ =>
        {
            ObservableUtils.Timer(300).Subscribe(__ =>
            {
                TurnStart();
            });
        });
    }
    
}
