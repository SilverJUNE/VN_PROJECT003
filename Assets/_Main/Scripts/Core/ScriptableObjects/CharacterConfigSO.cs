using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// CharacterConfigSO Ŭ������ ���� ���� ��ũ��Ʈ ��ü�Դϴ�.
    /// </summary>
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName ="Dialogue System/Character Configuration Asset")]
    public class CharacterConfigSO : ScriptableObject
    {
        // �� ĳ������ ���� �����͸� �����ϴ� �迭�Դϴ�.
        public CharacterConfigData[] characters;

        /// <summary>
        /// GetConfig �޼ҵ�� �־��� ĳ���� �̸��� �ش��ϴ� ���� �����͸� ã�� ��ȯ�մϴ�.
        /// ã�� ���ϸ� �⺻ ���� �����͸� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="characterName">ã���� �ϴ� ĳ������ �̸��Դϴ�.</param>
        /// <returns>ã�� ���� ������ �Ǵ� �⺻ ���� �������Դϴ�.</returns>
        public CharacterConfigData GetConfig(string characterName)
        {
            // ��ҹ��ڸ� �������� �ʵ��� �̸��� �ҹ��ڷ� ��ȯ�մϴ�.
            characterName = characterName.ToLower();

            // ��� ĳ���� ���� �����͸� ��ȸ�ϸ� ã���ϴ�.
            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                // ��ҹ��ڸ� �������� �ʵ��� �̸��� ������ ���մϴ�.
                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                    // ã�� ��� �ش� ���� �������� ���纻�� ��ȯ�մϴ�.    
                    return data.Copy();
            }

            // ã�� ���� ��� �⺻ ���� �����͸� ��ȯ�մϴ�.
            return CharacterConfigData.Default;
        }
    }
}