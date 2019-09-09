using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// Textを範囲内に収まるように省略して表示する
/// 省略表示させたいTextと一緒にアタッチする
/// </summary>
public class TextEllipsis : UIBehaviour
{
    private Text m_Text;

    protected override void Awake()
    {
        if (m_Text != null)
        {
            return;
        }

        m_Text = GetComponent<Text>();
        m_Text.RegisterDirtyLayoutCallback(DoEllipsis);
    }

    protected override void OnDestroy()
    {
        m_Text.UnregisterDirtyLayoutCallback(DoEllipsis);
    }


    /// <summary>
    /// 省略記号
    /// </summary>
    private static readonly string ELLIPSIS = "...";

    private void DoEllipsis()
    {
        if (!IsActive() || m_Text == null)
        {
            return;
        }

        if (!NeedsEllipsis(m_Text))
        {
            return;
        }

        TextGenerator generator = m_Text.cachedTextGenerator;
        TextGenerationSettings settings = m_Text.GetGenerationSettings(m_Text.rectTransform.rect.size);
        generator.Populate(m_Text.text, settings);
        string result = string.Empty;
        for (int i = 0; i < generator.characterCount; ++i)
        {
            string current = m_Text.text.Substring(i, 1);
            string next = string.Empty;
            if (i + 1 <= generator.characterCount)
            {
                next = m_Text.text.Substring(i + 1, 1);
            }

            var preferredWidth = GetPreferredWidth(result + current + next);
            if (IsOverSize(m_Text.rectTransform.rect.size.x, preferredWidth))
            {
                result += ELLIPSIS;
                break;
            }

            result += current;
        }

        m_Text.text = result;
    }

    private bool NeedsEllipsis(Text text)
    {
        return IsOverSize(text.rectTransform.rect.size.x, text.preferredWidth);
    }

    private bool IsOverSize(float textBoxWidth, float preferredWidth)
    {
        return textBoxWidth < preferredWidth;
    }

    private float GetPreferredWidth(string str)
    {
        var settings = m_Text.GetGenerationSettings(Vector2.zero);
        return m_Text.cachedTextGeneratorForLayout.GetPreferredWidth(str, settings) / m_Text.pixelsPerUnit;
    }
}
