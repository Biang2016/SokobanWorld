﻿using System;
using BiangLibrary.GameDataFormat.Grid;
using Sirenix.OdinInspector;

[Serializable]
public class BoxPassiveSkillAction_ChangeBoxType : BoxPassiveSkillAction, BoxPassiveSkillAction.IPureAction
{
    protected override string Description => "更改箱子类型";

    [BoxName]
    [LabelText("更改箱子类型为")]
    [ValueDropdown("GetAllBoxTypeNames", IsUniqueList = true, DropdownTitle = "选择箱子类型", DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)]
    public string ChangeBoxTypeTo = "None";

    public void Execute()
    {
        if (Box.State == Box.States.Static)
        {
            WorldModule module = WorldManager.Instance.CurrentWorld.GetModuleByGridPosition(Box.WorldGP);
            if (module != null)
            {
                GridPos3D localGP = Box.LocalGP;
                Box.DestroyBox();
                ushort boxTypeIndex = ConfigManager.GetBoxTypeIndex(ChangeBoxTypeTo);
                if (boxTypeIndex != 0) module.GenerateBox(boxTypeIndex, localGP);
            }
        }
    }

    protected override void ChildClone(BoxPassiveSkillAction newAction)
    {
        base.ChildClone(newAction);
        BoxPassiveSkillAction_ChangeBoxType action = ((BoxPassiveSkillAction_ChangeBoxType)newAction);
        action.ChangeBoxTypeTo = ChangeBoxTypeTo;
    }

    public override void CopyDataFrom(BoxPassiveSkillAction srcData)
    {
        base.CopyDataFrom(srcData);
        BoxPassiveSkillAction_ChangeBoxType action = ((BoxPassiveSkillAction_ChangeBoxType) srcData);
        ChangeBoxTypeTo = action.ChangeBoxTypeTo;
    }
}