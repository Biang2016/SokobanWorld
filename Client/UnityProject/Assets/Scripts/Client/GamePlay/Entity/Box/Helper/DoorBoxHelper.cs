﻿using System.Collections.Generic;
using BiangLibrary.GameDataFormat.Grid;
using UnityEngine;

public class DoorBoxHelper : BoxMonoHelper
{
    public Animator DoorAnim;

    public List<EntityIndicator> DoorEntityIndicators = new List<EntityIndicator>();

    private bool open;

    public bool Open
    {
        get { return open; }
        set
        {
            if (open != value)
            {
                open = value;
                DoorAnim.SetTrigger(open ? "Open" : "Close");
                foreach (EntityIndicator doorEntityIndicator in DoorEntityIndicators)
                {
                    GridPos3D offset = doorEntityIndicator.Offset;
                    GridPos offset_rotated = GridPos.RotateGridPos(new GridPos(offset.x, offset.z), Entity.EntityOrientation);
                    GridPos3D offset3D_rotated = new GridPos3D(offset_rotated.x, offset.y, offset_rotated.z);
                    WorldManager.Instance.CurrentWorld.GetBoxByGridPosition(offset3D_rotated + Entity.WorldGP, out WorldModule module, out GridPos3D boxGridLocalGP);
                    Box existedBox = (Box) module[TypeDefineType.Box, boxGridLocalGP];
                    if (!value && existedBox != null) existedBox.DestroySelf(); // 被门夹的箱子自行摧毁 todo 先这样处理
                    module[TypeDefineType.Box, boxGridLocalGP, true] = value ? null : Entity;
                    doorEntityIndicator.gameObject.SetActive(!value);
                }
            }
        }
    }

    public override void OnHelperRecycled()
    {
        base.OnHelperRecycled();
        DoorAnim.SetTrigger("Close");
        foreach (EntityIndicator doorEntityIndicator in DoorEntityIndicators)
        {
            doorEntityIndicator.gameObject.SetActive(true);
        }
    }

    public override void OnHelperUsed()
    {
        base.OnHelperUsed();
    }
}