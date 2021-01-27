using System.Collections.Generic;
using System.Linq;
using Battler;
using Consts;
using Consts.Enums;
using Dictionary;
using Dungeon;
using Item.Logic;
using Model;
using Model.Person;
using Model.Structs;
using Serializable;
using Structs;
using UniRx;
using UnityEngine;
using Utils;
using View;
using Random = UnityEngine.Random;

namespace Battle.Logic
{
    public class BattleLogic
    {
        /// <summary>
        /// 単体の対象に向けてスキルを使う
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="fromBattler"></param>
        /// <param name="toBattler"></param>
        /// <returns></returns>
        public static List<SkillDamage> SkillToBattler(string skillId, BattlerSerializable fromBattler,
            BattlerSerializable toBattler)
        {
            List<SkillDamage> skillDamages = new List<SkillDamage>();
            var skill = SkillsDicionary.GetSkillById(skillId);
            foreach (var (x, i) in skill.effect.value.Select((x, i) => (x, i)))
            {
                var skillDamage = new SkillDamage();
                var damage = SkillLogic.GetTotalDamageByTarget(x, fromBattler);
                skillDamage.damage = damage;
                skillDamage.valueTarget = x.valueTarget;
                skillDamage.turn = x.turn;
                skillDamages.Add(skillDamage);
                skillDamage.damage = CalcDamage(skillDamage, toBattler);
            }

            return skillDamages;
        }

