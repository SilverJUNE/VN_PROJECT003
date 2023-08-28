using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DIALOGUE_LINE Ŭ������ ��ȭ �ý��ۿ��� �� ��ȭ ���� �����մϴ�. 
    /// �� ��ȭ ���� ��ȭ�� ����(speakerData), ��ȭ ����(dialogueData), ��ɾ�(commandData)�� �����˴ϴ�.
    /// </summary>
    public class DIALOGUE_LINE
    {
        public string rawData { get; private set; } = string.Empty;
        // ��ȭ�� ������ �����ϴ� �����Դϴ�.
        public DL_SPEAKER_DATA     speakerData;
        // ��ȭ ������ �����ϴ� �����Դϴ�.
        public DL_DIALOGUE_DATA    dialogueData;
        // ��ɾ �����ϴ� �����Դϴ�.
        public DL_COMMAND_DATA     commandData;

        // ��ȭ�� ������ �ִ����� ��Ÿ���� ������Ƽ�Դϴ�. �� �ʱ� ����speaker  != string.Empty;
        public bool    hasSpeaker  =>  speakerData != null;
        // ��ȭ ������ �ִ����� ��Ÿ���� ������Ƽ�Դϴ�.
        public bool    hasDialogue =>  dialogueData != null;
        // ��ɾ �ִ����� ��Ÿ���� ������Ƽ�Դϴ�.
        public bool    hasCommand  =>  commandData != null;

        /// <summary>
        /// DIALOGUE_LINE Ŭ������ �������Դϴ�. 
        /// �� ���ڸ� �޾� ��ȭ�� ����, ��ȭ ����, ��ɾ �ʱ�ȭ�մϴ�.
        /// </summary>
        /// <param name="speaker">��ȭ�� ������ ��Ÿ���� ���ڿ��Դϴ�.</param>
        /// <param name="dialogue">��ȭ ������ ��Ÿ���� ���ڿ��Դϴ�.</param>
        /// <param name="commands">��ɾ ��Ÿ���� ���ڿ��Դϴ�.</param>
        public DIALOGUE_LINE(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawData;
            this.speakerData    =   (string.IsNullOrWhiteSpace(speaker)  ? null : new DL_SPEAKER_DATA(speaker));
            this.dialogueData   =   (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DIALOGUE_DATA(dialogue));
            this.commandData   =   (string.IsNullOrWhiteSpace(commands)  ? null : new DL_COMMAND_DATA(commands));
        }
    }
}

