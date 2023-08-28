using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    /// <summary>
    /// NameContainer�� ��ȭ ���ڿ� ǥ�õǴ� �̸� �ؽ�Ʈ�� �����ϴ� Ŭ�����Դϴ�. DialogueContainer�� �Ϻ��Դϴ�.
    /// </summary>
    public class NameContainer
    {
        // �̸� �ؽ�Ʈ�� �����ϴ� ���� ������Ʈ�� �����մϴ�.
        [SerializeField] private GameObject root;
        // �̸��� ǥ���ϴ� TextMeshProUGUI ������Ʈ�� �����մϴ�.
        [SerializeField] private TextMeshProUGUI nameText;

        /// <summary>
        /// �̸��� ǥ���ϴ� �޼����Դϴ�. ǥ���Ϸ��� �̸��� ���ڿ��� ������ �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="nameToShow">ȭ�鿡 ǥ���Ϸ��� �̸��Դϴ�. �⺻���� �� ���ڿ��Դϴ�.</param>
        public void Show(string nameToShow = "")
        {
            // �̸��� �����ϴ� ���� ������Ʈ�� Ȱ��ȭ�մϴ�.    
            root.SetActive(true);

            // �̸��� �� ���ڿ��� �ƴϸ�, �̸� �ؽ�Ʈ�� �����մϴ�.
            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        /// <summary>
        /// �̸��� ����� �޼����Դϴ�.
        /// </summary>
        public void Hide()
        {
            // �̸��� �����ϴ� ���� ������Ʈ�� ��Ȱ��ȭ�մϴ�.
            root.SetActive(false);
        }

        /// <summary>
        /// �̸� �ؽ�Ʈ�� ������ �����մϴ�.
        /// </summary>
        /// <param name="color">������ �����Դϴ�.</param>
        public void SetNameColor(Color color) => nameText.color = color;

        /// <summary>
        /// �̸� �ؽ�Ʈ�� ��Ʈ�� �����մϴ�.
        /// </summary>
        /// <param name="font">������ ��Ʈ�Դϴ�.</param>
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;

        public void SetNameFontSize(float size) => nameText.fontSize = size;

    }

}

