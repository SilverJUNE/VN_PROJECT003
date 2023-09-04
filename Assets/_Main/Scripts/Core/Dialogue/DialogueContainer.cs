using UnityEngine;
using TMPro;
using System.Collections;
using System;

namespace DIALOGUE
{

    /// <summary>
    /// DialogueContainer Ŭ������ ��ȭ �ý��ۿ��� ��ȭ �� �̸� ǥ�ø� �����մϴ�.
    /// </summary>
    [System.Serializable]
    public class DialogueContainer
    {
        /// <summary>
        /// ��ȭ ǥ�ø� ���� UI ��Ҹ� �����ϴ� ���� ������Ʈ�� �����մϴ�.
        /// </summary>
        public GameObject root;

        /// <summary>
        /// ĳ���� �̸��� ǥ���ϱ� ���� NameContainer ��ü�� �����մϴ�.
        /// </summary>
        public NameContainer nameContainer;

        /// <summary>
        /// ��ȭ �ؽ�Ʈ�� ǥ���ϴ� TextMeshProUGUI ������Ʈ�� �����մϴ�.
        /// </summary>
        public TextMeshProUGUI dialogueText;

        private CanvasGroupController cgController;


        /// <summary>
        /// ��ȭ �ؽ�Ʈ�� ������ �����մϴ�.
        /// </summary>
        /// <param name="color">������ �����Դϴ�.</param>
        public void SetDialogueColor(Color color) => dialogueText.color = color;

        /// <summary>
        /// ��ȭ �ؽ�Ʈ�� ��Ʈ�� �����մϴ�.
        /// </summary>
        /// <param name="font">������ ��Ʈ�Դϴ�.</param>
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

        internal void HideDialogueBox()
        {
            throw new NotImplementedException();
        }

        internal void ShowDialogueBox()
        {
            throw new NotImplementedException();
        }

        public void ShowDialogueBox(float speed = 1f, bool immediate = false)
        {
            // Assuming cgController handles the visibility of the dialogue box
            cgController.Show(speed, immediate);
        }

        public void HideDialogueBox(float speed = 1f, bool immediate = false)
        {
            // Assuming cgController handles the visibility of the dialogue box
            cgController.Hide(speed, immediate);
        }


    }
}

