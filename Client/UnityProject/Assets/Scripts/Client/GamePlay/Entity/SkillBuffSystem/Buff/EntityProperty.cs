﻿using System;
using Sirenix.OdinInspector;

[Serializable]
public class EntityProperty : Property
{
    public EntityProperty()
    {
    }

    public EntityProperty(EntityPropertyType propertyType)
    {
        m_PropertyType = (int) propertyType;
    }

    public EntityProperty(EntitySkillPropertyType propertyType)
    {
        m_PropertyType = (int) propertyType;
    }

    internal int m_PropertyType;

    protected override void ChildApplyDataTo(Property target)
    {
        base.ChildApplyDataTo(target);
        EntityProperty newProp = (EntityProperty) target;
        newProp.m_PropertyType = m_PropertyType;
    }
}

public class EntityPropertyAttribute : Attribute
{
}

public class BoxPropertyAttribute : Attribute
{
}

public class ActorPropertyAttribute : Attribute
{
}

public enum EntityPropertyType
{
    [EntityProperty]
    [LabelText("血量耐久上限")]
    MaxHealthDurability = 0,

    [EntityProperty]
    [LabelText("血量耐久回复速度")]
    HealthDurabilityRecovery = 1,

    [EntityProperty]
    [LabelText("血量耐久回复时间间隔")]
    HealthDurabilityRecoveryTimeInterval = 2,

    [EntityProperty]
    [LabelText("燃烧伤害抵消")]
    FiringDamageDefense = 10,

    [EntityProperty]
    [LabelText("爆炸伤害抵消")]
    ExplodeDamageDefense = 11,

    [ActorProperty]
    [LabelText("角色碰撞伤害抵消")]
    ActorCollideDamageDefense = 12,

    [BoxProperty]
    [LabelText("箱子碰撞减少自身耐久")]
    BoxCollideDamageSelf = 13,

    [EntityProperty]
    [LabelText("碰撞伤害")]
    CollideDamage = 14,

    [ActorProperty]
    [LabelText("移动速度")]
    MoveSpeed = 30,

    [ActorProperty]
    [LabelText("最大行动力")]
    MaxActionPoint = 31,

    [ActorProperty]
    [LabelText("行动力回复速度")]
    ActionPointRecovery = 32,

    [ActorProperty]
    [LabelText("Kick消耗行动力")]
    KickConsumeActionPoint = 33,

    [ActorProperty]
    [LabelText("Dash消耗行动力")]
    DashConsumeActionPoint = 34,

    [ActorProperty]
    [LabelText("Vault消耗行动力")]
    VaultConsumeActionPoint = 35,

    [EntityProperty]
    [LabelText("冰冻抗性")]
    FrozenResistance = 100,

    [EntityProperty]
    [LabelText("燃烧抗性")]
    FiringResistance = 101,

    [EntityProperty]
    [LabelText("冰冻恢复率")]
    FrozenRecovery = 200,

    [EntityProperty]
    [LabelText("燃烧恢复率")]
    FiringRecovery = 201,

    [EntityProperty]
    [LabelText("燃烧增长率")]
    FiringGrowthPercent = 301,

    [EntityProperty]
    [LabelText("燃烧蔓延率")]
    FiringSpreadPercent = 401,
}

public enum EntitySkillPropertyType
{
    [LabelText("伤害")]
    Damage = 10001,

    [LabelText("附加燃烧值")]
    Attach_FiringValue = 10011,

    [LabelText("附加冰冻值")]
    Attach_FrozenValue = 10021,

    [LabelText("施法正方形范围边长")]
    CastingRadius = 10110,

    [LabelText("效果半径")]
    EffectRadius = 10111,

    [LabelText("宽度X")]
    Width = 10112,

    [LabelText("深度Z")]
    Depth = 10113,

    [LabelText("冷却时间")]
    Cooldown = 10120,

    [LabelText("前摇")]
    WingUp = 10121,

    [LabelText("施法时间")]
    CastDuration = 10122,

    [LabelText("后摇")]
    Recovery = 10123,
}