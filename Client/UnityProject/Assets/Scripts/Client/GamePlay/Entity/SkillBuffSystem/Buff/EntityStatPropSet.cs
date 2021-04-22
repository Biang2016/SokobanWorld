﻿using System;
using System.Collections.Generic;
using BiangLibrary.CloneVariant;
using BiangLibrary.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Profiling;

[Serializable]
public class EntityStatPropSet
{
    public static List<EntityStatType> EntityStatTypeEnumList = new List<EntityStatType>();
    public static List<EntityPropertyType> EntityPropertyTypeEnumList = new List<EntityPropertyType>();

    public EntityStatPropSet()
    {
        InitDelegates();
    }

    internal Entity Entity;

    public Dictionary<EntityStatType, EntityStat> StatDict = new Dictionary<EntityStatType, EntityStat>(20);
    public Dictionary<EntityPropertyType, EntityProperty> PropertyDict = new Dictionary<EntityPropertyType, EntityProperty>(20);

    #region 财产

    [BoxGroup("财产")]
    [LabelText("@\"拾取吸附半径\t\"+CollectDetectRadius")]
    public EntityProperty CollectDetectRadius = new EntityProperty(EntityPropertyType.CollectDetectRadius);

    [BoxGroup("财产")]
    [LabelText("@\"当前金子\t\"+Gold")]
    public EntityStat Gold = new EntityStat(EntityStatType.Gold);

    [BoxGroup("财产")]
    [LabelText("@\"火元素碎片\t\"+FireElementFragment")]
    public EntityStat FireElementFragment = new EntityStat(EntityStatType.FireElementFragment);

    [BoxGroup("财产")]
    [LabelText("@\"火元素碎片上限\t\"+MaxFireElementFragment")]
    public EntityProperty MaxFireElementFragment = new EntityProperty(EntityPropertyType.MaxFireElementFragment);

    [BoxGroup("财产")]
    [LabelText("@\"冰元素碎片\t\"+IceElementFragment")]
    public EntityStat IceElementFragment = new EntityStat(EntityStatType.IceElementFragment);

    [BoxGroup("财产")]
    [LabelText("@\"冰元素碎片上限\t\"+MaxIceElementFragment")]
    public EntityProperty MaxIceElementFragment = new EntityProperty(EntityPropertyType.MaxIceElementFragment);

    [BoxGroup("财产")]
    [LabelText("@\"电元素碎片\t\"+LightningElementFragment")]
    public EntityStat LightningElementFragment = new EntityStat(EntityStatType.LightningElementFragment);

    [BoxGroup("财产")]
    [LabelText("@\"电元素碎片上限\t\"+MaxLightningElementFragment")]
    public EntityProperty MaxLightningElementFragment = new EntityProperty(EntityPropertyType.MaxLightningElementFragment);

    #endregion

    #region 耐久

    [BoxGroup("耐久")]
    [LabelText("@\"血量耐久\t\"+HealthDurability")]
    public EntityStat HealthDurability = new EntityStat(EntityStatType.HealthDurability);

    [BoxGroup("耐久")]
    [LabelText("@\"血量耐久上限\t\"+MaxHealthDurability")]
    public EntityProperty MaxHealthDurability = new EntityProperty(EntityPropertyType.MaxHealthDurability);

    [BoxGroup("耐久")]
    [LabelText("@\"坠落留存率%\t\"+DropFromAirSurviveProbabilityPercent")]
    public EntityStat DropFromAirSurviveProbabilityPercent = new EntityStat(EntityStatType.DropFromAirSurviveProbabilityPercent);

    #endregion

    #region 碰撞

    [BoxGroup("碰撞")]
    [LabelText("@\"角色碰撞伤害抵消\t\"+ActorCollideDamageDefense")]
    public EntityProperty ActorCollideDamageDefense = new EntityProperty(EntityPropertyType.ActorCollideDamageDefense);

