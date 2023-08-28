using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DialogueParser Ŭ������ ��� ������ ��ȭ��, ���, �׸��� ��ɾ� �� ���� ���� ��ҷ� �Ľ��ϴ� ����� �����մϴ�.
    /// </summary>
    public class DialogueParser
    {
        /// <summary>
        /// ��ɾ� ���ڿ��� �ĺ��ϴ� �� ����ϴ� ���� ǥ���� �����Դϴ�.
        /// </summary>
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";

        /// <summary>
        /// ���� ��� ������ �Ľ��Ͽ� DIALOGUE_LINE ��ü�� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="rawLine">�Ľ��� ���� ��� �����Դϴ�.</param>
        /// <returns>�Ľ̵� DIALOGUE_LINE ��ü�Դϴ�.</returns>
        public static DIALOGUE_LINE Parse(string rawLine)
        {
            // ���� ��� ������ ��ȭ��, ���, ��ɾ�� �����ϴ�.  �� Debug.Log($"parsing line - '{rawLine}'");
            (string speaker, string dialogue, string commands) = RipContent(rawLine);


            // �Ľ̵� ����� DIALOGUE_LINE ��ü�� ��ȯ�մϴ�.     �� Debug.Log($"Speaker = '{speaker}'\nDialogue ='{dialogue}'\nCommands = '{commands}'");
            return new DIALOGUE_LINE(rawLine, speaker, dialogue, commands);
        }

        /// <summary>
        /// ���� ��� ������ ��ȭ��, ���, ��ɾ�� �����ϴ�.
        /// �� �޼���� ���ڿ��� ��ĵ�ϸ鼭 ��ȭ��, ���, ��ɾ� ������ ���۰� ���� ã�Ƴ��ϴ�.
        /// �� �������� ��ȭ�ڿ� ���, ��ɾ �������� ���� ���� �ֽ��ϴ�. �� ��� �� ���ڿ��� ó���˴ϴ�.
        /// </summary>
        /// <param name="rawLine">���� ���� ��� �����Դϴ�.</param>
        /// <returns>��ȭ��, ���, ��ɾ Ʃ�� ���·� ��ȯ�մϴ�.</returns>
        private static (string, string, string) RipContent(string rawLine)
        {
            // ��ȭ��, ���, ��ɾ� ������ ���۰� �� �ε����� �ʱ�ȭ�մϴ�.
            string speaker = "", dialogue = "", commands = "";

            // ����� ���۰� �� �ε����� ������ ������ �����ϰ� �ʱ�ȭ�մϴ�.
            int     dialogueStart = -1;
            int     dialogueEnd = -1;

            // �̽������� ����('\\')�� ó���ϱ� ���� �Ҹ��� �����Դϴ�.
            bool    isEscaped = false;

            // ���� ��� ������ ���� ������ ��ĵ�մϴ�.
            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];

                // �̽������� ���ڸ� �߰��ϸ� �̽������� ���¸� ����մϴ�.
                if (current == '\\')
                    isEscaped = !isEscaped;

                // ����ǥ�� �߰��ϸ� ����� ���� �Ǵ� ������ �����մϴ�.
                // �̽������� �� ����ǥ�� �����մϴ�.
                else if (current == '"' && !isEscaped)
                {
                    // ����� ���� �ε����� �����մϴ�.
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    // ����� �� �ε����� �����մϴ�.
                    else if (dialogueEnd == -1)
                        dialogueEnd = i;
                }
                // �� ���� ���� �̽������� ���¸� �����մϴ�.
                else
                    isEscaped = false;
            }

            // ��ɾ� ������ �ĺ��ϴ� ����ǥ������ ����Ͽ� ��ɾ��� ���� �ε����� ã���ϴ�
            Regex commandRegex = new Regex(commandRegexPattern);
            MatchCollection matches = commandRegex.Matches(rawLine);
            int commandStart = -1;
            foreach(Match match in matches)
            {
                // ����� ���� �ٱ����� ��ɾ ã���� �ش� �ε����� �����մϴ�.
                if (match.Index < dialogueStart||match.Index > dialogueEnd)
                {
                    commandStart = match.Index;
                    break;
                }
            }

            // ��ɾ �ִ� ��츦 ó���մϴ�.
            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
                return ("", "", rawLine.Trim());

            // ���⿡ �����ߴٸ�, ��� �Ǵ� ��ɾ ���� ���� �ܾ� �μ��� ������ �ֽ��ϴ�. �̰��� ������� Ȯ���غ��ϴ�.
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                // ��ȿ�� ��縦 ������ �ִٴ� ���� �� �� �ֽ��ϴ�.
                speaker     = rawLine.Substring(0, dialogueStart).Trim();           // ��ȭ��, ���, ��ɾ �����մϴ�.
                dialogue    = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");  // ��縦 �����մϴ�.
                
                if (commandStart != -1)
                    commands = rawLine.Substring(commandStart).Trim();      // ��ɾ �����մϴ�.
            }
            // ��ü ������ ��ɾ�� ������ ��츦 ó���մϴ�.
            else if (commandStart != -1 && dialogueStart > commandStart) 
                commands = rawLine;
            // ��ü ������ ��ȭ�ڷ� ������ ��츦 ó���մϴ�.
            else
                dialogue = rawLine;

            // ��ȭ��, ���, ��ɾ� Ʃ���� ��ȯ�մϴ�.
            return (speaker, dialogue, commands);
        }
    }
}
