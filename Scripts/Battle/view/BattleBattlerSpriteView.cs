using System.Collections.Generic;
using System.Linq;
using Consts;
using Consts.Enums;
using DG.Tweening;
using Dictionary;
using Model;
using Serializable;
using TMPro;
using UniRx;
using UnityEngine;
using Utils;
using Image = UnityEngine.UI.Image;
using Object = UnityEngine.Object;
using Slider = UnityEngine.UI.Slider;

namespace Battle.view.GUI
{
    public struct BattlerSpriteStruct
    {
        public int UniqId;
        public Transform SpriteObject;
        public SpriteRenderer SpriteRenderer;
        public MaterialPropertyBlock MaterialPropertyBlock;
        public UiStruct Ui;
        public Transform StatusTransform;
        public List<StateStruct> Statuses;
    }

    public struct UiStruct
    {
        public Transform UiParent;
        public Transform HpBar;
        public Transform HpText;
        public Transform MpBar;
        public Transform MpText;
    }
    
    public class StateStruct
    {
        public BattlerSerializableStatus Status;
        public Transform StateTransform;
    }
    
    public class BattleBattlerSpriteView
    {
        private List<BattlerSpriteStruct> _sprites;
        public BattleBattlerSpriteView()
        {
            _sprites = new List<BattlerSpriteStruct>();
            CreateMemberSprite();
            CreateEnemySprite();
        }

        private void CreateMemberSprite()
        {
            var actorsTransformParent = GameObject.Find("Actors");
            var members = MemberDataModel.Instance.GetActorData();
            var memberSize = members.Count;
            foreach (var (battler, index) in members.Select((battler, index) => (battler, index)))
            {
                var monsterData = MonsterDicionary.GetMonsterData(battler.monsterId);
                GameObject memberObject = new GameObject();
                var spriteRenderer = CreateSprite(memberObject,battler.monsterId);
                var padding = 16 / memberSize;
                var posX = (padding * index) - (padding * (memberSize - 1) / 2) - (0.5f * memberSize);
                memberObject.transform.position = new Vector3(posX, -1.9f, 0);
                if (monsterData.imageData.actorScale != 0)
                {
                    memberObject.transform.localScale = new Vector3(monsterData.imageData.actorScale,
                        monsterData.imageData.actorScale, 1);
                }

                memberObject.transform.SetParent(actorsTransformParent.transform, false);
                memberObject.AddComponent<BoxCollider2D>();
                var ui = Object.Instantiate(
                    (GameObject) Resources.Load("Prefabs/Battle/MemberInfo"),memberObject.transform.position,Quaternion.identity,
                    GameObject.Find("FrontCanvas/EnemiesInfo").transform
                );
                var uiStruct = new UiStruct
                {
                    UiParent = ui.transform,
                    HpBar = ui.transform.Find("HPBar"),
                    HpText = ui.transform.Find("HPBar/ValueText"),
                    MpBar = ui.transform.Find("MPBar"),
                    MpText = ui.transform.Find("MPBar/ValueText")
                };
                uiStruct.HpText.GetComponent<TextMeshProUGUI>().SetText(battler.parameter.hp.ToString());
                uiStruct.MpText.GetComponent<TextMeshProUGUI>().SetText(battler.parameter.mp.ToString());
                float rate = (float) battler.parameter.hp / battler.parameter.maxHp;
                uiStruct.HpBar.GetComponent<Slider>().value = rate;
                rate = (float) battler.parameter.mp / battler.parameter.maxMp;
                uiStruct.MpBar.GetComponent<Slider>().value = rate;
                _sprites.Add(new BattlerSpriteStruct()
                {
                    UniqId = battler.uniqId,
                    SpriteObject = memberObject.transform,
                    SpriteRenderer = spriteRenderer,
                    MaterialPropertyBlock = new MaterialPropertyBlock(),
                    Ui = uiStruct,
                    StatusTransform = ui.transform.Find("Status"),
                    Statuses = new List<StateStruct>()
                });
            }
        }

