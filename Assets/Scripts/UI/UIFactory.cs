using UnityEngine;
using UnityEngine.UI;

namespace AroundTheCorner
{
    public static class UIFactory
    {
        public static RectTransform CreateUIObject(string name, Transform parent)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            if (parent != null)
            {
                rectTransform.SetParent(parent, false);
            }

            return rectTransform;
        }

        public static Image CreatePanel(string name, Transform parent, Color color)
        {
            RectTransform rectTransform = CreateUIObject(name, parent);
            Image image = rectTransform.gameObject.AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Text CreateText(
            string name,
            Transform parent,
            Font font,
            string content,
            int fontSize,
            TextAnchor alignment,
            Color color,
            FontStyle fontStyle)
        {
            RectTransform rectTransform = CreateUIObject(name, parent);
            Text text = rectTransform.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.fontStyle = fontStyle;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        public static Button CreateButton(
            string name,
            Transform parent,
            Font font,
            string label,
            int fontSize,
            Color backgroundColor,
            Color textColor)
        {
            Image image = CreatePanel(name, parent, backgroundColor);
            Button button = image.gameObject.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = backgroundColor;
            colors.highlightedColor = backgroundColor * 1.08f;
            colors.pressedColor = backgroundColor * 0.90f;
            colors.selectedColor = backgroundColor * 1.05f;
            colors.disabledColor = new Color(backgroundColor.r * 0.55f, backgroundColor.g * 0.55f, backgroundColor.b * 0.55f, 0.75f);
            button.colors = colors;

            Text buttonLabel = CreateText("Label", image.transform, font, label, fontSize, TextAnchor.MiddleCenter, textColor, FontStyle.Bold);
            Stretch(buttonLabel.rectTransform, 0f, 0f, 0f, 0f);
            return button;
        }

        public static VerticalLayoutGroup AddVerticalLayout(
            GameObject target,
            int spacing,
            RectOffset padding,
            TextAnchor alignment,
            bool expandWidth,
            bool expandHeight)
        {
            VerticalLayoutGroup group = target.AddComponent<VerticalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childAlignment = alignment;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = expandWidth;
            group.childForceExpandHeight = expandHeight;
            return group;
        }

        public static HorizontalLayoutGroup AddHorizontalLayout(
            GameObject target,
            int spacing,
            RectOffset padding,
            TextAnchor alignment,
            bool expandWidth,
            bool expandHeight)
        {
            HorizontalLayoutGroup group = target.AddComponent<HorizontalLayoutGroup>();
            group.spacing = spacing;
            group.padding = padding;
            group.childAlignment = alignment;
            group.childControlWidth = true;
            group.childControlHeight = true;
            group.childForceExpandWidth = expandWidth;
            group.childForceExpandHeight = expandHeight;
            return group;
        }

        public static GridLayoutGroup AddGridLayout(GameObject target, Vector2 cellSize, Vector2 spacing, RectOffset padding)
        {
            GridLayoutGroup group = target.AddComponent<GridLayoutGroup>();
            group.cellSize = cellSize;
            group.spacing = spacing;
            group.padding = padding;
            group.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            return group;
        }

        public static LayoutElement AddLayoutElement(GameObject target, float minHeight, float preferredHeight, float flexibleHeight, float minWidth, float preferredWidth, float flexibleWidth)
        {
            LayoutElement layoutElement = target.AddComponent<LayoutElement>();
            layoutElement.minHeight = minHeight;
            layoutElement.preferredHeight = preferredHeight;
            layoutElement.flexibleHeight = flexibleHeight;
            layoutElement.minWidth = minWidth;
            layoutElement.preferredWidth = preferredWidth;
            layoutElement.flexibleWidth = flexibleWidth;
            return layoutElement;
        }

        public static void Stretch(RectTransform rectTransform, float left, float bottom, float right, float top)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(-right, -top);
        }

        public static void SetAnchors(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.offsetMin = offsetMin;
            rectTransform.offsetMax = offsetMax;
        }
    }
}
