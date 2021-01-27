using System.Collections.Generic;
using System.Linq;
using Consts;
using Consts.Enums;
using Dictionary;
using Model;
using Serializable;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Battle
{
    public class BattleView
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
        
        private List<BattlerSpriteStruct> _sprites;
        
        public void Initialize()
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
        
        /// <summary>
        /// アクティブ時のアウトライン
        /// </summary>
        /// <param name="uniqId"></param>
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
        
        public void ExitSelectOutline(BattlerSpriteStruct spriteStruct)
        {
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