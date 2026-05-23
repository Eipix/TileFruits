#if UNITY_EDITOR

namespace Editor
{
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;

    [InitializeOnLoad]
    public static class GlobalFastPlay
    {
        static GlobalFastPlay()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);

            if (toolbars.Length == 0) return;

            var currentToolbar = toolbars[0];

            var rootProperty = currentToolbar.GetType()
                .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            var root = rootProperty?.GetValue(currentToolbar) as VisualElement;

            if (root == null) return;

            var toolbarZone = root.Q("ToolbarZonePlayMode");

            if (toolbarZone != null)
            {
                if (toolbarZone.Q("FastPlayButton") == null)
                {
                    var button = CreateFastPlayButton();
                    toolbarZone.Insert(0, button);
                }

                EditorApplication.update -= OnUpdate;
            }
        }

        private static Button CreateFastPlayButton()
        {
            var button = new Button(OnClick)
            {
                name = "FastPlayButton",
                tooltip = "Fast Play (No Domain Reload)",
            };

            button.style.width = 30;
            button.style.height = 20;
            button.style.marginTop = 1;
            button.style.marginLeft = 4;
            button.style.marginRight = 4;

            button.style.paddingLeft = 0;
            button.style.paddingRight = 0;
            button.style.paddingTop = 0;
            button.style.paddingBottom = 0;
            button.style.backgroundColor = new StyleColor(new Color(0.3f, 0.3f, 0.3f, 1f));
            button.style.width = 0;

            button.AddToClassList("unity-toolbar-button");
            button.AddToClassList("unity-editor-toolbar__button");

            var iconImage = new Image();
            iconImage.image = EditorGUIUtility.IconContent("d_SpeedScale").image;

            iconImage.style.width = 16;
            iconImage.style.height = 16;

            iconImage.style.alignSelf = Align.Center;
            iconImage.pickingMode = PickingMode.Ignore;

            button.Add(iconImage);

            return button;
        }

        private static void OnClick()
        {
            FastPlayLogic.EnterPlayModeFast();
        }
    }
}

#endif
