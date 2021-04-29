﻿using System;
using System.Collections.Generic;
using System.Linq;
using BiangLibrary;
using BiangLibrary.GameDataFormat.Grid;
using FlowCanvas;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

[ExecuteInEditMode]
public class WorldModuleDesignHelper : MonoBehaviour
{
    public WorldModuleFeature WorldModuleFeature;

    public BGM_Theme BGM_ThemeState;

    [LabelText("模组AI")]
    [PropertyOrder(-1)]
    public FlowScript WorldModuleAI;

#if UNITY_EDITOR

    [PropertyOrder(-1)]
    [Button("创建模组AI")]
    [HideIf("WorldModuleAI", null)]
    private void CreateModuleAIFlowScript()
    {
        if (WorldModuleAI != null) return;
        Graph newGraph = (Graph) EditorUtils.CreateAsset(typeof(FlowScript), "Assets/Resources/AI/ModuleAI/" + name + ".asset");
        if (newGraph != null)
        {
            UndoUtility.RecordObject(gameObject, "Create Module AI asset");
            WorldModuleAI = (FlowScript) newGraph;
            UndoUtility.SetDirty(gameObject);
            UndoUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }

    public WorldModuleData ExportWorldModuleData()
    {
        List<Entity_LevelEditor> entities = GetComponentsInChildren<Entity_LevelEditor>().ToList();

        WorldModuleData worldModuleData = new WorldModuleData();
        worldModuleData.InitNormalModuleData();
        worldModuleData.WorldModuleFeature = WorldModuleFeature;
        worldModuleData.BGM_ThemeState = BGM_ThemeState;

        Grid3DBounds boxBounds = new Grid3DBounds();
        int xMin = int.MaxValue;
        int xMax = int.MinValue;
        int yMin = int.MaxValue;
        int yMax = int.MinValue;
        int zMin = int.MaxValue;
        int zMax = int.MinValue;
        foreach (Entity_LevelEditor entity in entities)
        {
            GridPos3D gp = GridPos3D.GetGridPosByLocalTrans(entity.transform, 1);
            EntityData entityData = entity.EntityData.Clone();
            ushort entityTypeIndex = entityData.EntityTypeIndex;
            EntityOccupationData entityOccupationData = ConfigManager.EntityOccupationConfigDict[entityTypeIndex];

            bool isLevelEventTriggerAppearEntity = false;
            foreach (EntityPassiveSkill eps in entity.EntityData.RawEntityExtraSerializeData.EntityPassiveSkills)
            {
                if (eps is EntityPassiveSkill_LevelEventTriggerAppear appear)
                {
                    isLevelEventTriggerAppearEntity = true;
                    EntityPassiveSkill_LevelEventTriggerAppear.Data data = new EntityPassiveSkill_LevelEventTriggerAppear.Data();
                    data.LocalGP = gp;
                    data.EntityData = entity.EntityData.Clone();
                    data.EntityPassiveSkill_LevelEventTriggerAppear = (EntityPassiveSkill_LevelEventTriggerAppear) appear.Clone();
                    worldModuleData.EventTriggerAppearEntityDataList.Add(data);
                    break;
                }
            }

            bool isTriggerEntity = false;
            if (!isLevelEventTriggerAppearEntity)
            {
                isTriggerEntity = entityOccupationData.IsTriggerEntity;
                if (isTriggerEntity)
                {
                    EntityData data = entity.EntityData.Clone();
                    data.LocalGP = gp;
                    worldModuleData.TriggerEntityDataList.Add(data);
                }
            }

            bool spaceAvailable = true;
            List<GridPos3D> entityOccupation_rotated = GridPos3D.TransformOccupiedPositions_XZ(entity.EntityOrientation, entityOccupationData.EntityIndicatorGPs);

            if (!isLevelEventTriggerAppearEntity && !isTriggerEntity)
            {
                foreach (GridPos3D gridPos3D in entityOccupation_rotated)
                {
                    GridPos3D gridPos = gridPos3D + gp;
                    if (entityData.EntityType.TypeDefineType == TypeDefineType.Box)
                    {
                        EntityData overlapEntityData = worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BetweenBoxes[gridPos.x, gridPos.y, gridPos.z];
                        if (overlapEntityData != null)
                        {
                            spaceAvailable = false;
                            string entity1Name = ConfigManager.TypeDefineConfigs[overlapEntityData.EntityType.TypeDefineType].TypeNameDict[overlapEntityData.EntityTypeIndex];
                            string entity2Name = ConfigManager.TypeDefineConfigs[TypeDefineType.Box].TypeNameDict[entityTypeIndex];
                            Debug.Log($"世界模组[{name}]的{gridPos}位置处存在重叠实体{entity1Name}和{entity2Name},已忽略后者");
                        }

                        if (!entityOccupationData.Passable)
                        {
                            overlapEntityData = worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BoxAndActor[gridPos.x, gridPos.y, gridPos.z];
                            if (overlapEntityData != null)
                            {
                                spaceAvailable = false;
                                string entity1Name = ConfigManager.TypeDefineConfigs[overlapEntityData.EntityType.TypeDefineType].TypeNameDict[overlapEntityData.EntityTypeIndex];
                                string entity2Name = ConfigManager.TypeDefineConfigs[TypeDefineType.Box].TypeNameDict[entityTypeIndex];
                                Debug.Log($"世界模组[{name}]的{gridPos}位置处存在重叠实体{entity1Name}和{entity2Name},已忽略后者");
                            }
                        }
                    }
                    else if (entityData.EntityType.TypeDefineType == TypeDefineType.Actor)
                    {
                        EntityData overlapEntityData = worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BoxAndActor[gridPos.x, gridPos.y, gridPos.z];
                        if (overlapEntityData != null)
                        {
                            spaceAvailable = false;
                            string entity1Name = ConfigManager.TypeDefineConfigs[overlapEntityData.EntityType.TypeDefineType].TypeNameDict[overlapEntityData.EntityTypeIndex];
                            string entity2Name = ConfigManager.TypeDefineConfigs[TypeDefineType.Actor].TypeNameDict[entityTypeIndex];
                            Debug.Log($"世界模组[{name}]的{gridPos}位置处存在重叠实体{entity1Name}和{entity2Name},已忽略后者");
                        }
                    }
                }
            }

            if (spaceAvailable)
            {
                foreach (GridPos3D gridPos3D in entityOccupation_rotated)
                {
                    GridPos3D gridPos = gridPos3D + gp;
                    xMin = Mathf.Min(xMin, gridPos.x);
                    xMax = Mathf.Max(xMax, gridPos.x);
                    yMin = Mathf.Min(yMin, gridPos.y);
                    yMax = Mathf.Max(yMax, gridPos.y);
                    zMin = Mathf.Min(zMin, gridPos.z);
                    zMax = Mathf.Max(zMax, gridPos.z);
                }

                if (!isLevelEventTriggerAppearEntity && !isTriggerEntity)
                {
                    worldModuleData[entityData.EntityType.TypeDefineType, gp] = entityData;
                    foreach (GridPos3D gridPos3D in entityOccupation_rotated)
                    {
                        GridPos3D gridPos = gridPos3D + gp;
                        if (entityData.EntityType.TypeDefineType == TypeDefineType.Box)
                        {
                            if (!entityOccupationData.Passable) worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BoxAndActor[gridPos.x, gridPos.y, gridPos.z] = entityData;
                            worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BetweenBoxes[gridPos.x, gridPos.y, gridPos.z] = entityData;
                        }
                        else if (entityData.EntityType.TypeDefineType == TypeDefineType.Actor)
                        {
                            worldModuleData.EntityDataMatrix_Temp_CheckOverlap_BoxAndActor[gridPos.x, gridPos.y, gridPos.z] = entityData;
                        }
                    }
                }
            }
        }

        boxBounds.position = new GridPos3D(xMin, yMin, zMin);
        boxBounds.size = new GridPos3D(xMax - xMin + 1, yMax - yMin + 1, zMax - zMin + 1);
        worldModuleData.BoxBounds = boxBounds;

        List<BornPointDesignHelper> bornPoints = GetComponentsInChildren<BornPointDesignHelper>().ToList();
        foreach (BornPointDesignHelper bp in bornPoints)
        {
            BornPointData data = (BornPointData) bp.BornPointData.Clone();
            GridPos3D gp = GridPos3D.GetGridPosByLocalTrans(bp.transform, 1);
            data.LocalGP = gp;
            worldModuleData.WorldModuleBornPointGroupData.PlayerBornPoints.Add(name + (string.IsNullOrEmpty(data.BornPointAlias) ? "" : "_" + data.BornPointAlias), data);
        }

        worldModuleData.WorldModuleFlowAssetPath = "";
        if (WorldModuleAI)
        {
            string aiAssetPath = AssetDatabase.GetAssetPath(WorldModuleAI);
            if (aiAssetPath.StartsWith("Assets/Resources/") && aiAssetPath.EndsWith(".asset"))
            {
                worldModuleData.WorldModuleFlowAssetPath = aiAssetPath.Replace("Assets/Resources/", "").Replace(".asset", "");
            }
        }

        return worldModuleData;
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = "#9E93383D".HTMLColorToColor();
        Gizmos.DrawWireCube(Vector3.zero + Vector3.one * (WorldModule.MODULE_SIZE / 2f - 0.5f), Vector3.one * WorldModule.MODULE_SIZE);

        // 仅在模组编辑器中绘制坐标标尺
        WorldDesignHelper world = GetComponentInParent<WorldDesignHelper>();
        if (!world)
        {
            for (int x = 0; x < WorldModule.MODULE_SIZE; x++)
            {
                for (int z = 0; z < WorldModule.MODULE_SIZE; z++)
                {
                    if (x == 0 && z == 0)
                    {
                        transform.DrawSpecialTip(new Vector3(x - 0.25f, -0.5f, z - 0.1f), Color.clear, "#6BC04543".HTMLColorToColor(), $"{x}", 7);
                    }
                    else if (z == 0)
                    {
                        transform.DrawSpecialTip(new Vector3(x - 0.25f, -0.5f, z - 0.1f), Color.clear, "#6BC04543".HTMLColorToColor(), $"{x}", 7);
                        Gizmos.color = "#1AFFFFFF".HTMLColorToColor();
                        Gizmos.DrawLine(new Vector3(x, 0, z) - Vector3.one * 0.5f, new Vector3(x, 0, WorldModule.MODULE_SIZE) - Vector3.one * 0.5f);
                    }
                    else if (x == 0)
                    {
                        transform.DrawSpecialTip(new Vector3(x - 0.25f, -0.5f, z - 0.1f), Color.clear, "#66609BF9".HTMLColorToColor(), $"{z}", 7);
                        Gizmos.color = "#1AFFFFFF".HTMLColorToColor();
                        Gizmos.DrawLine(new Vector3(x, 0, z) - Vector3.one * 0.5f, new Vector3(WorldModule.MODULE_SIZE, 0, z) - Vector3.one * 0.5f);
                    }
                }
            }
        }
    }

    [HideInPlayMode]
    [HideInPrefabAssets]
    [ShowInInspector]
    [NonSerialized]
    [BoxGroup("快速替换")]
    [InlineProperty]
    [HideLabel]
    [ValidateInput("ValidateReplaceWorldModuleTypeName", "只能选择WorldModule")]
    [PropertyOrder(3)]
    [GUIColor(0.7f, 0.7f, 0.7f)]
    private TypeSelectHelper ReplaceWorldModuleTypeName =
        new TypeSelectHelper {TypeDefineType = TypeDefineType.WorldModule};

    private bool ValidateReplaceWorldModuleTypeName(TypeSelectHelper value)
    {
        if (value.TypeDefineType == TypeDefineType.WorldModule) return true;
        return false;
    }

    [HideInPlayMode]
    [HideInPrefabAssets]
    [BoxGroup("快速替换")]
    [Button("替换世界模组", ButtonSizes.Large)]
    [GUIColor(0f, 1f, 1f)]
    [PropertyOrder(3)]
    private void ReplaceWorldModule_Editor()
    {
        WorldDesignHelper world = GetComponentInParent<WorldDesignHelper>();
        if (!world)
        {
            Debug.LogError("此功能只能在世界编辑器中使用");
            return;
        }

        GameObject prefab = (GameObject) AssetDatabase.LoadAssetAtPath<Object>(ConfigManager.FindWorldModulePrefabPathByName(TypeDefineType.WorldModule, ReplaceWorldModuleTypeName.TypeName));
        GameObject go = (GameObject) PrefabUtility.InstantiatePrefab(prefab, transform.parent);
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        DestroyImmediate(gameObject);
    }

    public bool SortModule()
    {
        WorldDesignHelper world = GetComponentInParent<WorldDesignHelper>();
        if (world)
        {
            Debug.LogError("此功能只能在模组编辑器中使用");
            return false;
        }

        bool dirty = false;
        dirty |= ArrangeAllRoots();
        dirty |= FormatAllEntityName_Editor();
        dirty |= FormatAllBornPointName_Editor();
        return dirty;
    }

    private bool FormatAllEntityName_Editor()
    {
        bool dirty = false;
        List<Entity_LevelEditor> entities = GetComponentsInChildren<Entity_LevelEditor>().ToList();
        foreach (Entity_LevelEditor entity in entities)
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(entity.gameObject);
            dirty |= entity.RefreshOrientation();
            string prefabName = prefab.name.Replace("_LevelEditor", "");
            if (entity.name != prefabName)
            {
                entity.name = prefabName;
                dirty = true;
            }
        }

        return dirty;
    }

