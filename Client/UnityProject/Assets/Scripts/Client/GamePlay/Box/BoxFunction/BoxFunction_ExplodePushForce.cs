﻿using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class BoxFunction_ExplodePushForce : BoxFunctionBase
{
    protected override string BoxFunctionDisplayName => "箱子撞击爆炸AOE对周围Box施加推力";

    [LabelText("碰撞爆炸推力半径")]
    public int ExplodePushRadius = 3;

    public override void OnFlyingCollisionEnter(Collision collision)
    {
        base.OnFlyingCollisionEnter(collision);
        ExplodePushBox(Box, Box.transform.position, ExplodePushRadius);
    }

    public override void OnBeingKickedCollisionEnter(Collision collision)
    {
        base.OnBeingKickedCollisionEnter(collision);
        ExplodePushBox(Box, Box.transform.position, ExplodePushRadius);
    }

    private void ExplodePushBox(Box m_Box, Vector3 center, int radius)
    {
        List<Box> boxes = new List<Box>();
        GridPos3D curGP = m_Box.transform.position.ToGridPos3D();
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                GridPos3D targetGP = curGP + new GridPos3D(x, 0, z);
                Box box = WorldManager.Instance.CurrentWorld.GetBoxByGridPosition(targetGP, out WorldModule module, out GridPos3D gp);
                if (box != null)
                {
                    boxes.Add(box);
                }
            }
        }

        Collider[] colliders = Physics.OverlapSphere(center, radius, LayerManager.Instance.LayerMask_HitBox_Box);
        foreach (Collider collider in colliders)
        {
            Box box = collider.gameObject.GetComponentInParent<Box>();
            if (box != null && box.Interactable && box != m_Box)
            {
                if (!boxes.Contains(box))
                {
                    if (box.State == Box.States.BeingKicked || box.State == Box.States.BeingPushed || box.State == Box.States.PushingCanceling || box.State == Box.States.PushingCanceling)
                    {
                        Vector3 diff = box.transform.position - center;
                        if (diff.x > diff.z)
                        {
                            diff.z = 0;
                        }
                        else if (diff.z > diff.x)
                        {
                            diff.x = 0;
                        }

                        diff.y = 0;
                        box.Kick(diff, 15f, m_Box.LastTouchActor);
                    }
                }
            }
        }
    }

    protected override void ChildClone(BoxFunctionBase newBF)
    {
        base.ChildClone(newBF);
        BoxFunction_ExplodePushForce bf = ((BoxFunction_ExplodePushForce) newBF);
        bf.ExplodePushRadius = ExplodePushRadius;
    }

    public override void CopyDataFrom(BoxFunctionBase srcData)
    {
        base.CopyDataFrom(srcData);
        BoxFunction_ExplodePushForce bf = ((BoxFunction_ExplodePushForce) srcData);
        ExplodePushRadius = bf.ExplodePushRadius;
    }
}