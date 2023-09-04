using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COMMANDS;
using CHARACTERS;
using DIALOGUE.LogicalLines;

namespace DIALOGUE
{
    /// <summary>
    /// ConversationManager 클래스는 대화의 실행과 관리를 담당합니다.
    /// </summary>
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;

        private Coroutine process = null;

        public bool isRunning => process != null;

        public TextArchitect architect = null;
        private bool userPrompt = false;

        private TagManager tagManager;
        private LogicalLineManager logicalLineManager;

        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);

        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());
        
        private ConversationQueue conversationQueue;

        /// <summary>
        /// ConversationManager 클래스의 생성자입니다. TextArchitect 인스턴스를 인수로 받습니다.
        /// </summary>
        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;

            tagManager = new TagManager();
            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        /// <summary>
        /// 사용자가 다음 버튼을 눌렀을 때 호출되는 메서드입니다.
        /// </summary>
        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }

        /// <summary>
        /// 대화를 시작하는 메서드입니다. 대화 내용의 목록을 인수로 받습니다.
        /// </summary>
        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());

            return process;
        }

        /// <summary>
        /// 현재 진행 중인 대화를 중지하는 메서드입니다.
        /// </summary>
        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        /// <summary>
        /// 대화를 실행하는 코루틴입니다. 대화 내용을 담은 문자열 목록을 인수로 받습니다.
        /// </summary>
        IEnumerator RunningConversation()
        {
            // 대화 목록의 각 요소에 대해
            while (!conversationQueue.IsEmpty())
            { 
                Conversation currentConversation = conversation;

                if(currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = currentConversation.CurrentLine();

                // 공백이거나 내용이 없는 라인은 건너뜁니다.
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                // 각 대화 라인을 파싱하여 DIALOGUE_LINE 객체를 생성합니다.
                DIALOGUE_LINE line = DialogueParser.Parse(rawLine);

                if(logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    yield return logic;
                }
                else
                {
                    // 대화 내용이 있는 경우, 대화를 실행하는 코루틴을 호출합니다.
                    if (line.hasDialogue)
                        yield return Line_RunDialogue(line);

                    // 명령이 있는 경우, 명령을 실행하는 코루틴을 호출합니다.
                    if (line.hasCommand)
                        yield return Line_RunCommands(line);

                    //Wait for user input if dialogue was in this line
                    if (line.hasDialogue)
                    {
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcesses();
                    }
                }

                TryAdvanceConversation(currentConversation);
            }

            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
        }

        /// <summary>
        /// 대사를 실행하는 코루틴입니다. 대사 정보를 담은 DIALOGUE_LINE 객체를 인수로 받습니다.
        /// </summary>
        IEnumerator Line_RunDialogue(DIALOGUE_LINE line)
        {
            // 화자가 존재하는 경우, 화자의 이름을 표시하거나 숨깁니다.
            if (line.hasSpeaker)
                HandlesSpeakerLogic(line.speakerData);

            //If the dialogue box is not visible - make sure it becomes visible automatically
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            // 대사를 구성합니다.
            yield return BuildLineSegments(line.dialogueData);

        }


        private void HandlesSpeakerLogic(DL_SPEAKER_DATA speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfDoesNotExist: characterMustBeCreated);

            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing))
                character.Show();

            //Add character name to the UI
            dialogueSystem.ShowSpeakerName(tagManager.Inject(speakerData.displayName));
            
            DialogueSystem.instance.ApplySpeakerDataToDialogueContainer(speakerData.name);

            //Cast position
            if (speakerData.isCastingPosition)
                character.MovePosition(speakerData.castPosition);

            //Casting Expression
            if(speakerData.isCastingExpressions)
            {
                foreach(var ce in speakerData.CastExpressions)
                {
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
                }
            }

        }

        /// <summary>
        /// 명령을 실행하는 코루틴입니다. 명령 정보를 담은 DIALOGUE_LINE 객체를 인수로 받습니다.
        /// </summary>
        IEnumerator Line_RunCommands(DIALOGUE_LINE line)
        {
            List<DL_COMMAND_DATA.Command> commands = line.commandData.commands;

            foreach(DL_COMMAND_DATA.Command command in  commands)
            {
                if (command.waitForCompletion || command.name == "wait")
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while(!cw.IsDone)
                    {
                        if (userPrompt)
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }
                        yield return null;
                    }
                }

                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }
            yield return null;
        }

        /// <summary>
        /// 대화 데이터를 구성하는 세그먼트를 순차적으로 처리합니다.
        /// 각 세그먼트에 대해 세그먼트의 시작 신호가 발생할 때까지 기다린 후, 해당 세그먼트의 대화를 출력합니다.
        /// </summary>
        /// <param name="line">처리할 대화 데이터입니다.</param>
        /// <returns>처리 과정을 제어하는 열거자입니다.</returns>
        IEnumerator BuildLineSegments(DL_DIALOGUE_DATA line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment = line.segments[i];

                // 세그먼트의 시작 신호가 발생할 때까지 기다립니다.
                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                // 세그먼트의 대화를 출력합니다.
                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        public bool isWaitingOnAutoTimer { get; private set; } = false;

        /// <summary>
        /// 대화 세그먼트의 시작 신호가 발생할 때까지 기다립니다.
        /// 사용자 입력을 기다리거나, 지정된 시간을 기다립니다.
        /// </summary>
        /// <param name="segment">시작 신호를 기다릴 대화 세그먼트입니다.</param>
        /// <returns>기다리는 과정을 제어하는 열거자입니다.</returns>
        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment)
        {
            switch(segment.startSignal)
            {
                // 사용자 입력을 기다립니다.
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.C:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                // 지정된 시간을 적으면 그 시간 뒤에 사용자의 입력을 기다립니다.
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WC:
                case DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 대화를 구성하는 코루틴입니다. 구성할 대화 문자열을 인수로 받습니다.
        /// </summary>
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = tagManager.Inject(dialogue);

            //Build the dialogue
            if(!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            //Wait for the dialogue to complete.
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }
                yield return null;
            }
        }

        /// <summary>
        /// 사용자 입력을 대기하는 코루틴입니다.
        /// </summary>
        IEnumerator WaitForUserInput()
        {
            dialogueSystem.prompt.Show();

            while (!userPrompt)
                yield return null;

            dialogueSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}