        /// <summary>
        /// 回避判定
        /// 味方に対するスキル、もしくは必中でない限り回避チェックを行う
        /// 物理属性が含まれる場合は命中率-回避率
        /// 魔法属性のみの場合は100%-魔法回避率
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="fromBattler"></param>
        /// <param name="toBattler"></param>
        /// <returns></returns>
        private static bool HitCheck(string skillId, BattlerSerializable fromBattler,
            BattlerSerializable toBattler)
        {
            var skill = SkillsDicionary.GetSkillById(skillId);
            if (skill.target == SkillConsts.MEMBER || skill.target == SkillConsts.ALL_MEMBER ||
                skill.target == SkillConsts.RANDOM_MEMBER)
            {
                return true;
            }

            float hitRate = fromBattler.parameter.hit;
            hitRate = hitRate * (skill.effect.hitRate / 100);
            if (skill.effect.elements.Contains(SkillConsts.ELEMENTS_PHYSICS))
            {
                hitRate -= toBattler.parameter.dodge;
            }
            else if (skill.effect.elements.Contains(SkillConsts.ELEMENTS_MAGIC))
            {
                hitRate -= toBattler.parameter.magicDodge;
            }

            if (Random.Range(0, 100) < hitRate)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ダメージと敵ステータスを比較してダメージ計算し反映する
        /// </summary>
        /// <returns></returns>
        private static int CalcDamage(SkillDamage damage, BattlerSerializable toBattler)
        {
            var _damage = damage.damage;
            switch (damage.valueTarget)
            {
                //通常ダメージ計算
                // TODO
                case SkillValueTarget.HP:
                    _damage = _damage - toBattler.parameter.def;
                    //ダメージは25%以下に減らない
                    if (_damage < damage.damage * 0.25)
                    {
                        _damage = (int) (damage.damage * 0.25);
                    }

                    //ダメージのブレ
                    _damage += Random.Range(_damage, (int) (_damage * 0.1));
                    SkillDamage(_damage, toBattler);
                    return _damage;
                //毒計算
                case SkillValueTarget.TARGET_POISON:
                    damage.damage = damage.damage - (int) (damage.damage * (toBattler.resistRate.poison / 100));
                    StatusDamage(damage, toBattler);
                    return damage.damage;
                //睡眠判定
                case SkillValueTarget.SLEEP:
                    if (IsStateJudgment(damage.damage, toBattler))
                    {
                        damage.damage = 1;
                        SetState(SkillValueTarget.SLEEP, toBattler, 20);
                    }
                    else
                    {
                        damage.isResist = true;
                        damage.damage = 0;
                    }

                    return damage.damage;
                //再生計算
                case SkillValueTarget.TARGET_REGENERATION_HP:
                    StatusDamage(damage, toBattler);
                    return damage.damage;
            }

            return damage.damage;
        }

        private static bool IsStateJudgment(int damage,  BattlerSerializable toBattler)
        {
            if (Random.Range(0, 100) < damage - toBattler.parameter.res)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ダメージ処理
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="battler"></param>
        public static void SkillDamage(int damage, BattlerSerializable battler)
        {
            Damage(damage, battler);
            //睡眠状態なら睡眠状態を解除する
            if (DeadCheck(battler) == false && StatusDictionary.IsSleep(battler))
            {
                RemoveStatus(StatusConsts.SLEEP, battler);
            }
        }

        public static void SustainDamage(int damage, BattlerSerializable battler)
        {
            Damage(damage,battler);
        }

        private static void Damage(int damage, BattlerSerializable battler)
        {
            battler.parameter.hp -= damage;
            if (battler.parameter.hp < 0)
                battler.parameter.hp = 0;
            BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Hp.OnNext(battler.parameter.hp);
        }

        private static void StatusDamage(SkillDamage damage, BattlerSerializable battler)
        {
            var status = battler.status.Find(x => x.type == damage.valueTarget);
            if (status == null)
            {
                BattlerSerializableStatus battlerSerializableStatus = new BattlerSerializableStatus();
                battlerSerializableStatus.type = damage.valueTarget;
                battlerSerializableStatus.value = damage.damage;
                if (StatusDictionary.IsTurnStatus(damage.valueTarget))
                {
                    battlerSerializableStatus.turn = damage.turn;
                }
                battler.status.Add(battlerSerializableStatus);
                BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Status.OnNext(Unit.Default);
            }
            else
            {
                status.value += damage.damage;
            }
        }

        public static void SetState(string type, BattlerSerializable battler, int turn = 0)
        {
            var state = battler.status.Find(x => x.type == type);
            if (state == null)
            {
                BattlerSerializableStatus battlerSerializableStatus = new BattlerSerializableStatus();
                battlerSerializableStatus.type = type;
                battlerSerializableStatus.value = 1;
                battlerSerializableStatus.turn = turn;
                battler.status.Add(battlerSerializableStatus);
                BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Status.OnNext(Unit.Default);
            }
        }

        public static bool DeadCheck(BattlerSerializable battler)
        {
            //死亡判定
            if (battler.parameter.hp <= 0)
            {
                battler.parameter.hp = 0;
                return true;
            }

            return false;
        }

        public static bool AllEnemyDeadCheck()
        {
            var battlers = BattleDictionary.GetRivalBattlers(BattlerEnum.BattlerType.Actor);
            foreach (var battler in battlers)
            {
                if (DeadCheck(battler) == false)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Dead(BattlerSerializable battler)
        {
            //すべての状態異常を解除する
            CleanAllStatus(battler);
            SetState(StatusConsts.DEAD, battler);
        }

        private static void RemoveStatus(string type,BattlerSerializable battler)
        {
            var state = battler.status.Find(x => x.type == type);
            if (state != null)
            {
                battler.status.Remove(state);
                BattlePresenter.GetInstance().BattlerSpriteModel.GetData(battler.uniqId).Status.OnNext(Unit.Default);
                StatusLogic.RemoveAnnounce(type, battler);
            }
        }
        
        private static void CleanAllStatus(BattlerSerializable battler)
        {
            battler.status.Clear();
        }

        public static AsyncSubject<Unit> SkillByAi(AiSelectSkillResultSerializableData ai, BattlerSerializable fromBattler)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            if (ai == null)
            {
                ObservableUtils.AsyncSubjectTimeZeroCompleted(subject);
            }
            else
            {
                var skill = SkillsDicionary.GetSkillById(ai.SkillId);
                if (SkillsDicionary.IsAll(skill))
                {
                    List<BattlerSerializable> toBattlers = new List<BattlerSerializable>();
                    ai.TargetUniqIds.ForEach(x => { toBattlers.Add(BattlerDictionary.GetBattlerByUniqId(x)); });
                    SkillToAll(ai.SkillId, fromBattler, toBattlers).Subscribe(_ =>
                    {
                        subject.OnNext(Unit.Default);
                        subject.OnCompleted();
                    });
                }
                else
                {
                    var uniqId = ai.TargetUniqIds.First();
                    var toBattler = BattlerDictionary.GetBattlerByUniqId(uniqId);
                    var targetTransform = BattleDictionary.GetTransformByUniqId(uniqId);
                    SkillToSingle(ai.SkillId, fromBattler, toBattler).Subscribe(_ =>
                    {
                        subject.OnNext(Unit.Default);
                        subject.OnCompleted();
                    });
                }
            }
            return subject;
        }
        
        /// <summary>
        /// 全体にスキル発動
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="fromBattler"></param>
        /// <param name="toBattlers"></param>
        /// <returns></returns>
        public static AsyncSubject<Unit> SkillToAll(string skillId, BattlerSerializable fromBattler,
            List<BattlerSerializable> toBattlers)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            List<SkillDamages> damageses = new List<SkillDamages>();
            var skill = SkillsDicionary.GetSkillById(skillId);
            List<Transform> targetTransforms = new List<Transform>();
            toBattlers.ForEach(battler =>
            {
                var targetTransform = BattleDictionary.GetTransformByUniqId(battler.uniqId);
                var isHit = HitCheck(skillId, fromBattler, battler);
                List<SkillDamage> damages = new List<SkillDamage>();
                if (isHit)
                {
                    damages = SkillToBattler(skillId, fromBattler, battler);
                }

                var isDead = DeadCheck(battler);
                damageses.Add(new SkillDamages()
                {
                    SkillDamage = damages,
                    targetUniqId = battler.uniqId,
                    isHit = isHit,
                    isDead = isDead
                });
                targetTransforms.Add(targetTransform);
            });
            //ダメージテキストの表示
            AnnounceTextView.Instance.AddDamageText(fromBattler, skill.name, damageses);
            
            EffectManager.Instance.SkillToAll(skillId, damageses).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        /// <summary>
        /// ランダムの対象にスキル発動
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="fromBattler"></param>
        /// <param name="uniqIds"></param>
        /// <returns></returns>
        public static AsyncSubject<Unit> SkillToRandom(string skillId, BattlerSerializable fromBattler,
            List<int> uniqIds)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            List<SkillDamages> damageses = new List<SkillDamages>();

            var skill = SkillsDicionary.GetSkillById(skillId);

            var strikeSize = 1;
            if (skill.strikeSize.min != 0 && skill.strikeSize.max != 0)
            {
                strikeSize = Random.Range(skill.strikeSize.min, skill.strikeSize.max);
            }
            List<BattlerSerializable> toBattlers = new List<BattlerSerializable>();
            uniqIds.ForEach(uniqId =>
            {
                toBattlers.Add(BattlerDictionary.GetBattlerByUniqId(uniqId));
            });
            for (int i = 0; i < strikeSize; i++)
            {
                var toBattler = BattleDictionary.GetAliveBattlerByRandom(toBattlers);
                if (toBattler == null)
                    break;
                var isHit = HitCheck(skillId, fromBattler, toBattler);
                List<SkillDamage> damages = new List<SkillDamage>();
                if (isHit)
                {
                    damages = SkillToBattler(skillId, fromBattler, toBattler);
                }

                var isDead = DeadCheck(toBattler);
                damageses.Add(new SkillDamages
                {
                    SkillDamage = damages,
                    targetUniqId = toBattler.uniqId,
                    isHit = isHit,
                    isDead = isDead
                });
            }
            //ダメージテキストの表示
            AnnounceTextView.Instance.AddDamageText(fromBattler, skill.name, damageses);
            
            EffectManager.Instance.SkillToTarget(skillId, damageses).Subscribe(_ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });
            return subject;
        }

        public static AsyncSubject<Unit> SkillToSingle(string skillId, BattlerSerializable fromBattler,
            BattlerSerializable toBattler)
        {
            AsyncSubject<Unit> subject = new AsyncSubject<Unit>();
            var skill = SkillsDicionary.GetSkillById(skillId);
            var strikeSize = 1;
            if (skill.strikeSize.min != 0 && skill.strikeSize.max != 0)
            {
                strikeSize = Random.Range(skill.strikeSize.min, skill.strikeSize.max);
            }

            List<SkillDamages> damageses = new List<SkillDamages>();
            for (int i = 0; i < strikeSize; i++)
            {
                var isHit = HitCheck(skillId, fromBattler, toBattler);
                List<SkillDamage> damage = new List<SkillDamage>();
                if (isHit)
                {
                    damage = SkillToBattler(skillId, fromBattler, toBattler);
                }

                var isDead = DeadCheck(toBattler);
                damageses.Add(new SkillDamages
                {
                    SkillDamage = damage,
                    targetUniqId = toBattler.uniqId,
                    isHit = isHit,
                    isDead = isDead
                });
            }

            //ダメージテキストの表示
            AnnounceTextView.Instance.AddDamageText(fromBattler, skill.name, damageses);
            
            EffectManager.Instance.SkillToTarget(skillId, damageses).Subscribe(__ =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            });

            return subject;
        }

