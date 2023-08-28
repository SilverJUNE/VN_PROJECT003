using UnityEngine;
using TMPro;
using System.Collections;

namespace DIALOGUE
{

    /// <summary>
    /// DialogueContainer 클래스는 대화 시스템에서 대화 및 이름 표시를 관리합니다.
    /// </summary>
    [System.Serializable]
    public class DialogueContainer
    {
        /// <summary>
        /// 대화 표시를 위한 UI 요소를 포함하는 게임 오브젝트를 참조합니다.
        /// </summary>
        public GameObject root;

        /// <summary>
        /// 캐릭터 이름을 표시하기 위한 NameContainer 객체를 참조합니다.
        /// </summary>
        public NameContainer nameContainer;

        /// <summary>
        /// 대화 텍스트를 표시하는 TextMeshProUGUI 컴포넌트를 참조합니다.
        /// </summary>
        public TextMeshProUGUI dialogueText;

        private CanvasGroupController cgController;


        /// <summary>
        /// 대화 텍스트의 색상을 설정합니다.
        /// </summary>
        /// <param name="color">적용할 색상입니다.</param>
        public void SetDialogueColor(Color color) => dialogueText.color = color;

        /// <summary>
        /// 대화 텍스트의 폰트를 설정합니다.
        /// </summary>
        /// <param name="font">적용할 폰트입니다.</param>
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;

        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        private bool initialized = false;
        public void Initialize()
        {
            if(initialized) 
               return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate   );
    }
}

