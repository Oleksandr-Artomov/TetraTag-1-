#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ControllerSetupEditorWindow : EditorWindow
{
    private static string GenerateKey => Application.productName + "_setup_shown";

    [InitializeOnLoadMethod]
    private static void OnInitialize()
    {
        if (EditorPrefs.GetBool(GenerateKey, false) || HasOpenInstances<ControllerSetupEditorWindow>()) return;
        ShowWindow();
    }

    public void CreateGUI()
    {
        var label = new Label(
            "Hello,\n\nThank you for your support. I hope you find the controller both useful and enjoyable.\n\nUpon initial import, you may encounter errors, most likely due to missing dependencies. To maximize the controller's capabilities, I strongly recommend installing the new <b>Input System</b>. While the legacy input system is supported, for future-proofing your game, the new input system is highly advisable.\n\nIf you're interested in exploring the demo scene, you'll also need to install the <b>2D Tilemap Editor</b> package.\n\nTo install these packages, navigate to Window -> Package Manager and install both '<b>Input System</b>' and '<b>2D Tilemap Editor</b>'.\n\nIf the demo scene is not of interest to you, feel free to delete the _Demo folder.\n\n<b>Usage:</b> \nTo get started, simply drag and drop the prefab into your scene and hit the 'Play' button. The controller is designed to work right out of the box. If you wish to modify player stats, you can do so via the 'Player Stats' scriptable object. You also have the option to create alternate versions of the scriptable object for quick stat swapping.\n\nFor direct support, the best way to reach me is through the Patreon-only Discord channel: https://discord.gg/tarodev\n\nhttps://www.patreon.com/tarodev\n\nBest regards,\nTarodev");
        label.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);
        rootVisualElement.Add(label);

        rootVisualElement.style.paddingLeft = 10;
        rootVisualElement.style.paddingRight = 10;
        rootVisualElement.style.paddingTop = 10;
        rootVisualElement.style.paddingBottom = 10;
    }

    [MenuItem("Tarodev Controller/Setup Window")]
    public static void ShowMyEditor() => ShowWindow();

    private static void ShowWindow()
    {
        Debug.Log(  Application.productName);
        EditorWindow wnd = GetWindow<ControllerSetupEditorWindow>();
        wnd.titleContent = new GUIContent("Tarodev Controller Setup");

        wnd.maxSize = new Vector2(500, 500);
        wnd.minSize = new Vector2(500, 500);

        EditorPrefs.SetBool(GenerateKey, true);
    }
}

#endif