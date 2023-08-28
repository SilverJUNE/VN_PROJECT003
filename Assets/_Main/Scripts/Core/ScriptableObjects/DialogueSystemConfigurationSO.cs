using CHARACTERS;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DialogueSystemConfigurationSO 클래스는 대화 시스템의 설정을 저장하는 스크립트 개체입니다.
    /// </summary>
    [CreateAssetMenu(fileName ="Dialogue System Configuration", menuName ="Dialogue System/Dialogue Configuration Asset")]
    public class DialogueSystemConfigurationSO : ScriptableObject
    {
        public const float DEFAULT_FONTSIZE_NAME = 80f;
        public const float DEFAULT_FONTSIZE_DIALOGUE = 72f;

        // 캐릭터 설정 스크립트 개체에 대한 참조입니다.
        public CharacterConfigSO characterConfigurationAsset;

        // 대화 텍스트의 기본 색상입니다. 기본값은 흰색입니다.
        public Color defaultTextColor = Color.white;

        // 대화 텍스트의 기본 폰트입니다.
        public TMP_FontAsset defaultFont;

        public float dialogueFontScale = 1f;
        public float defaultNameFontSize = DEFAULT_FONTSIZE_NAME;
        public float defaultDialogueFontSize = DEFAULT_FONTSIZE_DIALOGUE;

    }
}