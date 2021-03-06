﻿using System;
using System.Collections.Generic;
using BiangLibrary.CloneVariant;
using BiangLibrary.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class EntityOccupationData : IClone<EntityOccupationData>
{
    [LabelText("长方体外形")]
    public bool IsShapeCuboid = false;

    [LabelText("包围尺寸")]
    [SerializeField]
    public BoundsInt BoundsInt;

    [LabelText("格子占位信息")]
    [ListDrawerSettings(ListElementLabelName = "ToString")]
    public List<GridPos3D> EntityIndicatorGPs = new List<GridPos3D>();

    public Dictionary<GridPosR.Orientation, List<GridPos3D>> BoxIndicatorGPs_RotatedDict;

    public void Clear()
    {
        IsShapeCuboid = false;
        BoundsInt = default;
        EntityIndicatorGPs.Clear();
    }

    public void CalculateEveryOrientationOccupationGPs()
    {
        if (BoxIndicatorGPs_RotatedDict == null) BoxIndicatorGPs_RotatedDict = new Dictionary<GridPosR.Orientation, List<GridPos3D>>();
        BoxIndicatorGPs_RotatedDict.Clear();
        BoxIndicatorGPs_RotatedDict.Add(GridPosR.Orientation.Up, GridPos3D.TransformOccupiedPositions_XZ(GridPosR.Orientation.Up, EntityIndicatorGPs));
        BoxIndicatorGPs_RotatedDict.Add(GridPosR.Orientation.Right, GridPos3D.TransformOccupiedPositions_XZ(GridPosR.Orientation.Right, EntityIndicatorGPs));
        BoxIndicatorGPs_RotatedDict.Add(GridPosR.Orientation.Down, GridPos3D.TransformOccupiedPositions_XZ(GridPosR.Orientation.Down, EntityIndicatorGPs));
        BoxIndicatorGPs_RotatedDict.Add(GridPosR.Orientation.Left, GridPos3D.TransformOccupiedPositions_XZ(GridPosR.Orientation.Left, EntityIndicatorGPs));
    }

    public EntityOccupationData Clone()
    {
        EntityOccupationData newData = new EntityOccupationData();
        newData.IsShapeCuboid = IsShapeCuboid;
        newData.BoundsInt = BoundsInt;
        newData.EntityIndicatorGPs = EntityIndicatorGPs.Clone();
        return newData;
    }
}