        private void CreateEnemySprite()
        {
            var enemiesTransformParent = GameObject.Find("Enemies");
            var enemies = EnemyDataModel.Instance.Data;
            var enemiesSize = enemies.Count;

            foreach (var (battler, index) in enemies.Select((battler, index) => (battler, index)))
            {
                //敵のスプライトの作成
                GameObject enemyObject = new GameObject();
                enemyObject.name = "Enemy";
                var spriteRenderer = CreateSprite(enemyObject,battler.monsterId);
                var padding = 20 / enemiesSize;
                var posX = (padding * index) - (padding * (enemiesSize - 1) / 2);
                enemyObject.transform.position = new Vector3(posX, 1.5f, 0);
                enemyObject.transform.SetParent(enemiesTransformParent.transform, false);
                enemyObject.AddComponent<BoxCollider2D>();
                //敵のinfoを作成
                var ui = Object.Instantiate(
                    (GameObject) Resources.Load("Prefabs/Battle/EnemyInfo"),enemyObject.transform.position + new Vector3(0,-1f,0),Quaternion.identity,
                    GameObject.Find("FrontCanvas/EnemiesInfo").transform
                    );
                var uiStruct = new UiStruct
                {
                    UiParent = ui.transform,
                    HpBar = ui.transform.Find("HPBar"),
                    HpText = ui.transform.Find("HPBar/ValueText")
                };
                uiStruct.HpText.GetComponent<TextMeshProUGUI>().SetText(battler.parameter.hp.ToString());
                _sprites.Add(new BattlerSpriteStruct()
                {
                    UniqId = battler.uniqId,
                    SpriteObject = enemyObject.transform,
                    SpriteRenderer = spriteRenderer,
                    MaterialPropertyBlock = new MaterialPropertyBlock(),
                    Ui = uiStruct,
                    StatusTransform = ui.transform.Find("Status"),
                    Statuses = new List<StateStruct>()
                });
            }
        }
        
        private SpriteRenderer CreateSprite(GameObject targetObject, string monsterId)
        {
            var monsterData = MonsterDicionary.GetMonsterData(monsterId);
            var spriteRenderer = targetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = MonsterDicionary.GetSprite(monsterId);
            spriteRenderer.material = Resources.Load("Material/AllinDefaultMaterials", typeof(Material)) as Material;
            spriteRenderer.sortingOrder = 2;
            return spriteRenderer;
        }

        public void OnActiveOutline(int uniqId)
        {
            var sprite = GetSprite(uniqId);
            var spriteRenderer = sprite.SpriteRenderer;
            var materialPropertyBlock = sprite.MaterialPropertyBlock;

            spriteRenderer.material.EnableKeyword(ShaderProperties.OUTBASE_ON);
            spriteRenderer.GetPropertyBlock(materialPropertyBlock);
            if (BattleDictionary.IsActor(sprite.UniqId))
                materialPropertyBlock.SetColor(ShaderProperties.OutlineColor, Color.yellow);
            else
                materialPropertyBlock.SetColor(ShaderProperties.OutlineColor, new Color(1f, 0.2758853f, 0.2f, 1f));
            spriteRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void DeActiveOutline()
        {
            _sprites.ForEach(x =>
            {
                x.SpriteRenderer.material.DisableKeyword(ShaderProperties.OUTBASE_ON);
            });
        }

        /// <summary>
        /// 対象選択時のMaterial操作
        /// </summary>
        /// <param name="spriteStruct"></param>
        public void SelectOutline(BattlerSpriteStruct spriteStruct)
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(spriteStruct.UniqId);
            spriteStruct.SpriteRenderer.material.EnableKeyword(ShaderProperties.OUTBASE_ON);
            spriteStruct.SpriteRenderer.GetPropertyBlock(spriteStruct.MaterialPropertyBlock);
            if (battler.battlerType == BattlerEnum.BattlerType.Actor)
                spriteStruct.MaterialPropertyBlock.SetColor(ShaderProperties.OutlineColor, Color.green);
            else
                spriteStruct.MaterialPropertyBlock.SetColor(ShaderProperties.OutlineColor, Color.red);
            spriteStruct.SpriteRenderer.SetPropertyBlock(spriteStruct.MaterialPropertyBlock);
        }
        
        public void DeSelectOutline(BattlerSpriteStruct spriteStruct)
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(spriteStruct.UniqId);
            spriteStruct.SpriteRenderer.material.DisableKeyword(ShaderProperties.OUTBASE_ON);
        }

        /// <summary>
        /// バトラータイプが同一の選択時のアウトラインを消す
        /// </summary>
        public void DeSelectOutlineByBattlerType(BattlerEnum.BattlerType battlerType)
        {
            _sprites.ForEach(x =>
            {
                var battler = BattlerDictionary.GetBattlerByUniqId(x.UniqId);
                if (battler.battlerType == battlerType)
                {
                    x.SpriteRenderer.material.DisableKeyword(ShaderProperties.OUTBASE_ON);
                }
            });
        }

