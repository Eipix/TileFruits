#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;

[InitializeOnLoad]
public static class FastPlayLogic
{
    private const string KEY_ACTIVE = "FastPlay_IsActive";
    private const string KEY_OPTIONS_ENABLED = "FastPlay_WasOptionsEnabled";
    private const string KEY_ORIGINAL_OPTIONS = "FastPlay_OriginalOptions";

    static FastPlayLogic()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public static void EnterPlayModeFast()
    {
        if (EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        SessionState.SetBool(KEY_OPTIONS_ENABLED, EditorSettings.enterPlayModeOptionsEnabled);
        SessionState.SetInt(KEY_ORIGINAL_OPTIONS, (int)EditorSettings.enterPlayModeOptions);
        SessionState.SetBool(KEY_ACTIVE, true);

        EditorSettings.enterPlayModeOptionsEnabled = true;
        EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;

        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state is PlayModeStateChange.EnteredEditMode)
        {
            if (SessionState.GetBool(KEY_ACTIVE, false))
            {
                EditorSettings.enterPlayModeOptionsEnabled = SessionState.GetBool(KEY_OPTIONS_ENABLED, false);
                EditorSettings.enterPlayModeOptions = (EnterPlayModeOptions)SessionState.GetInt(KEY_ORIGINAL_OPTIONS, 0);

                SessionState.SetBool(KEY_ACTIVE, false);
            }
        }
    }
}

[EditorToolbarElement(ID, typeof(SceneView))]
public class FastPlayButton : EditorToolbarButton
{
    public const string ID = "FastPlay/Button";

    public FastPlayButton()
    {
        this.icon = EditorGUIUtility.IconContent("d_SpeedScale").image as Texture2D;
        this.tooltip = "Fast Play (No Domain Reload)";
        this.text = "Fast Play";

        this.clicked += OnClick;
    }

    private void OnClick()
    {
        FastPlayLogic.EnterPlayModeFast();
    }
}

[Overlay(typeof(SceneView), "Fast Play Toolbar", true)]
public class FastPlayToolbarOverlay : ToolbarOverlay
{
    FastPlayToolbarOverlay() : base(FastPlayButton.ID) { }
}

#endif
