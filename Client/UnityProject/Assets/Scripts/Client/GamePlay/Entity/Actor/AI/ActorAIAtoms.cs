﻿using System.Collections;
using System.Collections.Generic;
using BiangStudio;
using BiangStudio.GameDataFormat.Grid;
using FlowCanvas;
using FlowCanvas.Nodes;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using Random = UnityEngine.Random;
using WaitUntil = FlowCanvas.Nodes.WaitUntil;

public static class ActorAIAtoms
{
    #region 寻路

    [Category("敌兵/寻路")]
    [Name("设定目的地为玩家")]
    [Description("设定目的地为玩家")]
    public class BT_Enemy_SetDestinationToMainPlayer : BTNode
    {
        [Name("保持最小距离")]
        public BBParameter<float> KeepDistanceMin;

        [Name("保持最大距离")]
        public BBParameter<float> KeepDistanceMax;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ActorAIAgent.IsPathFinding) return Status.Failure;
            Actor player = BattleManager.Instance.Player1;
            ActorAIAgent.SetDestinationRetCode retCode = Actor.ActorAIAgent.SetDestination(player.CurWorldGP, KeepDistanceMin.value, KeepDistanceMax.value, false, ActorPathFinding.DestinationType.Actor);
            switch (retCode)
            {
                case ActorAIAgent.SetDestinationRetCode.AlreadyArrived:
                {
                    return Status.Success;
                }
                case ActorAIAgent.SetDestinationRetCode.TooClose:
                {
                    // 逃脱寻路
                    List<GridPos3D> runDestList = new List<GridPos3D>();

                    for (int angle = 0; angle <= 360; angle += 10)
                    {
                        float radianAngle = Mathf.Deg2Rad * angle;
                        Vector3 dest = player.CurWorldGP.ToVector3() + new Vector3((KeepDistanceMin.value + 1) * Mathf.Sin(radianAngle), 0, (KeepDistanceMin.value + 1) * Mathf.Cos(radianAngle));
                        GridPos3D destGP = dest.ToGridPos3D();
                        runDestList.Add(destGP);
                    }

                    runDestList.Sort((gp1, gp2) =>
                    {
                        float dist1 = (gp1.ToVector3() + player.transform.position - Actor.transform.position).magnitude;
                        float dist2 = (gp2.ToVector3() + player.transform.position - Actor.transform.position).magnitude;
                        return dist1.CompareTo(dist2);
                    });

                    foreach (GridPos3D gp in runDestList)
                    {
                        ActorAIAgent.SetDestinationRetCode rc = Actor.ActorAIAgent.SetDestination(gp, 0, 1, false, ActorPathFinding.DestinationType.EmptyGrid);
                        if (rc == ActorAIAgent.SetDestinationRetCode.Suc)
                        {
                            return Status.Running;
                        }
                    }

                    return Status.Failure;
                }
                case ActorAIAgent.SetDestinationRetCode.Suc:
                {
                    return Status.Success;
                }
                case ActorAIAgent.SetDestinationRetCode.Failed:
                {
                    return Status.Failure;
                }
            }

