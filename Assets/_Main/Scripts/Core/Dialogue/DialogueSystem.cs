using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;

namespace DIALOGUE
{
    /// <summary>
    /// DialogueSystem 클래스는 게임 내 대화를 관리하고 표시하는 시스템을 구현합니다.
    /// </summary>
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfigurationSO _config;
        public  DialogueSystemConfigurationSO config => _config;

        public  DialogueContainer dialogueContainer   = new DialogueContainer();
        public ConversationManager conversationManager { get; private set; }
        private TextArchitect architect;
        private AutoReader autoReader;

        [SerializeField] private CanvasGroup mainCanvas;

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();

        public event DialogueSystemEvent onUserPrompt_Next;

        public bool isRunningConversation => conversationManager.isRunning;

        public DialogueContinuePrompt prompt;

        private CanvasGroupController cgController;

        /// <summary>
        /// Awake 함수에서는 DialogueSystem의 인스턴스가 이미 존재하는지 확인하고,
        /// 존재하지 않을 경우 현재 인스턴스를 정적 instance 변수에 할당합니다.
        /// 이미 다른 인스턴스가 존재할 경우, 현재 인스턴스는 즉시 파괴됩니다.
        /// </summary>
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }


        /// <summary>
        /// DialogueSystem을 초기화하는 메서드입니다.
        /// _initialized가 true라면 메서드는 아무것도 하지 않습니다.
        /// </summary>
        bool _initialized = false;

        private void Initialize()
        {
            if (_initialized)
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);
            dialogueContainer.Initialize();

            if (TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager);
        }

        /// <summary>
        /// 사용자가 다음 대화로 넘어가려고 시도하면 호출되는 메서드입니다.
        /// 이 메서드는 onUserPrompt_Next 이벤트를 발생시킵니다.
        /// </summary>
        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();

            if(autoReader != null && autoReader.isOn)
                autoReader.Disable();
        }

        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        /// <summary>
        /// 발화자의 이름을 통해 캐릭터의 구성 데이터를 가져와 대화 컨테이너에 적용합니다.
        /// </summary>
        /// <param name="speakerName">발화자의 이름입니다.</param>
        public void ApplySpeakerDataToDialogueContainer(string speakerName)
        {
            // 발화자 이름을 통해 캐릭터를 가져옵니다.
            Character character = CharacterManager.instance.GetCharacter(speakerName);

            // 캐릭터가 null이 아니면, 해당 캐릭터의 구성 데이터를 가져옵니다.
            // 캐릭터가 null이면, 발화자 이름을 통해 캐릭터의 구성 데이터를 가져옵니다.
            CharacterConfigData config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            // 구성 데이터를 대화 컨테이너에 적용합니다.
            ApplySpeakerDataToDialogueContainer(config);
        }

        /// <summary>
        /// 캐릭터의 구성 데이터를 대화 컨테이너에 적용합니다.
        /// </summary>
        /// <param name="config">적용할 캐릭터 구성 데이터입니다.</param>
        public void ApplySpeakerDataToDialogueContainer(CharacterConfigData config)
        {
            // 발화자 이름 컨테이너에 캐릭터의 이름 색상과 폰트와 사이즈를 설정합니다.
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            float fontSize = this.config.defaultNameFontSize * config.nameFontScale;
            dialogueContainer.nameContainer.SetNameFontSize(fontSize);

            // 대화 컨테이너에 캐릭터의 대화 색상과 폰트와 사이즈를 설정합니다.
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            fontSize = this.config.defaultDialogueFontSize * this.config.dialogueFontScale * config.dialogueFontScale;
            dialogueContainer.SetDialogueFontSize(fontSize);



        }

        /// <summary>
        /// 발화자의 이름을 화면에 표시하는 메서드입니다.
        /// 발화자의 이름이 "내레이션"이면 발화자 이름을 숨깁니다.
        /// </summary>
        /// <param name="speakerName">화면에 표시할 발화자의 이름입니다. 기본값은 빈 문자열입니다.</param>
        public void ShowSpeakerName(string speakerName = "")
        {
            if(speakerName.ToLower() != "내레이션")
                dialogueContainer.nameContainer.Show(speakerName);
            else
                HideSpeakerName();

        }

        /// <summary>
        /// 화면에 표시된 발화자의 이름을 숨기는 메서드입니다.
        /// </summary>
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        /// <summary>
        /// 발화자와 대화 내용을 받아 대화를 생성하고 표시하는 메서드입니다.
        /// </summary>
        /// <param name="speaker">발화자의 이름입니다.</param>
        /// <param name="dialogue">대화의 내용입니다.</param>
        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker}\"{dialogue}\"" };
            return Say(conversation);
        }

        /// <summary>
        /// 대화 내용을 리스트로 받아 대화를 생성하고 표시하는 메서드입니다.
        /// </summary>
        /// <param name="conversation">대화 내용을 포함하는 문자열 리스트입니다.</param>
        public Coroutine Say(List<string> lines)
        {
            Conversation conversation = new Conversation(lines);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public bool isVisible => cgController.isVisible;

        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);

        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);

    }
}
