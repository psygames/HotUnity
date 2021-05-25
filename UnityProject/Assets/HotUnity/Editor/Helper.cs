using System;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HotUnity.Editor
{
    public static class Helper
    {
        private const string ICON_FIRE_COLORFUL = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAABkUlEQVQ4T4WTzyulURjHP48bSspCkgWpWVrYsJrNkDi3zE2KSIpoMslSyYbZzMZWSDQrzURSQ/eeu5mh7FjY2LAgm/kf5nIfvcf7ct7XvTy78/z4nOd8n+cIZUwNn8RyXC4e+eUtALAFzIvloFzee4C/YeGwWPZKQd4C/AAmnouENooMSJ7vPigGUEOrWG61jy6EP4kbl1HOgXYfkgQE711RwynwMQG4BM5cVxV8lixHQfwZoGl6UZaBE2DBKw6KOsPzf6AKOBRLJglYR5kpIVQPwg5KI1AAKl1O2IXrQNN8QLkAaj3AJpARS5MahoBd4AFIuRxhVXLMPQGMa30pdnuRXiroEstimLMBfPGebcWSjgA/gREPcEiBSaqplyxXDtBPC/dchxoErjWxzEaAfWDQAxix5JN6qOEOaA79o2L5FQGChQkWJ7BtsUyX2jo1bowdCBuS42t8Ci86fBPrNHllargJhBZLQxSML1If4whD0Yx9gqbppsiY5Jny/a/+gvZQR4oayfMvBjBupL+TbT0CO8x0EaaDrpAAAAAASUVORK5CYII=";
        private static Texture2D _icon;
        public static Texture2D scriptIcon
        {
            get
            {
                _icon = null;
                if (_icon == null)
                {
                    _icon = GetBase64Texture(ICON_FIRE_COLORFUL);
                }
                return _icon;
            }
        }

        private static Texture2D GetBase64Texture(string base64Str)
        {
            var texture = new Texture2D(0, 0);
            texture.LoadImage(Convert.FromBase64String(base64Str));
            return texture;
        }

        static Color proColor = new Color32(56, 56, 56, 255);
        static Color plebColor = new Color32(194, 194, 194, 255);

        public static Color backgroudColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? proColor : plebColor;
            }
        }

        static Color objProColor = new Color32(76, 76, 76, 255);
        static Color objPlebColor = new Color32(194, 194, 194, 255);

        public static Color objFieldColor
        {
            get
            {
                return EditorGUIUtility.isProSkin ? objProColor : objPlebColor;
            }
        }

        public static string ToTitle(string name)
        {
            var sb = new StringBuilder();
            sb.Append(char.ToUpper(name[0]));
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    sb.Append(' ');
                }
                sb.Append(name[i]);
            }
            return sb.ToString();
        }
    }
}