    [BoxGroup("碰撞")]
    [LabelText("@\"箱子碰撞减少自身耐久\t\"+BoxCollideDamageSelf")]
    public EntityProperty BoxCollideDamageSelf = new EntityProperty(EntityPropertyType.BoxCollideDamageSelf);

    [BoxGroup("碰撞")]
    [LabelText("@\"碰撞伤害\t\"+CollideDamage")]
    public EntityProperty CollideDamage = new EntityProperty(EntityPropertyType.CollideDamage);

    [BoxGroup("碰撞")]
    [LabelText("@\"被冻结后碰撞伤害\t\"+FrozenBeCollideDamage")]
    public EntityProperty FrozenBeCollideDamage = new EntityProperty(EntityPropertyType.FrozenBeCollideDamage);

    [BoxGroup("碰撞")]
    [LabelText("@\"X轴碰撞伤害\t\"+CollideDamageX")]
    public EntityProperty CollideDamageX = new EntityProperty(EntityPropertyType.CollideDamageX);

    [BoxGroup("碰撞")]
    [LabelText("@\"Z轴碰撞伤害\t\"+CollideDamageZ")]
    public EntityProperty CollideDamageZ = new EntityProperty(EntityPropertyType.CollideDamageZ);

    [BoxGroup("碰撞")]
    [LabelText("@\"被碰撞硬直ms\t\"+BeCollidedHitStopDuration")]
    public EntityProperty BeCollidedHitStopDuration = new EntityProperty(EntityPropertyType.BeCollidedHitStopDuration);

    public EntityProperty GetCollideDamageByAxis(Box.KickAxis axis)
    {
        if (axis == Box.KickAxis.X) return CollideDamageX;
        if (axis == Box.KickAxis.Z) return CollideDamageZ;
        else return CollideDamage;
    }

    #endregion

    #region 爆炸

    [BoxGroup("爆炸")]
    [LabelText("@\"爆炸伤害抵消\t\"+ExplodeDamageDefense")]
    public EntityProperty ExplodeDamageDefense = new EntityProperty(EntityPropertyType.ExplodeDamageDefense);

    #endregion

    #region 冰冻

    [BoxGroup("冰冻")]
    [LabelText("@\"冰冻抗性\t\"+FrozenResistance")]
    public EntityProperty FrozenResistance = new EntityProperty(EntityPropertyType.FrozenResistance);

    [BoxGroup("冰冻")]
    [LabelText("@\"冰冻累积值\t\"+FrozenValue")]
    public EntityStat FrozenValue = new EntityStat(EntityStatType.FrozenValue);

    [BoxGroup("冰冻")]
    [LabelText("@\"冰冻等级\t\"+FrozenLevel")]
    public EntityStat FrozenLevel = new EntityStat(EntityStatType.FrozenLevel);

    internal int FrozenValuePerLevel => Mathf.RoundToInt(((float) FrozenValue.MaxValue / FrozenLevel.MaxValue));

    public bool IsFrozen => FrozenLevel.Value > 1; // 1级不算冰冻， 2级~4级冰冻对应三个模型

    [BoxGroup("冰冻")]
    [LabelText("@\"冰冻伤害抵消\t\"+FrozenDamageDefense")]
    public EntityProperty FrozenDamageDefense = new EntityProperty(EntityPropertyType.FrozenDamageDefense);

    [BoxGroup("冰冻")]
    [LabelText("@\"冰冻持续特效\t\"+FrozenFX")]
    public FXConfig FrozenFX = new FXConfig();

    #endregion