        public void HpBarRefresh(int uniqId)
        {
            var sprite = GetSprite(uniqId);
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            float rate = (float) battler.parameter.hp /
                         battler.parameter.maxHp;
            if (rate < 0)
                rate = 0;
            //sprite.Ui.DoAnimation?.Kill();
            var slider = sprite.Ui.HpBar.GetComponent<Slider>();
            DOTween.To(() => slider.value, (x) => slider.value = x, rate, 0.6f)
                .Play();
            int hp = int.Parse(sprite.Ui.HpText.GetComponent<TextMeshProUGUI>().text);
            DOTween.To(() => hp, (x) => sprite.Ui.HpText.GetComponent<TextMeshProUGUI>().SetText(x.ToString()),
                battler.parameter.hp, 0.6f).Play();
        }

        public void Dead(int uniqId)
        {
            var sprite = GetSprite(uniqId);
            if (BattleDictionary.IsActor(uniqId))
            {
                sprite.SpriteRenderer.material.EnableKeyword(ShaderProperties.GREYSCALE_ON);
            }
            else
            {
                sprite.SpriteRenderer.material.DisableKeyword(ShaderProperties.SHADOW_ON);
                sprite.SpriteRenderer.material.EnableKeyword(ShaderProperties.FADE_ON);
                sprite.Ui.HpBar.gameObject.SetActive(false);
                float fade = 0f;
                DOTween.To(() => fade,
                    (x) => fade = x, 1f, 0.8f).Play().OnUpdate(() =>
                {
                    sprite.SpriteRenderer.GetPropertyBlock(sprite.MaterialPropertyBlock);
                    sprite.MaterialPropertyBlock.SetFloat(ShaderProperties.FadeAmount, fade);
                    sprite.SpriteRenderer.SetPropertyBlock(sprite.MaterialPropertyBlock);
                }).OnComplete(
                    () =>
                    {
                        sprite.SpriteRenderer.enabled = false;
                    });
            }
        }

