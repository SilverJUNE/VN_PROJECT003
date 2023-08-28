using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    /// <summary>
    /// NameContainer는 대화 상자에 표시되는 이름 텍스트를 관리하는 클래스입니다. DialogueContainer의 일부입니다.
    /// </summary>
    public class NameContainer
    {
        // 이름 텍스트를 포함하는 게임 오브젝트를 참조합니다.
        [SerializeField] private GameObject root;
        // 이름을 표시하는 TextMeshProUGUI 컴포넌트를 참조합니다.
        [SerializeField] private TextMeshProUGUI nameText;

        /// <summary>
        /// 이름을 표시하는 메서드입니다. 표시하려는 이름을 문자열로 전달할 수 있습니다.
        /// </summary>
        /// <param name="nameToShow">화면에 표시하려는 이름입니다. 기본값은 빈 문자열입니다.</param>
        public void Show(string nameToShow = "")
        {
            // 이름을 포함하는 게임 오브젝트를 활성화합니다.    
            root.SetActive(true);

            // 이름이 빈 문자열이 아니면, 이름 텍스트를 설정합니다.
            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        /// <summary>
        /// 이름을 숨기는 메서드입니다.
        /// </summary>
        public void Hide()
        {
            // 이름을 포함하는 게임 오브젝트를 비활성화합니다.
            root.SetActive(false);
        }

        /// <summary>
        /// 이름 텍스트의 색상을 설정합니다.
        /// </summary>
        /// <param name="color">설정할 색상입니다.</param>
        public void SetNameColor(Color color) => nameText.color = color;

        /// <summary>
        /// 이름 텍스트의 폰트를 설정합니다.
        /// </summary>
        /// <param name="font">설정할 폰트입니다.</param>
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;

        public void SetNameFontSize(float size) => nameText.fontSize = size;

    }

}

