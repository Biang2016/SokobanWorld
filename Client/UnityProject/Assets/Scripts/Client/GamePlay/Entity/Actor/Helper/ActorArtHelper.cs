﻿using Sirenix.OdinInspector;
using UnityEngine;

public class ActorArtHelper : EntityArtHelper
{
    [SerializeField]
    private Animator ActorArtRootAnim;

    [SerializeField]
    private Animator ActorModelAnim;

    internal TargetEntityType TempTargetEntityType;

    public override void OnHelperRecycled()
    {
        base.OnHelperRecycled();
        CanTurn = true;
        CanPlayOtherAnimSkill = true;
        CanPan = true;
        if (ActorModelAnim != null)
        {
            foreach (AnimatorControllerParameter parameter in ActorModelAnim.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                    ActorModelAnim.SetBool(parameter.name, false);
                if (parameter.type == AnimatorControllerParameterType.Float)
                    ActorModelAnim.SetFloat(parameter.name, 0);
                if (parameter.type == AnimatorControllerParameterType.Int)
                    ActorModelAnim.SetInteger(parameter.name, 0);
                if (parameter.type == AnimatorControllerParameterType.Trigger)
                    ActorModelAnim.ResetTrigger(parameter.name);
            }
        }
    }

    public override void OnHelperUsed()
    {
        base.OnHelperUsed();
        CanTurn = true;
        CanPlayOtherAnimSkill = true;
        CanPan = true;
    }

    public void ResetModelAnimTrigger(string trigger)
    {
        if (ActorModelAnim != null) ActorModelAnim.ResetTrigger(trigger);
    }

    public void SetModelAnimTrigger(string trigger)
    {
        if (ActorModelAnim != null) ActorModelAnim.SetTrigger(trigger);
    }

    public void SetModelAnimBool(string boolean, bool value)
    {
        if (ActorModelAnim != null) ActorModelAnim.SetBool(boolean, value);
    }

    public void Vault()
    {
        if (ActorArtRootAnim != null)
        {
            ActorArtRootAnim.SetTrigger("Vault");
            ((Actor) Entity).SetModelSmoothMoveLerpTime(0f);
        }
    }

    /// <summary>
    /// Executed by animation
    /// </summary>
    public void SwapBox()
    {
        ((Actor) Entity).SwapBox();
    }

    /// <summary>
    /// Executed by animation
    /// </summary>
    public void VaultEnd()
    {
        ((Actor) Entity).SetModelSmoothMoveLerpTime(((Actor) Entity).DefaultSmoothMoveLerpTime);
    }

    public void Kick()
    {
        if (ActorArtRootAnim != null)
        {
            ActorArtRootAnim.SetTrigger("Kick");
        }
    }

    public void Dash()
    {
        if (ActorArtRootAnim != null)
        {
            ActorArtRootAnim.SetTrigger("Dash");
        }
    }

    /// <summary>
    /// Executed by animation
    /// </summary>
    public void DoDash()
    {
        ((Actor) Entity).DoDash();
    }

    void FixedUpdate()
    {
        if (Entity.IsNotNullAndAlive())
        {
            IsAnimFreeze = ((Actor) Entity).CannotAct;
        }
    }

    #region 第一优先级

    private bool isAnimFreeze = false;

    private bool IsAnimFreeze
    {
        get { return isAnimFreeze; }
        set
        {
            if (isAnimFreeze != value)
            {
                if (value)
                {
                    if (ActorModelAnim != null)
                    {
                        ActorModelAnim.speed = 0;
                    }
                }
                else
                {
                    if (ActorModelAnim != null)
                    {
                        ActorModelAnim.speed = 1;
                    }
                }

                isAnimFreeze = value;
            }
        }
    }

    #endregion

    #region 第二优先级

    public void PlaySkill(EntitySkillIndex skillIndex, TargetEntityType targetEntityType)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetTrigger(skillIndex.ToString());
        }

        TempTargetEntityType = targetEntityType;
    }

    /// <summary>
    /// Executed by animation
    /// </summary>
    public void TriggerSkill(EntitySkillIndex skillIndex)
    {
        if (((Actor) Entity).EntityActiveSkillDict.TryGetValue(skillIndex, out EntityActiveSkill eas))
        {
            eas.TriggerActiveSkill(TempTargetEntityType);
        }
    }

    public void SetIsPushing(bool isPushing)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetBool("IsPushing", isPushing);
        }
    }

    #endregion

    #region 第三优先级

    [ShowInInspector]
    internal bool CanTurn = true;

    [ShowInInspector]
    internal bool CanPlayOtherAnimSkill = true;

    [ShowInInspector]
    internal bool CanPan = true;

    public void SetIsChasing(bool isChasing)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetBool("IsChasing", isChasing);
        }
    }

    public void SetIsWalking(bool isWalking)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetBool("IsWalking", isWalking);
        }
    }

    public void SetPFMoveGridSpeed(int PFMoveGridSpeed)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetInteger("PFMoveGridSpeed", PFMoveGridSpeed);
        }
    }

    public void SetIsEscaping(bool isEscaping)
    {
        if (ActorModelAnim != null)
        {
            ActorModelAnim.SetBool("IsEscaping", isEscaping);
        }
    }

    #endregion
}