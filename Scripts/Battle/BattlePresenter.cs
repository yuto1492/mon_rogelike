using System.Collections.Generic;
using System.Linq;
using Battle.Logic;
using Battle.view.GUI;
using Consts;
using Consts.Enums;
using Container;
using Dictionary;
using Model;
using UniRx;
using UniRx.Triggers;
using Utils;
using Image = UnityEngine.UI.Image;

namespace Battle
{
    public class BattlePresenter
    {       
        private static BattlePresenter _singleton = new BattlePresenter();
        
        public static BattlePresenter GetInstance()
        {
            return _singleton;
        }
        
        private BattleBattlerSpriteModel _battlerSpriteModel;
        private BattleBattlerSpriteView _battlerSpriteView;
        private BattleSkillModel _skillModel;
        private SkillsView _skillsView;
        private BattleModel _battleModel;
        private BattleManager _battleManager;
        private IDisposableContainer _container = new IDisposableContainer();
        
        public void Initialize(BattleManager battleManager)
        {
            _battleManager = battleManager;
            _battleModel = new BattleModel();
            _battlerSpriteModel = new BattleBattlerSpriteModel();
            _battlerSpriteView = new BattleBattlerSpriteView();
            _skillModel = new BattleSkillModel();
            _skillsView = new SkillsView();
            _skillsView.Initialize();
            Refresh();
            SkillSelectSubscribe();
            BattlerSelectSubscribe();
            SkillToSubscribe();
            BattlerSpriteSubscribe();
            BattleSubscribe();
        }

        public void Refresh()
        {
            _skillModel.HoverIndex = -1;
            _skillModel.ActiveIndex = -1;
        }
        
        private void BattleSubscribe()
        {
            _battleModel.NextTurn.Subscribe(_ =>
            {
                _battleManager.TurnEnd();
            });
        }

        public void DisableObservable()
        {
            _container.AllDispose();
        }

        /// <summary>
        /// スキル選択時
        /// </summary>
        private void SkillSelectSubscribe()
        {
            foreach (var (icon, index) in _skillsView.Icons.Select((icon, index) => (icon, index)))
            {
                var image = icon.Find("Image").GetComponent<Image>();
                //マウスホバー時
                _container.Add(image.OnMouseEnterAsObservable().Subscribe(_ =>
                {
                    var battler = BattlerDictionary.GetBattlerByUniqId(_battleModel.ActiveUniqId);
                    var skill = SkillsDicionary.GetEquippedSkillByIndex(battler,index);
                    if (skill != null)
                    {
                        _skillModel.HoverIndex = index;
                        _skillsView.Refresh(_skillModel);
                        _skillsView.ViewSkillDescription(skill.skillId, _battleModel.ActiveUniqId);
                    }
                }));
                //マウスホバーアウト時
                _container.Add(image.OnMouseExitAsObservable().Subscribe(_ =>
                {
                    _skillModel.HoverIndex = -1;
                    _skillsView.Refresh(_skillModel);
                    //_skillsView.CleanSkillDescription();
                }));
                //クリック
                _container.Add(image.OnMouseUpAsButtonAsObservable().Subscribe(_ =>
                {
                    var battler = BattlerDictionary.GetBattlerByUniqId(_battleModel.ActiveUniqId);
                    var skill = SkillsDicionary.GetEquippedSkillByIndex(battler, index);
                    if (skill != null)
                    {
                        _skillModel.SkillId = skill.skillId;
                        _skillModel.ActiveIndex = index;
                        BattlerSpriteModel.SkillSelectSubject.OnNext(Unit.Default);
                        if (SkillsDicionary.IsAll(skill) || SkillsDicionary.IsRandom(skill))
                        {
                            BattlerSpriteModel.AllSelectSubject.OnNext(Unit.Default);                            
                        }
                    }
                }));
            };
        }

