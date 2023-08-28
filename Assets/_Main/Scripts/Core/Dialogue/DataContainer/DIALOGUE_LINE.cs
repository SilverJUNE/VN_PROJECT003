using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DIALOGUE_LINE 클래스는 대화 시스템에서 각 대화 줄을 관리합니다. 
    /// 각 대화 줄은 발화자 정보(speakerData), 대화 내용(dialogueData), 명령어(commandData)로 구성됩니다.
    /// </summary>
    public class DIALOGUE_LINE
    {
        public string rawData { get; private set; } = string.Empty;
        // 발화자 정보를 저장하는 변수입니다.
        public DL_SPEAKER_DATA     speakerData;
        // 대화 내용을 저장하는 변수입니다.
        public DL_DIALOGUE_DATA    dialogueData;
        // 명령어를 저장하는 변수입니다.
        public DL_COMMAND_DATA     commandData;

        // 발화자 정보가 있는지를 나타내는 프로퍼티입니다. ※ 초기 형태speaker  != string.Empty;
        public bool    hasSpeaker  =>  speakerData != null;
        // 대화 내용이 있는지를 나타내는 프로퍼티입니다.
        public bool    hasDialogue =>  dialogueData != null;
        // 명령어가 있는지를 나타내는 프로퍼티입니다.
        public bool    hasCommand  =>  commandData != null;

        /// <summary>
        /// DIALOGUE_LINE 클래스의 생성자입니다. 
        /// 각 인자를 받아 발화자 정보, 대화 내용, 명령어를 초기화합니다.
        /// </summary>
        /// <param name="speaker">발화자 정보를 나타내는 문자열입니다.</param>
        /// <param name="dialogue">대화 내용을 나타내는 문자열입니다.</param>
        /// <param name="commands">명령어를 나타내는 문자열입니다.</param>
        public DIALOGUE_LINE(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawData;
            this.speakerData    =   (string.IsNullOrWhiteSpace(speaker)  ? null : new DL_SPEAKER_DATA(speaker));
            this.dialogueData   =   (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DIALOGUE_DATA(dialogue));
            this.commandData   =   (string.IsNullOrWhiteSpace(commands)  ? null : new DL_COMMAND_DATA(commands));
        }
    }
}

