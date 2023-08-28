using CHARACTERS;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DialogueSystemConfigurationSO Ŭ������ ��ȭ �ý����� ������ �����ϴ� ��ũ��Ʈ ��ü�Դϴ�.
    /// </summary>
    [CreateAssetMenu(fileName ="Dialogue System Configuration", menuName ="Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        public const float DEFAULT_FONTSIZE_NAME = 80f;
        public const float DEFAULT_FONTSIZE_DIALOGUE = 72f;

        // ĳ���� ���� ��ũ��Ʈ ��ü�� ���� �����Դϴ�.
        public CharacterConfigSO characterConfigurationAsset;

        // ��ȭ �ؽ�Ʈ�� �⺻ �����Դϴ�. �⺻���� ����Դϴ�.
        public Color defaultTextColor = Color.white;

        // ��ȭ �ؽ�Ʈ�� �⺻ ��Ʈ�Դϴ�.
        public TMP_FontAsset defaultFont;

        public float dialogueFontScale = 1f;
        public float defaultNameFontSize = DEFAULT_FONTSIZE_NAME;
        public float defaultDialogueFontSize = DEFAULT_FONTSIZE_DIALOGUE;

    }
}