        /// <summary>
        /// バトラー選択時
        /// </summary>
        private void BattlerSelectSubscribe()
        {
            _container.Add(BattlerSpriteModel.SkillSelectSubject.Subscribe(_ =>
            {
                _battlerSpriteView.DeActiveOutline();
            }));
            //全体選択時
            _container.Add(BattlerSpriteModel.AllSelectSubject.Subscribe(_ =>
            {
                var skill = SkillsDicionary.GetSkillById(_skillModel.SkillId);
                BattlerSpriteView.Sprites.ForEach(sprite =>
                {
                    var battler = BattlerDictionary.GetBattlerByUniqId(sprite.UniqId);
                    if ((skill.target == SkillConsts.ALL_ENEMY || skill.target == SkillConsts.RANDOM_ENEMY) && battler.battlerType == BattlerEnum.BattlerType.Enemy ||
                        (skill.target == SkillConsts.ALL_MEMBER  || skill.target == SkillConsts.RANDOM_MEMBER) && battler.battlerType == BattlerEnum.BattlerType.Actor )
                    {
                        _battlerSpriteView.SelectOutline(sprite);
                    }
                });
            }));
            BattlerSpriteView.Sprites.ForEach(sprite =>
            {
                //ホバー時
                _container.Add(sprite.SpriteObject.OnMouseOverAsObservable().Subscribe(_ =>
                {
                    if (_skillModel.ActiveIndex != -1 && _battlerSpriteModel.HoverUniqId != sprite.UniqId)
                    {
                        var skill = SkillsDicionary.GetSkillById(_skillModel.SkillId);
                        if (SkillsDicionary.IsSingle(skill))
                        {
                            _battlerSpriteModel.HoverUniqId = sprite.UniqId;
                            var targetBattler = BattlerDictionary.GetBattlerByUniqId(sprite.UniqId);
                            _battlerSpriteView.DeSelectOutlineByBattlerType(targetBattler.battlerType);
                            if (BattleDictionary.IsAppropriate(skill.skillId, _battleModel.ActiveUniqId, sprite.UniqId))
                            {
                                _battlerSpriteView.SelectOutline(sprite);
                            }
                        }
                    }
                }));
                //ターゲット選択時の処理、選択したスキルが発動する
                _container.Add(sprite.SpriteObject.OnMouseUpAsButtonAsObservable().Subscribe(_ =>
                {
                    if (_skillModel.ActiveIndex != -1)
                    {
                        _skillsView.SkillHide();
                        var skill = SkillsDicionary.GetSkillById(_skillModel.SkillId);
                        if (BattleDictionary.IsAppropriate(skill.skillId, _battleModel.ActiveUniqId, sprite.UniqId))
                        {
                            _skillModel.ActiveIndex = -1;

                            switch (skill.target)
                            {
                                case SkillConsts.MEMBER:
                                case SkillConsts.ENEMY:
                                    _battlerSpriteView.DeSelectOutline(sprite);
                                    _battlerSpriteModel.SelectUniqId = sprite.UniqId;
                                    break;
                                case SkillConsts.ALL:
                                case SkillConsts.ALL_ENEMY:
                                case SkillConsts.ALL_MEMBER:
                                case SkillConsts.RANDOM_ENEMY:
                                case SkillConsts.RANDOM_MEMBER:
                                    BattlerSpriteView.Sprites.ForEach(x =>
                                    {
                                        _battlerSpriteView.DeSelectOutline(x);    
                                    });
                                    break;
                            }

                            _battlerSpriteModel.SelectedSubject.OnNext(Unit.Default);
                        }
                    }
                }));
            });
        }

