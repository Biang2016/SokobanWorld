﻿using System;
using BiangLibrary.CloneVariant;
using BiangLibrary.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public abstract class EntitySkillAction : IClone<EntitySkillAction>
{
    [ReadOnly]
    [HideInEditorMode]
    public uint InitWorldModuleGUID; // 创建时所属的世界模组GUID

    public abstract void OnRecycled();

    public virtual void Init(uint initWorldModuleGUID)
    {
        InitWorldModuleGUID = initWorldModuleGUID;
    }

    public virtual void UnInit()
    {
    }

    protected virtual string Description => "Entity被动技能行为基类";

    public interface IPureAction
    {
        void Execute();
    }

    public interface IWorldGPAction
    {
        void ExecuteOnWorldGP(GridPos3D worldGP);
    }

    public interface IEntityAction
    {
        void ExecuteOnEntity(Entity entity);
    }

    public interface ICollideAction
    {
        void ExecuteOnCollide(Collision collision);
    }

    public interface ITriggerAction
    {
        void ExecuteOnTriggerEnter(Collider collider);
        void ExecuteOnTriggerStay(Collider collider);
        void ExecuteOnTriggerExit(Collider collider);
    }

    internal Entity Entity;

    public EntitySkillAction Clone()
    {
        Type type = GetType();
        EntitySkillAction newAction = (EntitySkillAction) Activator.CreateInstance(type);
        ChildClone(newAction);
        return newAction;
    }

    protected virtual void ChildClone(EntitySkillAction newAction)
    {
    }

    public virtual void CopyDataFrom(EntitySkillAction srcData)
    {
    }
}