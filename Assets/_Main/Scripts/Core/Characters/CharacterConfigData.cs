using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// CharacterConfigData Ŭ������ ĳ���� ���� ������ �����մϴ�.
    /// </summary>
    [System.Serializable]
    public class CharacterConfigData
    {
        /// <summary>
        /// ĳ������ �̸��Դϴ�.
        /// </summary>
        public string name;
        /// <summary>
        /// ĳ������ �����Դϴ�.
        /// </summary>
        public string alias;
        /// <summary>
        /// ĳ������ Ÿ���Դϴ�.
        /// </summary>
        public Character.CharacterType characterType;


        /// <summary>
        /// �̸��� �����Դϴ�.
        /// </summary>
        public Color nameColor;
        /// <summary>
        /// ��ȭ�� �����Դϴ�.
        /// </summary>
        public Color dialogueColor;

        /// <summary>
        /// �̸��� ��Ʈ�Դϴ�.
        /// </summary>
        public TMP_FontAsset nameFont;
        /// <summary>
        /// ��ȭ�� ��Ʈ�Դϴ�.
        /// </summary>
        public TMP_FontAsset dialogueFont;

        public float nameFontScale = 1f;
        public float dialogueFontScale = 1f;

        /// <summary>
        /// �� �޼ҵ�� ���� CharacterConfigData ��ü�� ���纻�� �����մϴ�.
        /// </summary>
        /// <returns>����� CharacterConfigData ��ü�Դϴ�.</returns>
        public CharacterConfigData Copy()
        {
            CharacterConfigData result = new CharacterConfigData();

            result.name = name;
            result.alias = alias;

            result.characterType = characterType;
            result.nameFont = nameFont;
            result.dialogueFont = dialogueFont;

            // Color�� ������ ������ Color�� �� ��Ҹ� �����ؾ� �մϴ�.
            result.nameColor        = new Color(nameColor.r, nameColor.g, nameColor.b, nameColor.a);
            result.dialogueColor    = new Color(dialogueColor.r, dialogueColor.g, dialogueColor.b, dialogueColor.a);

            result.dialogueFontScale = dialogueFontScale;
            result.nameFontScale = nameFontScale;

            return result;
        }

        // �⺻ ����� ��Ʈ�� �����ɴϴ�.
        private static Color defaultColor        => DialogueSystem.instance.config.defaultTextColor;
        private static TMP_FontAsset defaultFont => DialogueSystem.instance.config.defaultFont;
        
        /// <summary>
        /// Default ������Ƽ�� �⺻ CharacterConfigData ��ü�� ��ȯ�մϴ�.
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