    private bool FormatAllBornPointName_Editor()
    {
        bool dirty = false;
        List<BornPointDesignHelper> bornPoints = GetComponentsInChildren<BornPointDesignHelper>().ToList();
        foreach (BornPointDesignHelper bp in bornPoints)
        {
            dirty |= bp.FormatAllName_Editor();
        }

        return dirty;
    }

    private Transform GetRoot(WorldModuleHierarchyRootType rootType)
    {
        Transform root = transform.Find($"@_{rootType}");
        if (root) return root;
        return transform;
    }

    private bool ArrangeAllRoots()
    {
        bool dirty = false;
        Transform root;
        foreach (WorldModuleHierarchyRootType rootType in Enum.GetValues(typeof(WorldModuleHierarchyRootType)))
        {
            root = transform.Find("@_" + rootType);
            if (root == null)
            {
                root = new GameObject("@_" + rootType).transform;
                root.parent = transform;
                dirty = true;
            }
        }

        List<BornPointDesignHelper> bornPoints = GetComponentsInChildren<BornPointDesignHelper>().ToList();
        root = GetRoot(WorldModuleHierarchyRootType.WorldModuleBornPointsRoot);
        foreach (BornPointDesignHelper bornPoint in bornPoints)
        {
            if (!bornPoint.transform.IsChildOf(root))
            {
                bornPoint.transform.parent = root;
                dirty = true;
            }
        }

        List<Box_LevelEditor> boxes = GetComponentsInChildren<Box_LevelEditor>().ToList();
        root = GetRoot(WorldModuleHierarchyRootType.BoxesRoot);
        foreach (Box_LevelEditor box in boxes)
        {
            if (!box.transform.IsChildOf(root))
            {
                box.transform.parent = root;
                dirty = true;
            }
        }

        List<Actor_LevelEditor> actors = GetComponentsInChildren<Actor_LevelEditor>().ToList();
        root = GetRoot(WorldModuleHierarchyRootType.ActorsRoot);
        foreach (Actor_LevelEditor actor in actors)
        {
            if (!actor.transform.IsChildOf(root))
            {
                actor.transform.parent = root;
                dirty = true;
            }
        }

        return dirty;
    }

    public bool RefreshBoxLevelEditor()
    {
        bool isDirty = false;
        List<Box_LevelEditor> boxes = GetComponentsInChildren<Box_LevelEditor>().ToList();
        foreach (Box_LevelEditor box in boxes)
        {
            isDirty |= box.RefreshData();
        }

        return isDirty;
    }

    public bool RefreshBornPointDesignHelper()
    {
        //bool isDirty = false;
        //List<BornPointDesignHelper> bps = GetComponentsInChildren<BornPointDesignHelper>().ToList();
        //foreach (BornPointDesignHelper bp in bps)
        //{
        //    isDirty |= bp.RefreshData();
        //}

        //return isDirty;
        return false;
    }

#endif
}

public enum WorldModuleHierarchyRootType
{
    BoxesRoot = 0,
    ActorsRoot = 1,
    WorldModuleBornPointsRoot = 2,
}