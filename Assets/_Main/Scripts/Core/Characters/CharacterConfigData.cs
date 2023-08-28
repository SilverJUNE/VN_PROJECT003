using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// CharacterConfigData 클래스는 캐릭터 설정 정보를 저장합니다.
    /// </summary>
    [System.Serializable]
    public class CharacterConfigData
    {
        /// <summary>
        /// 캐릭터의 이름입니다.
        /// </summary>
        public string name;
        /// <summary>
        /// 캐릭터의 가명입니다.
        /// </summary>
        public string alias;
        /// <summary>
        /// 캐릭터의 타입입니다.
        /// </summary>
        public Character.CharacterType characterType;


        /// <summary>
        /// 이름의 색상입니다.
        /// </summary>
        public Color nameColor;
        /// <summary>
        /// 대화의 색상입니다.
        /// </summary>
        public Color dialogueColor;

        /// <summary>
        /// 이름의 폰트입니다.
        /// </summary>
        public TMP_FontAsset nameFont;
        /// <summary>
        /// 대화의 폰트입니다.
        /// </summary>
        public TMP_FontAsset dialogueFont;

        public float nameFontScale = 1f;
        public float dialogueFontScale = 1f;

        /// <summary>
        /// 이 메소드는 현재 CharacterConfigData 객체의 복사본을 생성합니다.
        /// </summary>
        /// <returns>복사된 CharacterConfigData 객체입니다.</returns>
        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;

            result.characterType = characterType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            // Color를 복사할 때에는 Color의 각 요소를 복사해야 합니다.
            result.nameColor        = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor    = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.dialogueFontScale = dialogueFontScale;
            result.nameFontScale = nameFontScale;

            return result;
        }

        // 기본 색상과 폰트를 가져옵니다.
        private static Color defaultColor        => DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;
        
        /// <summary>
        /// Default 프로퍼티는 기본 CharacterConfigData 객체를 반환합니다.
        /// </summary>
        public static CharacterConfigData Default
        {
            get
            {
                CharacterConfigData result = new CharacterConfigData();

                result.name = "";
                result.alias = "";

                result.characterType = Character.CharacterType.Text;
                
                result.nameFont         = defaultFont;
                result.dialogueFont     = defaultFont;
                result.nameColor        = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);
                result.dialogueColor    = new Color(defaultColor.r, defaultColor.g, defaultColor.b, defaultColor.a);

                result.nameFontScale = 1f;      // DialogueSystem.instance.config.defaultNameFontSize;
                result.dialogueFontScale = 1f;  // DialogueSystem.instance.config.defaultDialogueFontSize;

                return result;
            }
        }
    }
}