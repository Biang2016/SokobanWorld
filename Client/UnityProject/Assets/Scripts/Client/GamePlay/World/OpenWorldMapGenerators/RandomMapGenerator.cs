﻿using BiangLibrary.GameDataFormat;

public sealed class RandomMapGenerator : MapGenerator
{
    public RandomMapGenerator(OpenWorld.GenerateLayerData layerData, int width, int height, uint seed, OpenWorld openWorld) : base(layerData, width, height, seed, openWorld)
    {
    }

    public override void ApplyToWorldMap()
    {
        if (GenerateLayerData.m_GenerateAlgorithm == OpenWorld.GenerateAlgorithm.Random && GenerateLayerData.CountPer10KGrid == 0) return; // 避免大量运算
        for (int module_x = 0; module_x < Width / WorldModule.MODULE_SIZE; module_x++)
        for (int module_z = 0; module_z < Height / WorldModule.MODULE_SIZE; module_z++)
        {
            uint module_Seed = (uint) (Seed + module_x * 100 + module_z); // 每个模组的Seed独一无二
            SRandom = new SRandom(module_Seed);

            for (int local_x = 0; local_x < WorldModule.MODULE_SIZE; local_x++)
            for (int local_z = 0; local_z < WorldModule.MODULE_SIZE; local_z++)
            {
                bool genSuc = SRandom.Range(0, 10000) < GenerateLayerData.CountPer10KGrid;
                if (genSuc)
                {
                    int world_x = module_x * WorldModule.MODULE_SIZE + local_x;
                    int world_z = module_z * WorldModule.MODULE_SIZE + local_z;
                    TryOverrideToWorldMap(world_x, world_z);
                }
            }
        }
    }
}