using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DialogueParser 클래스는 대사 라인을 발화자, 대사, 그리고 명령어 세 가지 구성 요소로 파싱하는 기능을 제공합니다.
    /// </summary>
    public class DialogueParser
    {
        /// <summary>
        /// 명령어 문자열을 식별하는 데 사용하는 정규 표현식 패턴입니다.
        /// </summary>
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";

        /// <summary>
        /// 원시 대사 라인을 파싱하여 DIALOGUE_LINE 객체를 반환합니다.
        /// </summary>
        /// <param name="rawLine">파싱할 원시 대사 라인입니다.</param>
        /// <returns>파싱된 DIALOGUE_LINE 객체입니다.</returns>
        public static DIALOGUE_LINE Parse(string rawLine)
        {
            // 원시 대사 라인을 발화자, 대사, 명령어로 나눕니다.  ※ Debug.Log($"parsing line - '{rawLine}'");
            (string speaker, string dialogue, string commands) = RipContent(rawLine);


            // 파싱된 결과를 DIALOGUE_LINE 객체로 반환합니다.     ※ Debug.Log($"Speaker = '{speaker}'\nDialogue ='{dialogue}'\nCommands = '{commands}'");
            return new DIALOGUE_LINE(rawLine, speaker, dialogue, commands);
        }

        /// <summary>
        /// 원시 대사 라인을 발화자, 대사, 명령어로 나눕니다.
        /// 이 메서드는 문자열을 스캔하면서 발화자, 대사, 명령어 각각의 시작과 끝을 찾아냅니다.
        /// 이 과정에서 발화자와 대사, 명령어가 존재하지 않을 수도 있습니다. 이 경우 빈 문자열로 처리됩니다.
        /// </summary>
        /// <param name="rawLine">나눌 원시 대사 라인입니다.</param>
        /// <returns>발화자, 대사, 명령어를 튜플 형태로 반환합니다.</returns>
        private static (string, string, string) RipContent(string rawLine)
        {
            // 발화자, 대사, 명령어 각각의 시작과 끝 인덱스를 초기화합니다.
            string speaker = "", dialogue = "", commands = "";

            // 대사의 시작과 끝 인덱스를 저장할 변수를 선언하고 초기화합니다.
            int     dialogueStart = -1;
            int     dialogueEnd = -1;

            // 이스케이프 문자('\\')를 처리하기 위한 불리언 변수입니다.
            bool    isEscaped = false;

            // 원시 대사 라인을 문자 단위로 스캔합니다.
            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];

                // 이스케이프 문자를 발견하면 이스케이프 상태를 토글합니다.
                if (current == '\\')
                    isEscaped = !isEscaped;

                // 따옴표를 발견하면 대사의 시작 또는 끝으로 간주합니다.
                // 이스케이프 된 따옴표는 무시합니다.
                else if (current == '"' && !isEscaped)
                {
                    // 대사의 시작 인덱스를 설정합니다.
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    // 대사의 끝 인덱스를 설정합니다.
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                // 그 외의 경우는 이스케이프 상태를 해제합니다.
                else
                    isEscaped = false;
            }

            // 명령어 패턴을 식별하는 정규표현식을 사용하여 명령어의 시작 인덱스를 찾습니다
            Regex commandRegex = new Regex(commandRegexPattern);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach(Match match in matches)
            {
                // 대사의 범위 바깥에서 명령어를 찾으면 해당 인덱스를 저장합니다.
                if (match.Index < dialogueStart||match.Index > dialogueEnd)
                {
                    commandStart = match.Index;
                    break;
                }
            }

            // 명령어만 있는 경우를 처리합니다.
            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
                return ("", "", rawLine.Trim());

            // 여기에 도달했다면, 대사 또는 명령어에 대한 다중 단어 인수를 가지고 있습니다. 이것이 대사인지 확인해봅니다.
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                // 유효한 대사를 가지고 있다는 것을 알 수 있습니다.
                speaker     = rawLine.Substring(0, dialogueStart).Trim();           // 발화자, 대사, 명령어를 추출합니다.
                dialogue    = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");  // 대사를 추출합니다.
                
                if (commandStart != -1)
                    commands = rawLine.Substring(commandStart).Trim();      // 명령어를 추출합니다.
            }
            // 전체 라인이 명령어로 구성된 경우를 처리합니다.
            else if (commandStart != -1 && dialogueStart > commandStart) 
                commands = rawLine;
            // 전체 라인이 발화자로 구성된 경우를 처리합니다.
            else
                dialogue = rawLine;

            // 발화자, 대사, 명령어 튜플을 반환합니다.
            return (speaker, dialogue, commands);
        }
    }
}
