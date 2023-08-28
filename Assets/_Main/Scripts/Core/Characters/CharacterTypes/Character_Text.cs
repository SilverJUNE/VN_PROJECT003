using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 'Character_Text' 클래스는 텍스트 기반 캐릭터를 정의하는 클래스입니다.
    /// 이 클래스는 'Character' 클래스를 상속받습니다.
    /// </summary>
    public class Character_Text : Character
    {
        /// <summary>
        /// 'Character_Text' 클래스의 생성자입니다.
        /// 이 생성자는 상위 클래스 'Character'의 생성자를 호출하여 캐릭터의 이름을 초기화합니다.
        /// </summary>
        /// <param name="name">캐릭터의 이름입니다.</param>
        public Character_Text(string name, CharacterConfigData config) : base(name, config, prefab: null)  // 'base(name)'는 상위 클래스의 생성자를 호출하는 것을 의미합니다.?
        {
            Debug.Log($"Created Text Character: '{name}'");
        }


    }
}