        /// <summary>
        /// 状態異常アイコンの追加
        /// </summary>
        public void StatusIconRefresh(int uniqId)
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var sprite = GetSprite(uniqId);
            List<StateStruct> deletes = new List<StateStruct>();
            //削除判定
            sprite.Statuses.ForEach(stateStruct =>
            {
                if (battler.status.Find(state => state.type == stateStruct.Status.type) == null)
                    deletes.Add(stateStruct);
            });
            if (deletes.Count != 0)
            {
                RemoveStateAnimation(uniqId, deletes).Subscribe(_ => { AddStatusIcon(uniqId); });
            }
            else
            {
                ObservableUtils.Timer(0).Subscribe(_ => { AddStatusIcon(uniqId); });
            }
        }

        private void AddStatusIcon(int uniqId)
        {
            var battler = BattlerDictionary.GetBattlerByUniqId(uniqId);
            var sprite = GetSprite(uniqId);
            battler.status.ForEach(state =>
            {
                //まだ画面にステータスを表示させてない場合
                var stateStruct = sprite.Statuses.Find(f => f.Status.type == state.type);
                if (stateStruct == null)
                {
                    //ステータス表示させる必要がある時のみ
                    if (StatusDictionary.IsViewStatus(state.type))
                    {
                        StateStruct addStateStruct = new StateStruct
                        {
                            Status = state,
                            StateTransform = Object.Instantiate((GameObject) Resources.Load("Prefabs/Battle/StateIcon"))
                                .transform
                        };
                        var icon = addStateStruct.StateTransform.transform;
                        
                        icon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Icons/Status/" + StatusDictionary.StatusIconName(state.type));
                        /*icon.GetComponent<Image>().sprite =
                            AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/Sprites/Icons/Status/" +
                                                                  StatusDictionary.StatusIconName(state.type));*/


                        addStateStruct.StateTransform.transform.SetParent(sprite.StatusTransform,
                            false);
                        sprite.Statuses.Add(addStateStruct);
                        StateIconValueRefresh(addStateStruct, state);
                        AddStateAnimation(uniqId, addStateStruct);
                    }
                }
                //すでに画面に表示させている場合は更新する
                else
                {
                    StateIconValueRefresh(stateStruct, state);
                }
            });
        }

        private void StateIconValueRefresh(StateStruct stateStruct, BattlerSerializableStatus state)
        {
            //数値表示させる場合
            if (StatusDictionary.IsValueView(state.type))
            {
                var text =  stateStruct.StateTransform.Find("Value");
                text.GetComponent<TextMeshProUGUI>().text = state.value.ToString();
                text.GetComponent<TextMeshProUGUI>().color = StatusDictionary.StatusValueColor(state.type);
            }
            //ターン数表示させる場合
            if (StatusDictionary.IsTurnStatus(state.type))
            {
                var text =  stateStruct.StateTransform.Find("TurnValue");
                text.GetComponent<TextMeshProUGUI>().text = state.turn.ToString();
            }
        }

        private void AddStateAnimation(int uniqId, StateStruct state)
        {
            var sprite = GetSprite(uniqId);
            state.StateTransform.localPosition = state.StateTransform.localPosition + new Vector3((-40f * (sprite.Statuses.Count - 1)), 10f, 0);
            state.StateTransform.DOLocalMoveY(0, 0.4f).Play();
            state.StateTransform.GetComponent<CanvasGroup>().alpha = 0;
            DOTween.To(() => state.StateTransform.GetComponent<CanvasGroup>().alpha,
                (x) => state.StateTransform.GetComponent<CanvasGroup>().alpha = x, 1f, 0.4f).Play();

        }

        private AsyncSubject<Unit> RemoveStateAnimation(int uniqId, List<StateStruct> stateStructs)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var sprite = GetSprite(uniqId);
            stateStructs.ForEach(state =>
            {
                DOTween.To(() => state.StateTransform.GetComponent<CanvasGroup>().alpha,
                    (x) => state.StateTransform.GetComponent<CanvasGroup>().alpha = x, 0f, 0.4f).Play();
                state.StateTransform.DOMoveY(1, 0.4f).Play();
            });
            ObservableUtils.Timer(500).Subscribe(_ =>
            {
                stateStructs.ForEach(state =>
                {
                    sprite.Statuses.Remove(state);
                    if (state.StateTransform != null)
                    {
                        Object.Destroy(state.StateTransform.gameObject);
                    }
                });
                foreach (var (icon, index) in sprite.Statuses.Select((x, index) => (x, index)))
                {
                    float posX = 60 + -40f * (sprite.Statuses.Count - 1);
                    icon.StateTransform.DOLocalMoveX(posX,0.3f).Play();
                }
                ObservableUtils.Timer(400).Subscribe(__ =>
                {
                    subject.OnNext(Unit.Default);
                    subject.OnCompleted();
                });
            });
            return subject;
        }

        public void HitEffect(int uniqId, Color color)
        {
            var sprite = GetSprite(uniqId);
            sprite.SpriteRenderer.material.EnableKeyword(ShaderProperties.HITEFFECT_ON);
            sprite.SpriteRenderer.material.EnableKeyword(ShaderProperties.SHAKE_ON);
            
            sprite.SpriteRenderer.GetPropertyBlock(sprite.MaterialPropertyBlock);
            sprite.MaterialPropertyBlock.SetColor(ShaderProperties.HIT_EFFECT_COLOR, color);
            sprite.MaterialPropertyBlock.SetFloat(ShaderProperties.HIT_EFFECT_BLEND, 0f);
            sprite.MaterialPropertyBlock.SetFloat(ShaderProperties.SHAKE, 0f);
            sprite.SpriteRenderer.SetPropertyBlock(sprite.MaterialPropertyBlock);
            
            float value = 0f;
            DOTween.To(() => value,
                (x) => value = x, 1f, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Flash).Play().OnUpdate(() =>
            {
                sprite.SpriteRenderer.GetPropertyBlock(sprite.MaterialPropertyBlock);
                sprite.MaterialPropertyBlock.SetFloat(ShaderProperties.HIT_EFFECT_BLEND, value);
                sprite.SpriteRenderer.SetPropertyBlock(sprite.MaterialPropertyBlock);
            }).OnComplete(
                () =>
                {
                    sprite.SpriteRenderer.material.DisableKeyword(ShaderProperties.HITEFFECT_ON);
                });
            float shakeValue = 0;
            DOTween.To(() => 0,
                (x) => shakeValue = x, 5f, 0.1f).Play().OnUpdate(() =>
            {
                sprite.SpriteRenderer.GetPropertyBlock(sprite.MaterialPropertyBlock);
                sprite.MaterialPropertyBlock.SetFloat(ShaderProperties.SHAKE, shakeValue);
                sprite.SpriteRenderer.SetPropertyBlock(sprite.MaterialPropertyBlock);
            }).OnComplete(
                () =>
                {
                    sprite.SpriteRenderer.material.DisableKeyword(ShaderProperties.SHAKE_ON);
                });
        }

        public AsyncSubject<Unit> Hide()
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            _sprites.ForEach(x =>
            {
                x.SpriteRenderer.DOFade(0, 0.4f).Play();
                x.Ui.UiParent.GetComponent<CanvasGroup>().DOFade(0, 0.4f).Play();
            });
            ObservableUtils.Timer(60).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public BattlerSpriteStruct GetSprite(int uniqId)
        {
            return _sprites.Find(x => x.UniqId == uniqId);
        }

        public List<BattlerSpriteStruct> Sprites
        {
            get => _sprites;
        }
    }
}