﻿using System;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class BuffEditorWindow : EditorWindow
{
    [MenuItem("开发工具/配置/Buff克制编辑")]
    public static void ShowBuffEditorWindow()
    {
        BuffEditorWindow window = new BuffEditorWindow();
        window.ShowUtility();
    }

    void OnEnable()
    {
        name = "Buff克制编辑";
        Init();
    }

    private GUITable table;

    private void Init()
    {
        int buffTypeEnumCount = Enum.GetValues(typeof(BuffAttribute)).Length;
        string[] descriptionsOfBuffType = new string[buffTypeEnumCount];

        {
            Type enumType = typeof(BuffAttribute);
            MemberInfo[] memberInfos = enumType.GetMembers();
            int descIndex = 0;
            foreach (MemberInfo mi in memberInfos)
            {
                if (mi.DeclaringType == enumType)
                {
                    object[] valueAttributes = mi.GetCustomAttributes(typeof(LabelTextAttribute), false);
                    foreach (object va in valueAttributes)
                    {
                        if (va is LabelTextAttribute a)
                        {
                            descriptionsOfBuffType[descIndex] = a.Text;
                            descIndex++;
                        }
                    }
                }
            }
        }

        int relationshipEnumCount = Enum.GetValues(typeof(BuffAttributeRelationship)).Length;
        string[] descriptionsOfRelationship = new string[relationshipEnumCount];
        {
            Type enumType = typeof(BuffAttributeRelationship);
            MemberInfo[] memberInfos = enumType.GetMembers();
            int descIndex = 0;
            foreach (MemberInfo mi in memberInfos)
            {
                if (mi.DeclaringType == enumType)
                {
                    object[] valueAttributes = mi.GetCustomAttributes(typeof(LabelTextAttribute), false);
                    foreach (object va in valueAttributes)
                    {
                        if (va is LabelTextAttribute a)
                        {
                            descriptionsOfRelationship[descIndex] = a.Text;
                            descIndex++;
                        }
                    }
                }
            }
        }

        BuffAttributeRelationship[,] arr = ConfigManager.GetBuffAttributeMatrixAsset().BuffAttributeMatrix;
        if (arr != null)
        {
            this.table = GUITable.Create(
                twoDimArray: arr,
                drawElement: (rect, x, y) =>
                {
                    if (x > y) return;
                    Rect left = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
                    Rect right = new Rect(rect.x + rect.width / 2f, rect.y, rect.width / 2f, rect.height);
                    BuffAttributeRelationship newValue = (BuffAttributeRelationship) EditorGUI.EnumPopup(left, arr[x, y]);
                    if (x != y && (newValue == BuffAttributeRelationship.MaxDominant))
                    {
                        Debug.LogError($"【Buff相克矩阵】{(BuffAttribute) x}和{(BuffAttribute) y}之间的关系有误，异种BuffAttribute之间的关系不允许选用{newValue}");
                    }
                    else
                    {
                        arr[x, y] = newValue;
                        arr[y, x] = newValue;
                        EditorGUI.LabelField(right, descriptionsOfRelationship[(int) newValue]);
                    }
                },
                horizontalLabel: null, // horizontalLabel is optional and can be null.
                columnLabels: (rect, x) => GUI.Label(rect, descriptionsOfBuffType[x]), // columnLabels is optional and can be null.
                verticalLabel: null, // verticalLabel is optional and can be null.
                rowLabels: (rect, y) => GUI.Label(rect, descriptionsOfBuffType[y]) // rowLabels is optional and can be null.
            );
        }
        else
        {
            table = null;
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Buff克制矩阵");
        try
        {
            table?.DrawTable();
        }
        catch
        {
        }

        if (GUILayout.Button("加载 / 修改矩阵维度后先加载再保存，否则丢失数据"))
        {
            ConfigManager.LoadBuffAttributeMatrix(DataFormat.Binary);
            Init();
        }

        if (GUILayout.Button("保存"))
        {
            ConfigManager.ExportBuffAttributeMatrix(DataFormat.Binary);
            ConfigManager.LoadBuffAttributeMatrix(DataFormat.Binary);
            Init();
        }

        if (GUILayout.Button("新建"))
        {
            if (EditorUtility.DisplayDialog("提示", $"新建矩阵将覆盖现有矩阵，是否继续?", "继续", "取消"))
            {
                ConfigManager.CreateNewBuffAttributeMatrixAsset();
            }
        }
    }
}