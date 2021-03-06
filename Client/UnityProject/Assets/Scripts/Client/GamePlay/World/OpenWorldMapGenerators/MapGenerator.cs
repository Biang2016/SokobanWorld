﻿using BiangLibrary.GameDataFormat;
using BiangLibrary.GameDataFormat.Grid;

public abstract class MapGenerator
{
    protected uint Seed;
    protected SRandom SRandom;
    protected OpenWorld.GenerateLayerData GenerateLayerData;

    protected MapGeneratorType MapGeneratorType;

    protected ushort BoxTypeIndex;
    protected ushort EnemyTypeIndex;
    protected ushort StaticLayoutTypeIndex;

    protected OpenWorld m_OpenWorld;
    protected ushort[,,] WorldMap => m_OpenWorld.WorldMap;
    protected GridPosR.Orientation[,,] WorldMapOrientation => m_OpenWorld.WorldMapOrientation;
    protected ushort[,,] WorldMap_Occupied => m_OpenWorld.WorldMap_Occupied;
    protected ushort[,,] WorldMap_StaticLayoutOccupied => m_OpenWorld.WorldMap_StaticLayoutOccupied;

    protected int Width;
    protected int Depth;
    protected int Height = WorldModule.MODULE_SIZE;

    protected MapGenerator(OpenWorld.GenerateLayerData layerData, int width, int depth, uint seed, OpenWorld openWorld)
    {
        m_OpenWorld = openWorld;
        Seed = seed;
        SRandom = new SRandom(seed);
        GenerateLayerData = layerData;
        switch (layerData)
        {
            case OpenWorld.GenerateStaticLayoutLayerData staticLayoutLayerData:
            {
                MapGeneratorType = MapGeneratorType.StaticLayout;
                StaticLayoutTypeIndex = ConfigManager.GetStaticLayoutTypeIndex(staticLayoutLayerData.StaticLayoutTypeName);
                break;
            }
            case OpenWorld.GenerateBoxLayerData boxLayerData:
            {
                MapGeneratorType = MapGeneratorType.Box;
                BoxTypeIndex = ConfigManager.GetBoxTypeIndex(boxLayerData.BoxTypeName);
                break;
            }
            case OpenWorld.GenerateActorLayerData actorLayerData:
            {
                MapGeneratorType = MapGeneratorType.Actor;
                EnemyTypeIndex = ConfigManager.GetEnemyTypeIndex(actorLayerData.ActorTypeName);
                break;
            }
        }

        Width = width;
        Depth = depth;
    }

    public abstract void ApplyToWorldMap();

