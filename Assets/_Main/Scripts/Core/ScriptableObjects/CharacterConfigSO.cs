using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// CharacterConfigSO 클래스는 문자 설정 스크립트 개체입니다.
    /// </summary>
    [CreateAssetMenu(fileName = "Character Configuration Asset", menuName ="Dialogue System/Character Configuration Asset")]
    public class CharacterConfigSO : ScriptableObject
    {
        // 각 캐릭터의 설정 데이터를 저장하는 배열입니다.
        public CharacterConfigData[] characters;

        /// <summary>
        /// GetConfig 메소드는 주어진 캐릭터 이름에 해당하는 설정 데이터를 찾아 반환합니다.
        /// 찾지 못하면 기본 설정 데이터를 반환합니다.
        /// </summary>
        /// <param name="characterName">찾고자 하는 캐릭터의 이름입니다.</param>
        /// <returns>찾은 설정 데이터 또는 기본 설정 데이터입니다.</returns>
        public CharacterConfigData GetConfig(string characterName)
        {
            // 대소문자를 구분하지 않도록 이름을 소문자로 변환합니다.
            characterName = characterName.ToLower();

            // 모든 캐릭터 설정 데이터를 순회하며 찾습니다.
            for (int i = 0; i < characters.Length; i++)
            {
                CharacterConfigData data = characters[i];

                // 대소문자를 구분하지 않도록 이름과 별명을 비교합니다.
                if (string.Equals(characterName, data.name.ToLower()) || string.Equals(characterName, data.alias.ToLower()))
                    // 찾은 경우 해당 설정 데이터의 복사본을 반환합니다.    
                    return data.Copy();
            }

            // 찾지 못한 경우 기본 설정 데이터를 반환합니다.
            return CharacterConfigData.Default;
        }
    }
}