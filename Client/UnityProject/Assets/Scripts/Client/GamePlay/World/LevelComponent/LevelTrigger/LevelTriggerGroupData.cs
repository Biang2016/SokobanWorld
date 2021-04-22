﻿using System.Collections.Generic;
using BiangLibrary.CloneVariant;

public class LevelTriggerGroupData : IClone<LevelTriggerGroupData>
{
    public List<LevelTriggerBase.Data> TriggerDataList = new List<LevelTriggerBase.Data>();

    public LevelTriggerGroupData Clone()
    {
        LevelTriggerGroupData newData = new LevelTriggerGroupData();
        newData.TriggerDataList = TriggerDataList.Clone<LevelTriggerBase.Data, LevelComponentData>();
        return newData;
    }
}