    protected void TryOverrideToWorldMap(int world_x, int world_y, int world_z)
    {
        ushort existedIndex = WorldMap[world_x, world_y - Height, world_z];
        ushort existedIndex_StaticLayoutIndex = WorldMap_StaticLayoutOccupied[world_x, world_y - Height, world_z];
        if (existedIndex_StaticLayoutIndex != 0) return; // 不能覆盖在静态布局上
        ConfigManager.TypeStartIndex existedIndexType = existedIndex.ConvertToTypeStartIndex();
        switch (MapGeneratorType)
        {
            case MapGeneratorType.StaticLayout:
            {
                OpenWorld.GenerateStaticLayoutLayerData staticLayoutLayerData = (OpenWorld.GenerateStaticLayoutLayerData) GenerateLayerData;
                ushort staticLayoutTypeIndex = ConfigManager.GetStaticLayoutTypeIndex(staticLayoutLayerData.StaticLayoutTypeName);
                WorldModuleData staticLayoutData = ConfigManager.GetStaticLayoutDataConfig(staticLayoutTypeIndex, false); // 不拷贝，只读数据，避免运行时动态加载GC
                GridPosR.Orientation staticLayoutOrientation = (GridPosR.Orientation) SRandom.Next(4);

                // Check Space
                bool spaceAvailable = true;
                for (int sl_local_x = 0; sl_local_x < WorldModule.MODULE_SIZE; sl_local_x++)
                {
                    if (!spaceAvailable) break;
                    for (int sl_local_y = 0; sl_local_y < WorldModule.MODULE_SIZE; sl_local_y++)
                    {
                        if (!spaceAvailable) break;
                        for (int sl_local_z = 0; sl_local_z < WorldModule.MODULE_SIZE; sl_local_z++)
                        {
                            if (!spaceAvailable) break;
                            int rot_local_x = sl_local_x;
                            int rot_local_z = sl_local_z;
                            for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                            {
                                // 注意此处是逆向旋转的
                                int temp_z = rot_local_z;
                                rot_local_z = rot_local_x;
                                rot_local_x = WorldModule.MODULE_SIZE - 1 - temp_z;
                            }

                            ushort boxTypeIndex = staticLayoutData.RawBoxMatrix[rot_local_x, sl_local_y, rot_local_z];
                            if (boxTypeIndex != 0)
                            {
                                EntityOccupationData occupation = ConfigManager.GetEntityOccupationData(boxTypeIndex);
                                GridPosR.Orientation rot = staticLayoutData.RawBoxOrientationMatrix[rot_local_x, sl_local_y, rot_local_z];
                                for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                                {
                                    rot = GridPosR.RotateOrientationClockwise90(rot);
                                }

                                foreach (GridPos3D gridPos in occupation.BoxIndicatorGPs_RotatedDict[rot])
                                {
                                    int box_grid_world_x = world_x + sl_local_x + gridPos.x;
                                    int box_grid_world_y = world_y + sl_local_y + gridPos.y;
                                    int box_grid_world_z = world_z + sl_local_z + gridPos.z;
                                    if (box_grid_world_x > 0 && box_grid_world_x < Width - 1 && box_grid_world_y - Height >= 0 && box_grid_world_y - Height < Height && box_grid_world_z > 0 && box_grid_world_z < Depth - 1) // 静态布局不要贴边
                                    {
                                        if (WorldMap_Occupied[box_grid_world_x, box_grid_world_y - Height, box_grid_world_z] != 0)
                                        {
                                            spaceAvailable = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        spaceAvailable = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                if (spaceAvailable)
                {
                    for (int sl_local_x = 0; sl_local_x < WorldModule.MODULE_SIZE; sl_local_x++)
                    for (int sl_local_y = 0; sl_local_y < WorldModule.MODULE_SIZE; sl_local_y++)
                    for (int sl_local_z = 0; sl_local_z < WorldModule.MODULE_SIZE; sl_local_z++)
                    {
                        int rot_local_x = sl_local_x;
                        int rot_local_z = sl_local_z;
                        for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                        {
                            // 注意此处是逆向旋转的
                            int temp_z = rot_local_z;
                            rot_local_z = rot_local_x;
                            rot_local_x = WorldModule.MODULE_SIZE - 1 - temp_z;
                        }

                        ushort boxTypeIndex = staticLayoutData.RawBoxMatrix[rot_local_x, sl_local_y, rot_local_z];
                        if (boxTypeIndex != 0)
                        {
                            int box_world_x = world_x + sl_local_x;
                            int box_world_y = world_y + sl_local_y;
                            int box_world_z = world_z + sl_local_z;
                            WorldMap[box_world_x, box_world_y - Height, box_world_z] = boxTypeIndex;
                            EntityOccupationData occupation = ConfigManager.GetEntityOccupationData(boxTypeIndex);
                            GridPosR.Orientation rot = staticLayoutData.RawBoxOrientationMatrix[rot_local_x, sl_local_y, rot_local_z];
                            for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                            {
                                rot = GridPosR.RotateOrientationClockwise90(rot);
                            }

                            WorldMapOrientation[box_world_x, box_world_y - Height, box_world_z] = rot;
                            foreach (GridPos3D gridPos in occupation.BoxIndicatorGPs_RotatedDict[rot])
                            {
                                int box_grid_world_x = box_world_x + gridPos.x;
                                int box_grid_world_y = box_world_y + gridPos.y;
                                int box_grid_world_z = box_world_z + gridPos.z;
                                WorldMap_Occupied[box_grid_world_x, box_grid_world_y - Height, box_grid_world_z] = boxTypeIndex;
                            }
                        }
                    }

                    for (int sl_local_x = staticLayoutData.BoxBounds.x_min; sl_local_x <= staticLayoutData.BoxBounds.x_max; sl_local_x++)
                    for (int sl_local_y = staticLayoutData.BoxBounds.y_min; sl_local_y <= staticLayoutData.BoxBounds.y_max; sl_local_y++)
                    for (int sl_local_z = staticLayoutData.BoxBounds.z_min; sl_local_z <= staticLayoutData.BoxBounds.z_max; sl_local_z++)
                    {
                        int rot_local_x = sl_local_x;
                        int rot_local_z = sl_local_z;
                        for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                        {
                            int temp_x = rot_local_x;
                            rot_local_x = rot_local_z;
                            rot_local_z = WorldModule.MODULE_SIZE - 1 - temp_x;
                        }

                        int layoutOccupied_world_x = world_x + rot_local_x;
                        int layoutOccupied_world_y = world_y + sl_local_y;
                        int layoutOccupied_world_z = world_z + rot_local_z;
                        if (layoutOccupied_world_x >= 0 && layoutOccupied_world_x < Width && layoutOccupied_world_y - Height >= 0 && layoutOccupied_world_y - Height < Height && layoutOccupied_world_z >= 0 && layoutOccupied_world_z < Depth)
                        {
                            WorldMap_StaticLayoutOccupied[layoutOccupied_world_x, layoutOccupied_world_y - Height, layoutOccupied_world_z] = staticLayoutTypeIndex;
                        }
                    }

                    foreach (BornPointData enemyBornPoint in staticLayoutData.WorldModuleBornPointGroupData.EnemyBornPoints)
                    {
                        ushort enemyTypeIndex = ConfigManager.GetEnemyTypeIndex(enemyBornPoint.ActorType);
                        if (enemyTypeIndex != 0)
                        {
                            GridPos3D enemyGP_rotated = enemyBornPoint.LocalGP;
                            for (int rotCount = 0; rotCount < (int) staticLayoutOrientation; rotCount++)
                            {
                                enemyGP_rotated = new GridPos3D(enemyGP_rotated.z, enemyGP_rotated.y, WorldModule.MODULE_SIZE - 1 - enemyGP_rotated.x);
                            }

                            int x = world_x + enemyGP_rotated.x;
                            int y = world_y + enemyGP_rotated.y;
                            int z = world_z + enemyGP_rotated.z;
                            WorldMap[x, y - Height, z] = enemyTypeIndex;
                            WorldMap_Occupied[x, y - Height, z] = enemyTypeIndex;
                        }
                    }
                }

                break;
            }

            case MapGeneratorType.Box:
            {
                bool spaceAvailable = true;
                EntityOccupationData occupation = ConfigManager.GetEntityOccupationData(BoxTypeIndex);
                foreach (GridPos3D gridPos in occupation.BoxIndicatorGPs_RotatedDict[GridPosR.Orientation.Up]) // todo Orientation Random
                {
                    if (!spaceAvailable) break;
                    int box_grid_world_x = world_x + gridPos.x;
                    int box_grid_world_y = world_y + gridPos.y;
                    int box_grid_world_z = world_z + gridPos.z;
                    if (box_grid_world_x >= 0 && box_grid_world_x < Width && box_grid_world_y - Height >= 0 && box_grid_world_y - Height < Height && box_grid_world_z >= 0 && box_grid_world_z < Depth)
                    {
                        if (WorldMap_StaticLayoutOccupied[box_grid_world_x, box_grid_world_y - Height, box_grid_world_z] != 0)
                        {
                            spaceAvailable = false;
                            break;
                        }

                        ushort occupiedIndex = WorldMap_Occupied[box_grid_world_x, box_grid_world_y - Height, box_grid_world_z];
                        ConfigManager.TypeStartIndex occupiedIndexType = occupiedIndex.ConvertToTypeStartIndex();
                        switch (occupiedIndexType)
                        {
                            case ConfigManager.TypeStartIndex.Box when !GenerateLayerData.AllowReplacedBoxTypeNameSet.Contains(ConfigManager.GetBoxTypeName(occupiedIndex)):
                            case ConfigManager.TypeStartIndex.None when GenerateLayerData.OnlyOverrideAnyBox:
                            case ConfigManager.TypeStartIndex.Player:
                                spaceAvailable = false;
                                break;
                        }
                    }
                    else
                    {
                        spaceAvailable = false;
                        break;
                    }
                }

                if (spaceAvailable)
                {
                    WorldMap[world_x, 0, world_z] = BoxTypeIndex;
                    WorldMapOrientation[world_x, 0, world_z] = GridPosR.Orientation.Up;
                    foreach (GridPos3D gridPos in occupation.BoxIndicatorGPs_RotatedDict[GridPosR.Orientation.Up]) // todo Orientation Random
                    {
                        int box_grid_world_x = world_x + gridPos.x;
                        int box_grid_world_z = world_z + gridPos.z;
                        WorldMap_Occupied[box_grid_world_x, 0, box_grid_world_z] = BoxTypeIndex;
                    }
                }

                break;
            }
            case MapGeneratorType.Actor:
            {
                if (WorldMap_StaticLayoutOccupied[world_x, 0, world_z] != 0) break;

                bool spaceAvailable = true;
                switch (existedIndexType)
                {
                    case ConfigManager.TypeStartIndex.Box when !GenerateLayerData.AllowReplacedBoxTypeNameSet.Contains(ConfigManager.GetBoxTypeName(existedIndex)):
                    case ConfigManager.TypeStartIndex.None when GenerateLayerData.OnlyOverrideAnyBox:
                    case ConfigManager.TypeStartIndex.Player:
                        spaceAvailable = false;
                        break;
                }

                if (spaceAvailable)
                {
                    WorldMap[world_x, 0, world_z] = EnemyTypeIndex;
                    WorldMap_Occupied[world_x, 0, world_z] = EnemyTypeIndex;
                }

                break;
            }
        }
    }
}

public enum MapGeneratorType
{
    StaticLayout,
    Box,
    Actor,
}