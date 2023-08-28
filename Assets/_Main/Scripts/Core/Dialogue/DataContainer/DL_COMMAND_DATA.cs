using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DL_COMMAND_DATA 클래스는 대화 시스템에서 명령어 데이터를 처리합니다.
    /// </summary>
    public class DL_COMMAND_DATA
    {
        // 명령어 리스트를 저장하는 변수입니다.
        public List<Command> commands;

        // 명령어를 구분하는 문자입니다.
        private const char COMMANDSPLITTER_ID = ',';
        // 명령어의 인수를 둘러싸는 문자입니다.
        private const char ARGUMENTSCONTAINER_ID = '(';
        // 대기 명령어의 식별자입니다.
        private const string WAITCOMMAND_ID = "[wait]";

        /// <summary>
        /// Command 구조체는 개별 명령어의 정보를 저장합니다.
        /// </summary>
        public struct Command
        {
            // 명령어의 이름을 저장하는 변수입니다.
            public string name;
            // 명령어의 인수를 저장하는 배열입니다.
            public string[] arguments;
            // 명령어의 완료를 기다릴지 여부를 나타내는 변수입니다.
            public bool waitForCompletion;
        }

        /// <summary>
        /// DL_COMMAND_DATA 클래스의 생성자입니다. 
        /// 주어진 원시 명령어 문자열을 분석하여 명령어를 생성합니다.
        /// </summary>
        /// <param name="rawCommands">분석할 원시 명령어 문자열입니다.</param>
        public DL_COMMAND_DATA(string rawCommands)
        {
            commands = RipCommands(rawCommands);
        }

        /// <summary>
        /// 주어진 원시 명령어 문자열을 분석하여 명령어 리스트를 생성합니다.
        /// rawCommands 문자열을 쉼표(',')로 분리하여 각각의 명령어를 분석합니다.
        /// 각 명령어는 '(' 앞 부분을 명령어 이름으로, 뒷 부분을 인수로 인식합니다.
        /// 명령어 이름이 "[wait]"로 시작하는 경우 waitForCompletion 속성을 true로 설정합니다.
        /// </summary>
        /// <param name="rawCommands">분석할 원시 명령어 문자열입니다. 
        /// 예: "move(player,1,1), [wait]say(player, hello)"</param>
        /// <returns>생성된 명령어 리스트입니다.</returns>
        private List<Command> RipCommands(string rawCommands)
        {
            // rawCommands를 쉼표(',')로 분리하여 data 배열을 생성합니다.
            string[] data = rawCommands.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> result = new List<Command>();

            // 각 원시 명령어 문자열에 대해 처리를 수행합니다.
            foreach (string cmd in data)
            {
                Command command = new Command();


                // '('의 위치를 찾아 명령어 이름과 인수 부분을 분리합니다.
                int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
                command.name = cmd.Substring(0, index).Trim();

                // 명령어 이름이 "[wait]"로 시작하는 경우 waitForCompletion을 true로 설정하고, "[wait]" 부분을 제거합니다.
                if (command.name.ToLower().StartsWith(WAITCOMMAND_ID))
                {
                    command.name = command.name.Substring(WAITCOMMAND_ID.Length);
                    command.waitForCompletion = true;
                }
                // "[wait]"로 시작하지 않는 경우 waitForCompletion은 false입니다.
                else
                    command.waitForCompletion = false;

                // 인수 부분을 분석하여 command.arguments에 저장합니다.
                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
                // 분석 완료된 명령어를 결과 리스트에 추가합니다.
                result.Add(command);
            }

            return result;
        }

        /// <summary>
        /// 주어진 인수 문자열을 분석하여 인수 리스트를 생성합니다.
        /// 인수 문자열 내의 쌍따옴표는 문자열 인수의 경계를 나타냅니다.
        /// 쌍따옴표 안의 공백은 인수의 일부로 인식하며, 쌍따옴표 밖의 공백은 인수를 구분하는 데 사용됩니다.
        /// </summary>
        /// <param name="args">분석할 인수 문자열입니다.
        /// 예: "\"Hello, World!\", 1, 2"</param>
        /// <returns>생성된 인수 리스트입니다.</returns>
        private string[] GetArgs(string args)
        {
            // 인수를 담을 리스트와 현재 인수를 구성할 StringBuilder를 생성합니다.
            List<string> argList = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;

            // args 문자열의 각 문자를 순회하며 인수를 추출합니다.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '"')
                {
                    // 쌍따옴표를 만나면 inQuotes 상태를 토글합니다.
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && args[i] == ' ')
                {
                    // 쌍따옴표 밖의 공백을 만나면 현재까지 구성된 인수를 리스트에 추가하고, currentArg를 초기화합니다.
                    argList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }


                // 그 외의 경우 현재 문자를 currentArg에 추가합니다.
                currentArg.Append(args[i]);
            }

            // 마지막 인수를 리스트에 추가합니다.
            if (currentArg.Length > 0)
                argList.Add(currentArg.ToString());

            return argList.ToArray();
        }
    }

}