            return Status.Failure;
        }
    }

    [Category("敌兵/寻路")]
    [Name("是否能够移动到玩家")]
    [Description("是否能够移动到玩家")]
    public class BT_Enemy_CheckCanMoveToMainPlayer : ConditionTask
    {
        [Name("保持最小距离")]
        public BBParameter<float> KeepDistanceMin;

        [Name("保持最大距离")]
        public BBParameter<float> KeepDistanceMax;

        [Name("警戒距离")]
        public BBParameter<float> GuardingRange;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            Actor player = BattleManager.Instance.Player1;
            if ((player.transform.position - Actor.transform.position).magnitude > GuardingRange.value) return false;
            bool suc = ActorPathFinding.FindPath(Actor.CurWorldGP, player.CurWorldGP, null, KeepDistanceMin.value, KeepDistanceMax.value, ActorPathFinding.DestinationType.Actor);
            return suc;
        }
    }

    [Category("敌兵/寻路")]
    [Name("寻路终点是否是玩家坐标")]
    [Description("寻路终点是否是玩家坐标")]
    public class BT_Enemy_CheckDestIsMainPlayer : ConditionTask
    {
        [Name("容许偏差半径")]
        public BBParameter<float> ToleranceRadius;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null || !Actor.ActorAIAgent.IsPathFinding) return false;
            Actor player = BattleManager.Instance.Player1;
            return ((player.transform.position - Actor.ActorAIAgent.GetCurrentPathFindingDestination().ToVector3()).magnitude <= ToleranceRadius.value);
        }
    }

    [Category("敌兵/寻路")]
    [Name("设置闲逛目标点")]
    [Description("设置闲逛目标点")]
    public class BT_Enemy_Idle : BTNode
    {
        [Name("闲逛半径")]
        public BBParameter<int> IdleRadius;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ThrowState == Actor.ThrowStates.Lifting && Actor.CurrentLiftBox != null)
            {
                Actor.ThrowCharge();
                Actor.CurThrowPointOffset = Actor.transform.forward * 3f;
                Actor.Throw();
            }

            if (Actor.ActorAIAgent.IsPathFinding) return Status.Failure;
            bool suc = ActorPathFinding.FindRandomAccessibleDestination(Actor.CurWorldGP, IdleRadius.value, out GridPos3D destination);
            if (suc)
            {
                ActorAIAgent.SetDestinationRetCode retCode = Actor.ActorAIAgent.SetDestination(destination, 0f, 0.5f, false, ActorPathFinding.DestinationType.EmptyGrid);
                switch (retCode)
                {
                    case ActorAIAgent.SetDestinationRetCode.AlreadyArrived:
                    case ActorAIAgent.SetDestinationRetCode.TooClose:
                    {
                        return Status.Success;
                    }
                    case ActorAIAgent.SetDestinationRetCode.Suc:
                    {
                        return Status.Success;
                    }
                    case ActorAIAgent.SetDestinationRetCode.Failed:
                    {
                        return Status.Failure;
                    }
                }

                return Status.Success;
            }
            else
            {
                return Status.Failure;
            }
        }
    }

    [Category("敌兵/寻路")]
    [Name("结束寻路")]
    [Description("结束寻路")]
    public class BT_Enemy_TerminatePathFinding : BTNode
    {
        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ActorAIAgent.IsPathFinding)
            {
                Actor.ActorAIAgent.InterruptCurrentPathFinding();
            }

            return Status.Success;
        }
    }

    [Category("敌兵/寻路")]
    [Name("强制结束寻路")]
    [Description("强制结束寻路")]
    public class BT_Enemy_ForceTerminatePathFinding : BTNode
    {
        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ActorAIAgent.IsPathFinding)
            {
                Actor.ActorAIAgent.ClearPathFinding();
            }

            return Status.Success;
        }
    }

    [Category("敌兵/寻路")]
    [Name("结束寻路")]
    [Description("结束寻路")]
    public class FL_Enemy_TerminatePathFinding : CallableActionNode
    {
        public override void Invoke()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return;
            if (Actor.ActorAIAgent.IsPathFinding)
            {
                Actor.ActorAIAgent.InterruptCurrentPathFinding();
            }
        }
    }

    [Category("敌兵/寻路")]
    [Name("强制结束寻路")]
    [Description("强制结束寻路")]
    public class FL_Enemy_ForceTerminatePathFinding : CallableActionNode
    {
        public override void Invoke()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return;
            if (Actor.ActorAIAgent.IsPathFinding)
            {
                Actor.ActorAIAgent.ClearPathFinding();
            }
        }
    }

    [Category("敌兵/寻路")]
    [Name("有寻路任务但卡在一个地方")]
    [Description("有寻路任务但卡在一个地方")]
    public class BT_Enemy_StuckWithNavTask : ConditionTask
    {
        [Name("卡住时长/ms")]
        public BBParameter<int> StuckTime;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            if (!Actor.ActorAIAgent.IsPathFinding) return false;
            return Actor.ActorAIAgent.StuckWithNavTask_Tick > (StuckTime.value / 1000f);
        }
    }

    #endregion

    [Category("敌兵")]
    [Name("生命值大等于")]
    [Description("生命值大等于")]
    public class BT_Enemy_LifeCondition : ConditionTask
    {
        [Name("阈值")]
        public BBParameter<int> LifeThreshold;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            if (Actor.ActorBattleHelper == null) return false;
            return Actor.ActorStatPropSet.Health.Value >= LifeThreshold.value;
        }
    }

    [Category("敌兵")]
    [Name("生命值大于")]
    [Description("生命值大于")]
    public class BT_Enemy_LifeConditionLargerThan : ConditionTask
    {
        [Name("阈值")]
        public BBParameter<int> LifeThreshold;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            if (Actor.ActorBattleHelper == null) return false;
            return Actor.ActorStatPropSet.Health.Value > LifeThreshold.value;
        }
    }

    [Category("敌兵")]
    [Name("生命值大于%")]
    [Description("生命值大于%")]
    public class BT_Enemy_LifeConditionLargerThanPercent : ConditionTask
    {
        [Name("阈值%")]
        public BBParameter<int> LifePercentThreshold;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            if (Actor.ActorBattleHelper == null) return false;
            return Actor.ActorStatPropSet.Health.Value > LifePercentThreshold.value / 100f * Actor.ActorStatPropSet.MaxHealth.GetModifiedValue;
        }
    }

    [Category("敌兵")]
    [Name("玩家在距离内")]
    [Description("玩家在距离内")]
    public class BT_Enemy_PlayerInGuardRangeCondition : ConditionTask
    {
        [Name("阈值")]
        public BBParameter<float> RangeRadius;

        protected override bool OnCheck()
        {
            if (Actor == null || Actor.ActorAIAgent == null) return false;
            return (BattleManager.Instance.Player1.transform.position - Actor.transform.position).magnitude <= RangeRadius.value;
        }
    }

    [Category("敌兵")]
    [Name("玩家在距离内")]
    [Description("玩家在距离内")]
    public class BT_Enemy_PlayerInGuardRangeConditionBTNode : BTNode
    {
        [Name("阈值")]
        public BBParameter<float> RangeRadius;

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            bool inside = (BattleManager.Instance.Player1.transform.position - Actor.transform.position).magnitude <= RangeRadius.value;
            return inside ? Status.Success : Status.Failure;
        }
    }

    #region 战斗

    [Category("敌兵/战斗")]
    [Name("面向玩家")]
    [Description("面向玩家")]
    public class BT_Enemy_FaceToPlayer : BTNode
    {
        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.IsFrozen) return Status.Failure;
            if (Actor.ActorAIAgent.IsPathFinding) return Status.Failure;
            GridPos3D targetGP = BattleManager.Instance.Player1.CurWorldGP;
            GridPos3D curGP = Actor.CurWorldGP;
            GridPos3D diff = targetGP - curGP;
            Actor.CurForward = diff.Normalized().ToVector3();
            return Status.Success;
        }
    }

    [Category("敌兵/战斗")]
    [Name("释放技能")]
    [Description("释放技能")]
    public class BT_Enemy_TriggerSkill : BTNode
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        public override string name => $"释放技能 [{ActorSkillIndex.value}]";

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            return TriggerSkill(Actor, ActorSkillIndex.value);
        }

        public static Status TriggerSkill(Actor actor, ActorSkillIndex skillIndex)
        {
            if (actor == null || actor.ActorAIAgent == null) return Status.Failure;
            if (actor.IsFrozen) return Status.Failure;
            if (actor.ActorActiveSkillDict.TryGetValue(skillIndex, out ActorActiveSkill aas))
            {
                actor.ActorAIAgent.TargetActor = BattleManager.Instance.Player1;
                actor.ActorAIAgent.TargetActorGP = BattleManager.Instance.Player1.CurWorldGP;
                bool triggerSuc = aas.TriggerActiveSkill();
                if (triggerSuc)
                {
                    return Status.Success;
                }
                else
                {
                    return Status.Failure;
                }
            }

            return Status.Failure;
        }
    }

    [Category("敌兵/战斗")]
    [Name("释放技能+确认")]
    [Description("释放技能+确认")]
    public class BT_Enemy_TriggerSkill_ConditionTask : ConditionTask
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        protected override string info => $"释放技能且成功 [{ActorSkillIndex.value}]";

        protected override bool OnCheck()
        {
            Status status = BT_Enemy_TriggerSkill.TriggerSkill(Actor, ActorSkillIndex.value);
            return status == Status.Success;
        }
    }

    [Category("敌兵/战斗")]
    [Name("释放技能")]
    [Description("释放技能")]
    public class FL_Enemy_TriggerSkillAction : CallableActionNode
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        public override string name => $"释放技能 [{ActorSkillIndex.value}]";

        public override void Invoke()
        {
            BT_Enemy_TriggerSkill.TriggerSkill(Actor, ActorSkillIndex.value);
        }
    }

    [Category("敌兵/战斗")]
    [Name("等待技能结束")]
    [Description("等待技能结束")]
    public class BT_Enemy_WaitSkillEnd : BTNode
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        public override string name => $"等待技能结束 [{ActorSkillIndex.value}]";

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ActorActiveSkillDict.TryGetValue(ActorSkillIndex.value, out ActorActiveSkill aas))
            {
                if (aas.SkillPhase == ActiveSkillPhase.CoolingDown || aas.SkillPhase == ActiveSkillPhase.Ready)
                {
                    return Status.Success;
                }
                else
                {
                    return Status.Running;
                }
            }

            return Status.Failure;
        }
    }

    [Category("敌兵/战斗")]
    [Name("等待技能结束")]
    [Description("等待技能结束")]
    public class FL_Enemy_WaitSkillEnd : WaitUntil
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        public override string name => $"等待技能结束 [{ActorSkillIndex.value}]";

        public override IEnumerator Invoke()
        {
            if (Actor == null || Actor.ActorAIAgent == null) yield return null;
            if (Actor.ActorActiveSkillDict.TryGetValue(ActorSkillIndex.value, out ActorActiveSkill aas))
            {
                while (aas.SkillPhase != ActiveSkillPhase.CoolingDown && aas.SkillPhase != ActiveSkillPhase.Ready)
                {
                    yield return null;
                }
            }
        }
    }

    [Category("敌兵/战斗")]
    [Name("强行打断技能")]
    [Description("强行打断技能")]
    public class BT_Enemy_InterruptSkill : BTNode
    {
        [Name("技能编号")]
        public BBParameter<ActorSkillIndex> ActorSkillIndex;

        public override string name => $"强行打断技能 [{ActorSkillIndex.value}]";

        protected override Status OnExecute(Component agent, IBlackboard blackboard)
        {
            if (Actor == null || Actor.ActorAIAgent == null) return Status.Failure;
            if (Actor.ActorActiveSkillDict.TryGetValue(ActorSkillIndex.value, out ActorActiveSkill aas))
            {
                aas.Interrupt();
                return Status.Success;
            }

            return Status.Failure;
        }
    }

    #endregion
}