        private static void WakeUpCheck(List<SkillDamages> damageses)
        {
            foreach (var damagese in damageses)
            {
                var battler = BattlerDictionary.GetBattlerByUniqId(damagese.targetUniqId);
                if (StatusDictionary.IsSleep(battler))
                {
                    if (damagese.isHit)
                    {
                        //HPにダメージを与えたか検索
                        var damage = damagese.SkillDamage.Find(x => x.valueTarget == SkillValueTarget.HP);
                        if (damage != null && damage.damage > 0)
                        {
                            RemoveStatus(StatusConsts.SLEEP, battler);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 報酬付与
        /// </summary>
        public static List<LootItemStruct> BattleResult()
        {
            List<LootItemStruct> loots = new List<LootItemStruct>();
            int gold = 0;
            //モンスター固有アイテム抽選
            EnemyDataModel.Instance.Data.ForEach(enemy =>
            {
                loots.AddRange(LootLogic.MonsterLootChoice(enemy.monsterId));
                //ゴールド計算 TODO
                gold += enemy.level * 10;
            });
            //ダンジョン固有アイテム抽選 TODO 確率
            loots.AddRange(LootLogic.DungeonLootChoice(0.1f));
            
            //アイテム獲得処理
            List<ItemStruct> items = new List<ItemStruct>();
            loots.ForEach(loot =>
            {
                var item = ItemLogic.CreateItemFromLoot(loot);
                items.Add(item);
            });
            StockItemDataModel.GetInstance().AddItems(items);
            
            //ゴールド獲得処理
            loots.Add(new LootItemStruct()
            {
                Amount = gold,
                ItemId = ItemConsts.GOLD
            });
            StockItemDataModel.GetInstance().AddGold(gold);

            //レベルアップ処理
            //TODO レベルアップの仕様検討
            //一般戦闘の場合は+1 エリート敵の場合は+2
            int levelUpAmount = 1;
            loots.Add(new LootItemStruct()
            {
                Amount = levelUpAmount,
                ItemId = ItemConsts.LEVEL
            });
            BattlerLogic.LevelUp(levelUpAmount);

            return loots;
        }

        public static EnemiesStruct EnemiesChoice()
        {
            var dungeonData = DungeonDictionary.GetDungeonMapData(DungeonDataModel.Instance.DungeonId);
            var floor = DungeonDataModel.Instance.Floor;
            
            //敵の数
            int amountMin = 0;
            int amountMax = 0;
            var enemyAmountData = dungeonData.enemyAmount.FirstOrDefault(x => x.border > floor);
            if (enemyAmountData == null)
                enemyAmountData = dungeonData.enemyAmount.Last();
            amountMin = enemyAmountData.min;
            amountMax = enemyAmountData.max;
                
            //敵の種類パターン
            var enemiesData = dungeonData.enemies.FirstOrDefault(x => x.border > floor);
            if (enemiesData == null)
                enemiesData = dungeonData.enemies.Last();
            
            WeightChoice<string> choice = new WeightChoice<string>();
            enemiesData.monsters.ForEach(enemy =>
            {
                choice.Add(1, enemy);
            });
            
            var amount = Random.Range(amountMin, amountMax);
            List<string> enemies = new List<string>();
            for (var x = 0; x < amount; x++)
            {
                enemies.Add(choice.ChoiceOne());
            }

            return new EnemiesStruct()
            {
                Enemies = enemies,
                AddLevel = enemiesData.addLevel
            };
        }

    }
}