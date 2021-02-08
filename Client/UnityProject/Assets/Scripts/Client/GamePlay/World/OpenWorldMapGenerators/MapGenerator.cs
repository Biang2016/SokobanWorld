﻿using BiangLibrary.GameDataFormat;
using BiangLibrary.GameDataFormat.Grid;

public abstract class MapGenerator
{
    protected uint Seed;
    protected SRandom SRandom;
    protected OpenWorld.GenerateBoxLayerData GenerateBoxLayerData;
    protected ushort BoxTypeIndex;

    protected int Width;
    protected int Height;
    protected GridPos LeaveSpaceForPlayerBP;

    protected MapGenerator(OpenWorld.GenerateBoxLayerData boxLayerData, int width, int height, uint seed, GridPos leaveSpaceForPlayerBP)
    {
        SRandom = new SRandom(seed);
        GenerateBoxLayerData = boxLayerData;
        BoxTypeIndex = ConfigManager.GetBoxTypeIndex(boxLayerData.BoxTypeName);
        Width = width;
        Height = height;
        LeaveSpaceForPlayerBP = leaveSpaceForPlayerBP;
    }

    public virtual void WriteMapInfoIntoWorldModuleData(WorldModuleData moduleData, int module_x, int module_z)
    {
    }

    protected void TryOverrideBoxInfoOnMap(WorldModuleData moduleData, ushort existedBoxTypeIndex, int x, int z)
    {
        if (existedBoxTypeIndex != 0)
        {
            if (GenerateBoxLayerData.AllowReplacedBoxTypeNameSet.Contains(ConfigManager.GetBoxTypeName(existedBoxTypeIndex)))
            {
                moduleData.RawBoxMatrix[x, 0, z] = BoxTypeIndex;
                moduleData.BoxMatrix[x, 0, z] = BoxTypeIndex;
            }
        }
        else
        {
            if (!GenerateBoxLayerData.OnlyOverrideAnyBox)
            {
                moduleData.RawBoxMatrix[x, 0, z] = BoxTypeIndex;
                moduleData.BoxMatrix[x, 0, z] = BoxTypeIndex;
            }
        }
    }
}