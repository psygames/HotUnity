using System;
using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    public class AddScriptWindow : EditorWindow
    {
        static AddScriptWindow _instance;
        static Styles _styles;
        Action<string> AddScriptDelegate;
        Func<string, string[]> FilerScriptDelegate;
        Vector2 _scrollPosition;
        string _searchString = string.Empty;

        public static void Show(Rect rect, Action<string> onAddScript, Func<string, string[]> onFilerScripts)
        {
            if (_instance == null)
            {
                _instance = CreateInstance<AddScriptWindow>();
            }

            _instance.Init(rect, onAddScript, onFilerScripts);
            _instance.Repaint();
        }

        private void Init(Rect rect, Action<string> onAddScript, Func<string, string[]> onFilerScripts)
        {
            var v2 = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            rect.x = v2.x;
            rect.y = v2.y;
            ShowAsDropDown(rect, new Vector2(rect.width, 320f));
            Focus();
            wantsMouseMove = true;
            AddScriptDelegate = onAddScript;
            FilerScriptDelegate = onFilerScripts;
        }

        void OnGUI()
        {
            _styles = _styles ?? new Styles();
            SearchGUI();
            ListGUI();
        }

        void SearchGUI()
        {
            GUI.Label(new Rect(0.0f, 0.0f, this.position.width, this.position.height), GUIContent.none, _styles.background);
            GUILayout.Space(7);
            GUILayout.BeginHorizontal();
            _searchString = GUILayout.TextField(_searchString, GUI.skin.FindStyle("SearchTextField"));
            var buttonStyle = _searchString == string.Empty ? GUI.skin.FindStyle("SearchCancelButtonEmpty") : GUI.skin.FindStyle("SearchCancelButton");
            if (GUILayout.Button(string.Empty, buttonStyle))
            {
                _searchString = string.Empty;
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
        }

        void ListGUI()
        {
            var rect = position;
            rect.x = 1;
            rect.y = 30;
            rect.height -= 30;
            rect.width -= 2;
            GUILayout.BeginArea(rect);
            rect = GUILayoutUtility.GetRect(10f, 25f);
            GUI.Label(rect, _searchString == string.Empty ? "HotScript" : "Search", _styles.header);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            var searchString = _searchString.ToLower();
            var scripts = FilerScriptDelegate(searchString);
            foreach (var script in scripts)
            {
                var buttonRect = GUILayoutUtility.GetRect(16f, 20f, GUILayout.ExpandWidth(true));
                buttonRect.xMin = -2;
                buttonRect.xMax += 2;
                if (GUI.Button(buttonRect, "", _styles.componentButton))
                {
                    AddScriptDelegate(script);
                    Close();
                }

                buttonRect.xMin = 18;
                GUI.Label(buttonRect, script, _styles.componentButton);

                buttonRect.height = 16;
                buttonRect.width = 16;
                buttonRect.x = 2;
                buttonRect.y += 2;
                GUI.DrawTexture(buttonRect, Helper.scriptIcon);
            }
            GUILayoutUtility.GetRect(0, 10);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private class Styles
        {
            public GUIStyle header = new GUIStyle((GUIStyle)"In BigTitle");
            public GUIStyle componentButton = new GUIStyle((GUIStyle)"PR Label");
            public GUIStyle background = (GUIStyle)"grey_border";
            public GUIStyle previewBackground = (GUIStyle)"PopupCurveSwatchBackground";
            public GUIStyle previewHeader = new GUIStyle(EditorStyles.label);
            public GUIStyle previewText = new GUIStyle(EditorStyles.wordWrappedLabel);
            public GUIStyle rightArrow = (GUIStyle)"AC RightArrow";
            public GUIStyle leftArrow = (GUIStyle)"AC LeftArrow";
            public GUIStyle groupButton;

            public Styles()
            {
                this.header.font = EditorStyles.boldLabel.font;
                this.componentButton.alignment = TextAnchor.MiddleLeft;
                this.componentButton.padding.left -= 15;
                this.componentButton.fixedHeight = 20f;
                this.groupButton = new GUIStyle(this.componentButton);
                this.groupButton.padding.left += 17;
                this.previewText.padding.left += 3;
                this.previewText.padding.right += 3;
                ++this.previewHeader.padding.left;
                this.previewHeader.padding.right += 3;
                this.previewHeader.padding.top += 3;
                this.previewHeader.padding.bottom += 2;
            }
        }
    }
}