        private void SkillToSubscribe()
        {
            _container.Add(_battlerSpriteModel.SelectedSubject.Subscribe(_ =>
            {
                var skill = SkillsDicionary.GetSkillById(_skillModel.SkillId);
                var battler = BattlerDictionary.GetBattlerByUniqId(_battleModel.ActiveUniqId);
                switch (skill.target)
                {
                    case SkillConsts.ENEMY:
                    case SkillConsts.MEMBER:
                        var uniqId = _battlerSpriteModel.SelectUniqId;
                        var toBattler = BattlerDictionary.GetBattlerByUniqId(uniqId);
                        BattleLogic.SkillToSingle(skill.skillId, battler, toBattler)
                            .Subscribe(
                                __ => { _battleModel.NextTurn.OnNext(Unit.Default); });
                        break;
                    case SkillConsts.ALL_ENEMY:
                        BattleLogic.SkillToAll(skill.skillId, battler,
                            BattleDictionary.GetRivalBattlers(battler.battlerType)).Subscribe(__ =>
                        {
                            _battleModel.NextTurn.OnNext(Unit.Default);
                        });
                        break;
                    case SkillConsts.ALL_MEMBER:
                        BattleLogic.SkillToAll(skill.skillId, battler,
                            BattleDictionary.GetMemberBattlers(battler.battlerType)).Subscribe(__ =>
                        {
                            _battleModel.NextTurn.OnNext(Unit.Default);
                        });
                        break;
                    case SkillConsts.RANDOM_ENEMY:
                        BattleLogic.SkillToRandom(skill.skillId, battler,
                            BattleDictionary.GetRivalUniqIds(BattlerEnum.BattlerType.Actor)).Subscribe(
                            __ =>
                            {
                                _battleModel.NextTurn.OnNext(Unit.Default);
                            });
                        break;
                    case SkillConsts.RANDOM_MEMBER:
                        BattleLogic.SkillToRandom(skill.skillId, battler,
                            BattleDictionary.GetMemberUniqIds(BattlerEnum.BattlerType.Actor)).Subscribe(
                            __ =>
                            {
                                _battleModel.NextTurn.OnNext(Unit.Default);
                            });
                        break;
                }
            }));
        }

        private void BattlerSpriteSubscribe()
        {
            List<int> uniqIds = EnemyDataModel.Instance.UniqIds();
            uniqIds.AddRange(MemberDataModel.Instance.UniqIds());
            uniqIds.ForEach(uniqId =>
            {
                //アクティブ時
                _container.Add(_battlerSpriteModel.GetData(uniqId).Active.Subscribe(isActive =>
                {
                    if (isActive)
                    {
                        AnnounceTextView.Instance.TurnStartText(uniqId);
                        _battlerSpriteView.DeActiveOutline();
                        _battlerSpriteView.OnActiveOutline(uniqId);
                        //メンバーの場合
                        if (BattleDictionary.IsActor(uniqId))
                        {
                            _battleModel.ActiveUniqId = uniqId;
                            _skillsView.SkillView(uniqId);
                        }
                        //敵の場合
                        else
                        {
                            _skillsView.SkillHide();
                            _battleManager.AiAction(uniqId);
                        }
                    }
                    else
                    {
                        _battlerSpriteView.DeActiveOutline();
                        _skillsView.SkillHide();
                    }
                }));
                //HPに変動があった時
                _container.Add(_battlerSpriteModel.GetData(uniqId).Hp.Subscribe(value =>
                {
                    _battlerSpriteView.HpBarRefresh(uniqId);
                }));
                //死んだ時
                _container.Add(_battlerSpriteModel.GetData(uniqId).Dead.Subscribe(isDead =>
                {
                    if (isDead)
                    {
                        _battlerSpriteView.Dead(uniqId);
                        BattleLogic.Dead(BattlerDictionary.GetBattlerByUniqId(uniqId));
                        //PlaySe.GetInstance().Play("SE/Miscs/MonsterDie");
                    }
                }));
                //状態異常にかかった時
                _container.Add(_battlerSpriteModel.GetData(uniqId).Status.Subscribe(_ =>
                {
                    _battlerSpriteView.StatusIconRefresh(uniqId);
                }));
            });
        }

        public BattleBattlerSpriteModel BattlerSpriteModel
        {
            get => _battlerSpriteModel;
            set => _battlerSpriteModel = value;
        }

        public BattleBattlerSpriteView BattlerSpriteView
        {
            get => _battlerSpriteView;
            set => _battlerSpriteView = value;
        }

        public BattleSkillModel SkillModel
        {
            get => _skillModel;
            set => _skillModel = value;
        }

        public SkillsView SkillsView
        {
            get => _skillsView;
            set => _skillsView = value;
        }
    }
}