using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 'Character_Text' Ŭ������ �ؽ�Ʈ ��� ĳ���͸� �����ϴ� Ŭ�����Դϴ�.
    /// �� Ŭ������ 'Character' Ŭ������ ��ӹ޽��ϴ�.
    /// </summary>
    public class Character_Text : Character
    {
        /// <summary>
        /// 'Character_Text' Ŭ������ �������Դϴ�.
        /// �� �����ڴ� ���� Ŭ���� 'Character'�� �����ڸ� ȣ���Ͽ� ĳ������ �̸��� �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="name">ĳ������ �̸��Դϴ�.</param>
        public Character_Text(string name, CharacterConfigData config) : base(name, config, prefab: null)  // 'base(name)'�� ���� Ŭ������ �����ڸ� ȣ���ϴ� ���� �ǹ��մϴ�.?
        {
            Debug.Log($"Created Text Character: '{name}'");
        }


    }
}