    #region 燃烧

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧抗性\t\"+FiringResistance")]
    public EntityProperty FiringResistance = new EntityProperty(EntityPropertyType.FiringResistance);

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧累积值\t\"+FiringValue")]
    public EntityStat FiringValue = new EntityStat(EntityStatType.FiringValue);

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧等级\t\"+FiringLevel")]
    public EntityStat FiringLevel = new EntityStat(EntityStatType.FiringLevel);

    internal float FiringValuePerLevel => ((float) FiringValue.MaxValue / FiringLevel.MaxValue);

    public bool IsFiring => FiringLevel.Value > 0;

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧蔓延率%/s\t\"+FiringSpreadPercent")]
    public EntityProperty FiringSpreadPercent = new EntityProperty(EntityPropertyType.FiringSpreadPercent);

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧伤害抵消\t\"+FiringDamageDefense")]
    public EntityProperty FiringDamageDefense = new EntityProperty(EntityPropertyType.FiringDamageDefense);

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧触发特效\t\"+StartFiringFX")]
    public FXConfig StartFiringFX = new FXConfig();

    [BoxGroup("燃烧")]
    [LabelText("@\"燃烧持续特效\t\"+FiringFX")]
    public FXConfig FiringFX = new FXConfig();

    #endregion

    #region 操作

    [BoxGroup("操作")]
    [LabelText("@\"移动速度\t\"+MoveSpeed")]
    public EntityProperty MoveSpeed = new EntityProperty(EntityPropertyType.MoveSpeed);

    [BoxGroup("操作")]
    [LabelText("@\"最大行动力\t\"+MaxActionPoint")]
    public EntityProperty MaxActionPoint = new EntityProperty(EntityPropertyType.MaxActionPoint);

    [BoxGroup("操作")]
    [LabelText("@\"行动力\t\"+ActionPoint")]
    public EntityStat ActionPoint = new EntityStat(EntityStatType.ActionPoint);

    [BoxGroup("操作")]
    [LabelText("@\"行动力恢复速度/100\t\"+ActionPointRecovery")]
    public EntityProperty ActionPointRecovery = new EntityProperty(EntityPropertyType.ActionPointRecovery);

    #endregion

    public void Initialize(Entity entity)
    {
        Profiler.BeginSample("ESPS #1");
        Entity = entity;

        #region Property初始化

        #region 财产

        CollectDetectRadius.Initialize();
        MaxFireElementFragment.Initialize();
        MaxIceElementFragment.Initialize();
        MaxLightningElementFragment.Initialize();

        #endregion

        #region 耐久

        MaxHealthDurability.Initialize();

        #endregion

        #region 碰撞

        ActorCollideDamageDefense.Initialize();
        BoxCollideDamageSelf.Initialize();
        CollideDamage.Initialize();
        FrozenBeCollideDamage.Initialize();
        CollideDamageX.Initialize();
        CollideDamageZ.Initialize();
        BeCollidedHitStopDuration.Initialize();

        #endregion

        #region 爆炸

        ExplodeDamageDefense.Initialize();

        #endregion

        #region 冰冻

        FrozenResistance.Initialize();
        FrozenDamageDefense.Initialize();

        #endregion

        #region 燃烧

        FiringResistance.Initialize();
        FiringSpreadPercent.Initialize();
        FiringDamageDefense.Initialize();

        #endregion

        #region 操作

        MoveSpeed.Initialize();
        MaxActionPoint.Initialize();
        ActionPointRecovery.Initialize();

        #endregion

        #endregion

        Profiler.EndSample();
        Profiler.BeginSample("ESPS #2");

        Profiler.BeginSample("ESPS #2-1");

        #region 财产

        PropertyDict.Add(EntityPropertyType.CollectDetectRadius, CollectDetectRadius);
        StatDict.Add(EntityStatType.Gold, Gold);

        FireElementFragment.MaxValue = MaxFireElementFragment.GetModifiedValue;
        StatDict.Add(EntityStatType.FireElementFragment, FireElementFragment);
        PropertyDict.Add(EntityPropertyType.MaxFireElementFragment, MaxFireElementFragment);

        IceElementFragment.MaxValue = MaxIceElementFragment.GetModifiedValue;
        StatDict.Add(EntityStatType.IceElementFragment, IceElementFragment);
        PropertyDict.Add(EntityPropertyType.MaxIceElementFragment, MaxIceElementFragment);

        LightningElementFragment.MaxValue = MaxLightningElementFragment.GetModifiedValue;
        StatDict.Add(EntityStatType.LightningElementFragment, LightningElementFragment);
        PropertyDict.Add(EntityPropertyType.MaxLightningElementFragment, MaxLightningElementFragment);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-2");

        #region 耐久

        HealthDurability.MaxValue = MaxHealthDurability.GetModifiedValue;
        StatDict.Add(EntityStatType.HealthDurability, HealthDurability);
        PropertyDict.Add(EntityPropertyType.MaxHealthDurability, MaxHealthDurability);

        StatDict.Add(EntityStatType.DropFromAirSurviveProbabilityPercent, DropFromAirSurviveProbabilityPercent);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-3");

        #region 碰撞

        PropertyDict.Add(EntityPropertyType.ActorCollideDamageDefense, ActorCollideDamageDefense);
        PropertyDict.Add(EntityPropertyType.BoxCollideDamageSelf, BoxCollideDamageSelf);
        PropertyDict.Add(EntityPropertyType.CollideDamage, CollideDamage);
        PropertyDict.Add(EntityPropertyType.FrozenBeCollideDamage, FrozenBeCollideDamage);
        PropertyDict.Add(EntityPropertyType.CollideDamageX, CollideDamageX);
        PropertyDict.Add(EntityPropertyType.CollideDamageZ, CollideDamageZ);
        PropertyDict.Add(EntityPropertyType.BeCollidedHitStopDuration, BeCollidedHitStopDuration);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-4");

        #region 爆炸

        PropertyDict.Add(EntityPropertyType.ExplodeDamageDefense, ExplodeDamageDefense);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-5");

        #region 冰冻

        PropertyDict.Add(EntityPropertyType.FrozenResistance, FrozenResistance);
        PropertyDict.Add(EntityPropertyType.FrozenDamageDefense, FrozenDamageDefense);

        FrozenValue.AbnormalStatResistance = FrozenResistance.GetModifiedValue;

        StatDict.Add(EntityStatType.FrozenValue, FrozenValue);
        StatDict.Add(EntityStatType.FrozenLevel, FrozenLevel);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-6");

        #region 燃烧

        PropertyDict.Add(EntityPropertyType.FiringResistance, FiringResistance);

        FiringValue.AbnormalStatResistance = FiringResistance.GetModifiedValue;

        StatDict.Add(EntityStatType.FiringValue, FiringValue);
        StatDict.Add(EntityStatType.FiringLevel, FiringLevel);
        PropertyDict.Add(EntityPropertyType.FiringSpreadPercent, FiringSpreadPercent);
        PropertyDict.Add(EntityPropertyType.FiringDamageDefense, FiringDamageDefense);

        #endregion

        Profiler.EndSample();

        Profiler.BeginSample("ESPS #2-7");

        #region 操作

        PropertyDict.Add(EntityPropertyType.MoveSpeed, MoveSpeed);

        PropertyDict.Add(EntityPropertyType.MaxActionPoint, MaxActionPoint);

        ActionPoint.MaxValue = MaxActionPoint.GetModifiedValue;
        ActionPoint.AutoChange = ActionPointRecovery.GetModifiedValue / 100f;
        StatDict.Add(EntityStatType.ActionPoint, ActionPoint);

        PropertyDict.Add(EntityPropertyType.ActionPointRecovery, ActionPointRecovery);

        #endregion

        Profiler.EndSample();
        Profiler.EndSample();

        foreach (KeyValuePair<EntityStatType, EntityStat> kv in StatDict)
        {
            kv.Value.m_NotifyActionSet.RegisterCallBacks(StatNotifyActionSetDict[kv.Key]);
        }

        foreach (KeyValuePair<EntityPropertyType, EntityProperty> kv in PropertyDict)
        {
            kv.Value.m_NotifyActionSet.RegisterCallBacks(PropertyNotifyActionSetDict[kv.Key]);
        }

        FrozenLevel.m_NotifyActionSet.OnValueChanged += Entity.EntityFrozenHelper.OnFrozeIntoIceBlockAction;
    }

    #region Delegates

    private Dictionary<EntityStatType, Stat.NotifyActionSet> StatNotifyActionSetDict = new Dictionary<EntityStatType, Stat.NotifyActionSet>();
    private Dictionary<EntityPropertyType, Property.NotifyActionSet> PropertyNotifyActionSetDict = new Dictionary<EntityPropertyType, Property.NotifyActionSet>();

    private void InitDelegates()
    {
        foreach (EntityStatType est in Enum.GetValues(typeof(EntityStatType)))
        {
            StatNotifyActionSetDict.Add(est, new Stat.NotifyActionSet());
            StatNotifyActionSetDict[est].OnValueChanged += delegate(int before, int after)
            {
                foreach (EntityPassiveSkill eps in Entity.EntityPassiveSkills)
                {
                    eps.OnEntityStatValueChange(est, before, after);
                }
            };
        }

        StatNotifyActionSetDict[EntityStatType.ActionPoint].OnValueNotEnoughWarning += OnActionPointNotEnoughWarning;
        StatNotifyActionSetDict[EntityStatType.Gold].OnValueIncrease += OnGoldIncrease;
        StatNotifyActionSetDict[EntityStatType.FireElementFragment].OnValueIncrease += OnFireElementFragmentIncrease;
        StatNotifyActionSetDict[EntityStatType.FireElementFragment].OnValueNotEnoughWarning += OnFireElementFragmentNotEnoughWarning;
        StatNotifyActionSetDict[EntityStatType.IceElementFragment].OnValueIncrease += OnIceElementFragmentIncrease;
        StatNotifyActionSetDict[EntityStatType.IceElementFragment].OnValueNotEnoughWarning += OnIceElementFragmentNotEnoughWarning;
        StatNotifyActionSetDict[EntityStatType.LightningElementFragment].OnValueIncrease += OnLightningElementFragmentIncrease;
        StatNotifyActionSetDict[EntityStatType.LightningElementFragment].OnValueNotEnoughWarning += OnLightningElementFragmentNotEnoughWarning;
        StatNotifyActionSetDict[EntityStatType.HealthDurability].OnValueIncrease += OnHealthDurabilityIncrease;
        StatNotifyActionSetDict[EntityStatType.HealthDurability].OnValueDecrease += OnHealthDurabilityDecrease;
        StatNotifyActionSetDict[EntityStatType.HealthDurability].OnValueReachZero += OnHealthDurabilityReachZero;
        StatNotifyActionSetDict[EntityStatType.FrozenValue].OnValueIncrease += OnFrozenValueIncrease;
        StatNotifyActionSetDict[EntityStatType.FrozenValue].OnValueChanged += OnFrozenValueChanged;
        StatNotifyActionSetDict[EntityStatType.FiringValue].OnValueChanged += OnFiringValueChanged;
        StatNotifyActionSetDict[EntityStatType.FiringValue].OnValueIncrease += OnFiringValueIncrease;
        StatNotifyActionSetDict[EntityStatType.FiringLevel].OnValueChanged += OnFiringLevelChanged;

        foreach (EntityPropertyType ept in Enum.GetValues(typeof(EntityPropertyType)))
        {
            PropertyNotifyActionSetDict.Add(ept, new Property.NotifyActionSet());
            PropertyNotifyActionSetDict[ept].OnValueChanged += delegate(int before, int after)
            {
                foreach (EntityPassiveSkill eps in Entity.EntityPassiveSkills)
                {
                    eps.OnEntityPropertyValueChange(ept, before, after);
                }
            };
        }

        PropertyNotifyActionSetDict[EntityPropertyType.CollectDetectRadius].OnValueChanged += OnCollectDetectRadiusChanged;
        PropertyNotifyActionSetDict[EntityPropertyType.MaxHealthDurability].OnValueChanged += OnMaxHealthDurabilityChanged;
        PropertyNotifyActionSetDict[EntityPropertyType.FrozenResistance].OnValueChanged += OnFrozenResistanceChanged;
        PropertyNotifyActionSetDict[EntityPropertyType.FiringResistance].OnValueChanged += OnFiringResistanceChanged;
        PropertyNotifyActionSetDict[EntityPropertyType.ActionPointRecovery].OnValueChanged += OnActionPointRecoveryChanged;
        PropertyNotifyActionSetDict[EntityPropertyType.MaxActionPoint].OnValueChanged += OnMaxActionPointChanged;
    }

    private void OnActionPointNotEnoughWarning()
    {
        if (Entity == BattleManager.Instance.Player1)
        {
            ClientGameManager.Instance.PlayerStatHUDPanel.PlayerStatHUDs_Player[0].ActionPointBar.OnStatLowWarning();
        }
    }

    private void OnGoldIncrease(int increase)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowGainGoldNumFX(increase);
        }
    }

    private void OnFireElementFragmentIncrease(int increase)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowGainFireElementFragmentNumFX(increase);
        }
    }

    private void OnFireElementFragmentNotEnoughWarning()
    {
        if (Entity == BattleManager.Instance.Player1)
        {
            ClientGameManager.Instance.PlayerStatHUDPanel.PlayerStatHUDs_Player[0].FireElementFragmentBar.OnStatLowWarning();
        }
    }

    private void OnIceElementFragmentIncrease(int increase)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowGainIceElementFragmentNumFX(increase);
        }
    }

    private void OnIceElementFragmentNotEnoughWarning()
    {
        if (Entity == BattleManager.Instance.Player1)
        {
            ClientGameManager.Instance.PlayerStatHUDPanel.PlayerStatHUDs_Player[0].IceElementFragmentBar.OnStatLowWarning();
        }
    }

    private void OnLightningElementFragmentIncrease(int increase)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowGainLightningElementFragmentNumFX(increase);
        }
    }

    private void OnLightningElementFragmentNotEnoughWarning()
    {
        if (Entity == BattleManager.Instance.Player1)
        {
            ClientGameManager.Instance.PlayerStatHUDPanel.PlayerStatHUDs_Player[0].LightningElementFragmentBar.OnStatLowWarning();
        }
    }

    private void OnHealthDurabilityIncrease(int increase)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowHealNumFX(increase);
            actor.EntityWwiseHelper.OnBeingHealed.Post(actor.gameObject);
        }
    }

    private void OnHealthDurabilityDecrease(int decrease)
    {
        if (Entity is Actor actor)
        {
            actor.ActorBattleHelper.ShowDamageNumFX(decrease);
            actor.EntityWwiseHelper.OnBeingDamaged.Post(actor.gameObject);
        }
    }

    private void OnHealthDurabilityReachZero(string changeInfo)
    {
        Entity.PassiveSkillMarkAsDestroyed = true;
        foreach (EntityBuffAttribute attribute in Enum.GetValues(typeof(EntityBuffAttribute)))
        {
            if (changeInfo.Contains($"Damage-{attribute}"))
            {
                foreach (EntityPassiveSkill eps in Entity.EntityPassiveSkills)
                {
                    eps.OnDestroyEntityByElementDamage(attribute);
                }

                switch (attribute)
                {
                    case EntityBuffAttribute.FiringDamage:
                    {
                        Entity.EntityWwiseHelper.OnDestroyed_ByFiringDamage.Post(Entity.gameObject);
                        break;
                    }
                    case EntityBuffAttribute.CollideDamage:
                    {
                        Entity.EntityWwiseHelper.OnDestroyed_ByCollideDamage.Post(Entity.gameObject);
                        break;
                    }
                    case EntityBuffAttribute.ExplodeDamage:
                    {
                        Entity.EntityWwiseHelper.OnDestroyed_ByExplodeDamage.Post(Entity.gameObject);
                        break;
                    }
                    case EntityBuffAttribute.FrozenDamage:
                    {
                        Entity.EntityWwiseHelper.OnDestroyed_ByFrozenDamage.Post(Entity.gameObject);
                        break;
                    }
                }
            }
        }

        if (Entity is Actor actor) actor.DestroySelf();
    }

    private void OnCollectDetectRadiusChanged(int before, int after)
    {
        if (Entity.EntityCollectHelper != null)
        {
            Entity.EntityCollectHelper.EnableDetector = after > 0;
            Entity.EntityCollectHelper.DetectRadius = after;
        }
    }

    private void OnMaxHealthDurabilityChanged(int before, int after)
    {
        HealthDurability.MaxValue = after;
    }

    private void OnFrozenResistanceChanged(int before, int after)
    {
        FrozenValue.AbnormalStatResistance = after;
    }

    private void OnFrozenValueIncrease(int increase)
    {
        FiringValue.SetValue(0);
    }

    private void OnFrozenValueChanged(int before, int after)
    {
        FrozenLevel.SetValue(after / FrozenValuePerLevel, "FrozenValueChange");
        if (FrozenLevel.Value > 0) Entity.EntityBuffHelper.PlayAbnormalStatFX(EntityStatType.FrozenValue, FrozenFX, FrozenLevel.Value); // 冰冻值变化时，播放一次特效
    }

    private void OnFiringResistanceChanged(int before, int after)
    {
        FiringValue.AbnormalStatResistance = after;
    }

    private void OnFiringValueChanged(int before, int after)
    {
        FiringLevel.SetValue(Mathf.RoundToInt(after / FiringValuePerLevel), "FiringValueChange");
        if (FiringLevel.Value > 0)
            Entity.EntityBuffHelper.PlayAbnormalStatFX(EntityStatType.FiringValue, FiringFX, FiringLevel.Value); // 燃烧值变化时，播放一次特效
        else if (after == 0) Entity.EntityBuffHelper.RemoveAbnormalStatFX(EntityStatType.FiringValue);
    }

    private void OnFiringValueIncrease(int increase)
    {
        FrozenValue.SetValue(FrozenValue.Value - increase);
    }

    private void OnFiringLevelChanged(int before, int after)
    {
        if (before == 0 && after > 0)
        {
            if (Entity is Box box)
            {
                foreach (GridPos3D offset in box.GetEntityOccupationGPs_Rotated())
                {
                    FX fx = FXManager.Instance.PlayFX(StartFiringFX, box.transform.position + Vector3.up * 0.5f + offset);
                    if (fx) fx.transform.parent = box.transform;
                }
            }

            Entity.EntityWwiseHelper.OnBeingLit.Post(Entity.gameObject);
        }

        if (before > 0 && after == 0) Entity.EntityWwiseHelper.OnBurningEnd.Post(Entity.gameObject);
    }

    private void OnActionPointRecoveryChanged(int before, int after)
    {
        ActionPoint.AutoChange = after / 100f;
    }

    private void OnMaxActionPointChanged(int before, int after)
    {
        ActionPoint.MaxValue = after;
    }

    #endregion

    public void OnRecycled()
    {
        foreach (KeyValuePair<EntityStatType, EntityStat> kv in StatDict)
        {
            kv.Value.OnRecycled();
        }

        StatDict.Clear();
        foreach (KeyValuePair<EntityPropertyType, EntityProperty> kv in PropertyDict)
        {
            kv.Value.OnRecycled();
        }

        PropertyDict.Clear();
    }

    private float abnormalStateAutoTick = 0f;
    private int abnormalStateAutoTickInterval = 1; // 异常状态值每秒降低

    public void Tick(float deltaTime)
    {
        foreach (KeyValuePair<EntityStatType, EntityStat> kv in StatDict)
        {
            kv.Value.FixedUpdate(deltaTime);
        }

        abnormalStateAutoTick += deltaTime;
        if (abnormalStateAutoTick > abnormalStateAutoTickInterval)
        {
            abnormalStateAutoTick -= abnormalStateAutoTickInterval;
            if (FiringLevel.Value >= 1)
            {
                // 燃烧蔓延
                foreach (Box adjacentBox in WorldManager.Instance.CurrentWorld.GetAdjacentBox(Entity.WorldGP))
                {
                    int diff = FiringValue.Value - adjacentBox.EntityStatPropSet.FiringValue.Value;
                    if (diff > 0)
                    {
                        adjacentBox.EntityStatPropSet.FiringValue.SetValue(adjacentBox.EntityStatPropSet.FiringValue.Value + Mathf.RoundToInt(diff * FiringSpreadPercent.GetModifiedValue / 100f), "FiringSpread");
                    }
                }

                Entity.EntityBuffHelper.Damage(FiringLevel.Value, EntityBuffAttribute.FiringDamage, 0);
            }
        }
    }

    public void ApplyDataTo(EntityStatPropSet target)
    {
        #region 财产

        CollectDetectRadius.ApplyDataTo(target.CollectDetectRadius);
        Gold.ApplyDataTo(target.Gold);
        FireElementFragment.ApplyDataTo(target.FireElementFragment);
        MaxFireElementFragment.ApplyDataTo(target.MaxFireElementFragment);
        IceElementFragment.ApplyDataTo(target.IceElementFragment);
        MaxIceElementFragment.ApplyDataTo(target.MaxIceElementFragment);
        LightningElementFragment.ApplyDataTo(target.LightningElementFragment);
        MaxLightningElementFragment.ApplyDataTo(target.MaxLightningElementFragment);

        #endregion

        #region 耐久

        HealthDurability.ApplyDataTo(target.HealthDurability);
        MaxHealthDurability.ApplyDataTo(target.MaxHealthDurability);
        DropFromAirSurviveProbabilityPercent.ApplyDataTo(target.DropFromAirSurviveProbabilityPercent);

        #endregion

        #region 碰撞

        ActorCollideDamageDefense.ApplyDataTo(target.ActorCollideDamageDefense);
        BoxCollideDamageSelf.ApplyDataTo(target.BoxCollideDamageSelf);
        CollideDamage.ApplyDataTo(target.CollideDamage);
        FrozenBeCollideDamage.ApplyDataTo(target.FrozenBeCollideDamage);
        CollideDamageX.ApplyDataTo(target.CollideDamageX);
        CollideDamageZ.ApplyDataTo(target.CollideDamageZ);
        BeCollidedHitStopDuration.ApplyDataTo(target.BeCollidedHitStopDuration);

        #endregion

        #region 爆炸

        ExplodeDamageDefense.ApplyDataTo(target.ExplodeDamageDefense);

        #endregion

        #region 冰冻

        FrozenResistance.ApplyDataTo(target.FrozenResistance);
        FrozenValue.ApplyDataTo(target.FrozenValue);
        FrozenLevel.ApplyDataTo(target.FrozenLevel);
        FrozenDamageDefense.ApplyDataTo(target.FrozenDamageDefense);
        target.FrozenFX.CopyDataFrom(FrozenFX);

        #endregion

        #region 燃烧

        FiringResistance.ApplyDataTo(target.FiringResistance);
        FiringValue.ApplyDataTo(target.FiringValue);
        FiringLevel.ApplyDataTo(target.FiringLevel);
        FiringSpreadPercent.ApplyDataTo(target.FiringSpreadPercent);
        FiringDamageDefense.ApplyDataTo(target.FiringDamageDefense);
        target.StartFiringFX.CopyDataFrom(StartFiringFX);
        target.FiringFX.CopyDataFrom(FiringFX);

        #endregion

        #region 操作

        MoveSpeed.ApplyDataTo(target.MoveSpeed);
        MaxActionPoint.ApplyDataTo(target.MaxActionPoint);
        ActionPoint.ApplyDataTo(target.ActionPoint);
        ActionPointRecovery.ApplyDataTo(target.ActionPointRecovery);

        #endregion
    }
}