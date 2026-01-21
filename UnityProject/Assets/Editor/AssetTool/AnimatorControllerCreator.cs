using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

public class AnimatorControllerCreator : EditorWindow
{
    [MenuItem("Tools/Animator Controller Creator")]
    public static void ShowWindow()
    {
        GetWindow<AnimatorControllerCreator>("Animator Controller Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Animator Controller and Add Animation", EditorStyles.boldLabel);
        if (GUILayout.Button("Create and Add Animation"))
        {
            CreateAnimatorControllerAndAddAnimation();
        }
    }

    private void CreateAnimatorControllerAndAddAnimation()
    {
        // 获取当前选中的目录
        string selectedPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(selectedPath) || !Directory.Exists(selectedPath))
        {
            selectedPath = "Assets"; // 默认使用Assets根目录
        }

        // 确保路径以/结尾
        if (!selectedPath.EndsWith("/"))
        {
            selectedPath += "/";
        }

        // 获取目录名称（不含路径）
        string directoryName = Path.GetFileName(Path.GetDirectoryName(selectedPath));
        if (string.IsNullOrEmpty(directoryName))
        {
            directoryName = "Default"; // 如果无法获取目录名，使用默认值
        }

        // 创建Controller目录（如果不存在）
        string controllerDir = selectedPath + "Controller/";
        if (!Directory.Exists(controllerDir))
        {
            Directory.CreateDirectory(controllerDir);
            AssetDatabase.Refresh();
        }

        // 创建Animation目录（如果不存在）
        string animationDir = selectedPath + "Animation/";
        if (!Directory.Exists(animationDir))
        {
            Directory.CreateDirectory(animationDir);
            AssetDatabase.Refresh();
        }

        // 在Animation目录中查找第一个.fbx文件
        string[] fbxFiles = Directory.GetFiles(animationDir, "*.fbx");
        if (fbxFiles.Length == 0)
        {
            Debug.LogError("No .fbx files found in the Animation directory: " + animationDir);
            return;
        }

        string firstFbxPath = fbxFiles[0];
        GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(firstFbxPath);
        if (fbxAsset == null)
        {
            Debug.LogError("Failed to load fbx asset: " + firstFbxPath);
            return;
        }

        // 获取fbx中的动画剪辑
        AnimationClip[] animationClips = AssetDatabase.LoadAllAssetsAtPath(firstFbxPath)
            .OfType<AnimationClip>()
            .ToArray();

        if (animationClips.Length == 0)
        {
            Debug.LogError("No animation clips found in the fbx file: " + firstFbxPath);
            return;
        }

        // 使用第一个动画剪辑
        AnimationClip firstAnimationClip = animationClips[0];

        // 创建AnimatorController，使用目录名作为前缀
        string animatorControllerName = directoryName + "AnimatorController.controller";
        string animatorControllerPath = controllerDir + animatorControllerName;
        AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath);
        if (animatorController == null)
        {
            Debug.LogError("Failed to create AnimatorController at: " + animatorControllerPath);
            return;
        }

        // 获取AnimatorController的根状态机
        AnimatorStateMachine rootStateMachine = animatorController.layers[0].stateMachine;

        // 检查是否存在名为"Idle"的状态
        ChildAnimatorState idleState = rootStateMachine.states.FirstOrDefault(s => s.state.name == "Idle");
        AnimatorState state;

        if (idleState.state == null)
        {
            // 如果不存在，创建新的Idle状态
            state = rootStateMachine.AddState("Idle");
        }
        else
        {
            // 如果存在，使用现有的Idle状态
            state = idleState.state;
        }

        // 设置动画剪辑到状态
        state.motion = firstAnimationClip;

        // 设置默认状态为Idle状态
        rootStateMachine.defaultState = state;

        // 保存所有更改
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 现在将创建的AnimatorController应用到Prefab子目录中的第一个GameObject上
        string prefabDir = selectedPath + "Prefab/";
        ApplyAnimatorControllerToPrefab(prefabDir, animatorControllerPath);

        Debug.Log($"Successfully created AnimatorController '{animatorControllerName}' and added animation to Idle state.");
    }

    private void ApplyAnimatorControllerToPrefab(string prefabDir, string animatorControllerPath)
    {
        // 检查Prefab目录是否存在
        if (!Directory.Exists(prefabDir))
        {
            Debug.LogError($"Prefab directory does not exist: {prefabDir}");
            return;
        }

        // 在Prefab目录中查找第一个Prefab文件
        string[] prefabFiles = Directory.GetFiles(prefabDir, "*.prefab");
        if (prefabFiles.Length == 0)
        {
            Debug.LogError($"No prefab files found in the Prefab directory: {prefabDir}");
            return;
        }

        string firstPrefabPath = prefabFiles[0];
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(firstPrefabPath);
        if (prefabAsset == null)
        {
            Debug.LogError($"Failed to load prefab asset: {firstPrefabPath}");
            return;
        }

        // 实例化Prefab以进行修改
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
        if (prefabInstance == null)
        {
            Debug.LogError($"Failed to instantiate prefab: {firstPrefabPath}");
            return;
        }

        // 检查Prefab实例是否有Animator组件
        Animator animator = prefabInstance.GetComponent<Animator>();
        if (animator == null)
        {
            // 如果没有，添加Animator组件
            animator = prefabInstance.AddComponent<Animator>();
            Debug.Log($"Added Animator component to prefab instance: {firstPrefabPath}");
        }

        // 设置Animator的Controller
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(animatorControllerPath);
        Debug.Log($"Applied AnimatorController to prefab instance: {firstPrefabPath}");

        // 保存修改后的Prefab
        PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.UserAction);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 销毁实例化的对象
        Object.DestroyImmediate(prefabInstance);

        Debug.Log($"Successfully applied AnimatorController to prefab: {firstPrefabPath}");
    }
}