using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class KyleUICodeGenWnd : EditorWindow
{
    [MenuItem("Tools/KyleUIGen")]
    public static void KyleUICodeGenrator()
    {
        GetWindow<KyleUICodeGenWnd>();
    }

    public class CodeGenContext
    {
        public string UIName;
        public string AssetPath;
        public string FolderName;
        public GameObject uiRoot;
        public string codePath;
        public bool isCell;

        public string GetCodePath()
        {
            var bindComp = uiRoot.GetComponent<BindObjectMono>();
            if(bindComp!=null && !string.IsNullOrEmpty(bindComp.ViewScripts))
            {
                return bindComp.ViewScripts;
            }
            var folderPath =$"Assets/GameScripts/GameLogic/GameUICode/{m_codeGenContext.FolderName}";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string codePath =Path.Combine(folderPath,$"{m_codeGenContext.UIName}.cs");
            return codePath;
        }
    }

    private static CodeGenContext m_codeGenContext = new CodeGenContext();
    private void OnGUI()
    {
        var selectGo = GetSelectedRoot();
        if (selectGo == null)
        {
            GUILayout.Label("先选择UI资源根节点");
        }
        else
        {
            GUILayout.Label("当前选中"+selectGo.name);
            string path = GetSelectedAssetPath(selectGo);
            EditorGUILayout.LabelField("Prefab Path: " + path);
        }

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("生成Window代码",GUILayout.Height(40)))
        {
            if (selectGo == null)
            {
                this.ShowNotification(new GUIContent("请先选中UI资源"));
                return;
            }
            Generate(selectGo,false);
        }
        if(GUILayout.Button("生成Cell代码",GUILayout.Height(40)))
        {
            Generate(selectGo,true);
        }
        GUILayout.EndHorizontal();
    }

    private static void Generate(GameObject selectGo,bool isCell)
    {
        
        var prefabGo = PrefabUtility.GetNearestPrefabInstanceRoot(selectGo);
        if (prefabGo == null)
        {
            prefabGo = selectGo;
         
        }
        m_codeGenContext = new CodeGenContext();
        m_codeGenContext.UIName = prefabGo.name;
        m_codeGenContext.uiRoot = prefabGo;
        m_codeGenContext.isCell = isCell;
        var assetPath =GetSelectedAssetPath(prefabGo);
        if(string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("没能找到路径");
            return;
        }
        m_codeGenContext.AssetPath =assetPath;
        m_codeGenContext.FolderName = new FileInfo(m_codeGenContext.AssetPath).Directory.Name;
        if (isCell)
        {
            GenerateCellViewCode();
        }
        else
        {
            GenerateWindowCode();
        }
        Debug.Log("生成成功");
    }

    static string GetSelectedAssetPath(GameObject go)
    {
        if (PrefabStageUtility.GetCurrentPrefabStage() != null)
        {
            return PrefabStageUtility.GetCurrentPrefabStage().assetPath;
        }
        return   PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
    }
    
    GameObject GetSelectedRoot()
    {
        if(PrefabStageUtility.GetCurrentPrefabStage() != null )
        {
            // if (PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot ==
            //     PrefabUtility.GetNearestPrefabInstanceRoot(Selection.activeGameObject))
            // {
                return PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot;
            // }
        }

        return Selection.activeGameObject;
    }
    static void GenerateWindowCode()
    {

        string windowCodeTemplate =
@"using UnityEngine;

namespace GameLogic
{
	[UI(""ASSET_PATH"")]
	public class UIName: UIWindowBase
	{
		#region 节点定义
		
		#endregion 节点定义

        protected override void OnBindUIEvent()
        {
            
        }
	}
}
";
        var codePath = m_codeGenContext.GetCodePath();
        m_codeGenContext.codePath = m_codeGenContext.GetCodePath();
        string fileContent;
        bool needRefresh = false;
        if (!File.Exists(codePath))
        {
            fileContent = windowCodeTemplate.Replace("UIName",m_codeGenContext.UIName);
            fileContent = fileContent.Replace("ASSET_PATH", m_codeGenContext.AssetPath);
            needRefresh = true;
        }
        else
        {
            fileContent = File.ReadAllText(codePath);
        }
        InsertFieldDeclare(ref fileContent, m_codeGenContext.uiRoot.GetComponent<BindObjectMono>());
        File.WriteAllText(codePath,fileContent);
        if (needRefresh)
        {
            AssetDatabase.Refresh();
        }
    }
    
    static void GenerateCellViewCode()
    {

        string windowCodeTemplate =
            @"using UnityEngine;

namespace GameLogic
{
	[UI(""ASSET_PATH"")]
	public class UIName: UICellViewBase
	{
		#region 节点定义
		
		#endregion 节点定义

        protected override void OnBindUIEvent()
        {
            
        }
	}
}
";

        var codePath = m_codeGenContext.GetCodePath();
        m_codeGenContext.codePath = codePath;
        string fileContent;
        bool needRefresh = false;
        if (!File.Exists(codePath))
        {
            fileContent = windowCodeTemplate.Replace("UIName",m_codeGenContext.UIName);
            fileContent = fileContent.Replace("ASSET_PATH", m_codeGenContext.AssetPath);
            needRefresh = true;
        }
        else
        {
            fileContent = File.ReadAllText(codePath);
        }
        InsertFieldDeclare(ref fileContent, m_codeGenContext.uiRoot.GetComponent<BindObjectMono>());
        File.WriteAllText(codePath,fileContent);
        if (needRefresh)
        {
            AssetDatabase.Refresh();
        }
    }


    
    private static void InsertFieldDeclare(ref string code, BindObjectMono referenceCollector)
    {
        if (referenceCollector == null)
            return;

        if (referenceCollector.ViewScripts!= m_codeGenContext.codePath)
        {
            referenceCollector.ViewScripts = m_codeGenContext.codePath;
            EditorUtility.SetDirty(referenceCollector);
            AssetDatabase.Refresh();
        }
        
        

        int beginIndex = code.IndexOf("#region 节点定义");
        int endIndex = code.IndexOf("#endregion 节点定义");
        if (beginIndex == -1 || endIndex == -1)
            return;

        var allGameObject = referenceCollector.objDict;
        string temp = code.Substring(0, beginIndex);
        temp += "#region 节点定义\n";
        temp += GenerateField(ref allGameObject);
        temp += "\n";
        temp += "        "+code.Substring(endIndex);

        code = temp;
    }

    private const string FileGetTemplate = @"
		protected GameObject RefKey {  get	{   return RefBind.GetGO(""RefKey"");	}}";
    private static string GenerateField(ref GenericDictionary<string, GameObject> allGameObject)
    {
        string add = "";
        foreach (var kv in allGameObject)
        {
            if (kv.Value != null)
            {
                add += FileGetTemplate.Replace("RefKey", kv.Key);
            }
        }
        return add;
    }

    public static void GenerateCodeDeclare(GameObject targetGameObject, string path)
    {
       Generate(targetGameObject,true);
    }
}
