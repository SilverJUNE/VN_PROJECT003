using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DIALOGUE
{
    /// <summary>
    /// DL_COMMAND_DATA Ŭ������ ��ȭ �ý��ۿ��� ��ɾ� �����͸� ó���մϴ�.
    /// </summary>
    public class DL_COMMAND_DATA
    {
        // ��ɾ� ����Ʈ�� �����ϴ� �����Դϴ�.
        public List<Command> commands;

        // ��ɾ �����ϴ� �����Դϴ�.
        private const char COMMANDSPLITTER_ID = ',';
        // ��ɾ��� �μ��� �ѷ��δ� �����Դϴ�.
        private const char ARGUMENTSCONTAINER_ID = '(';
        // ��� ��ɾ��� �ĺ����Դϴ�.
        private const string WAITCOMMAND_ID = "[wait]";

        /// <summary>
        /// Command ����ü�� ���� ��ɾ��� ������ �����մϴ�.
        /// </summary>
        public struct Command
        {
            // ��ɾ��� �̸��� �����ϴ� �����Դϴ�.
            public string name;
            // ��ɾ��� �μ��� �����ϴ� �迭�Դϴ�.
            public string[] arguments;
            // ��ɾ��� �ϷḦ ��ٸ��� ���θ� ��Ÿ���� �����Դϴ�.
            public bool waitForCompletion;
        }

        /// <summary>
        /// DL_COMMAND_DATA Ŭ������ �������Դϴ�. 
        /// �־��� ���� ��ɾ� ���ڿ��� �м��Ͽ� ��ɾ �����մϴ�.
        /// </summary>
        /// <param name="rawCommands">�м��� ���� ��ɾ� ���ڿ��Դϴ�.</param>
        public DL_COMMAND_DATA(string rawCommands)
        {
            commands = RipCommands(rawCommands);
        }

        /// <summary>
        /// �־��� ���� ��ɾ� ���ڿ��� �м��Ͽ� ��ɾ� ����Ʈ�� �����մϴ�.
        /// rawCommands ���ڿ��� ��ǥ(',')�� �и��Ͽ� ������ ��ɾ �м��մϴ�.
        /// �� ��ɾ�� '(' �� �κ��� ��ɾ� �̸�����, �� �κ��� �μ��� �ν��մϴ�.
        /// ��ɾ� �̸��� "[wait]"�� �����ϴ� ��� waitForCompletion �Ӽ��� true�� �����մϴ�.
        /// </summary>
        /// <param name="rawCommands">�м��� ���� ��ɾ� ���ڿ��Դϴ�. 
        /// ��: "move(player,1,1), [wait]say(player, hello)"</param>
        /// <returns>������ ��ɾ� ����Ʈ�Դϴ�.</returns>
        private List<Command> RipCommands(string rawCommands)
        {
            // rawCommands�� ��ǥ(',')�� �и��Ͽ� data �迭�� �����մϴ�.
            string[] data = rawCommands.Split(COMMANDSPLITTER_ID, System.StringSplitOptions.RemoveEmptyEntries);
            List<Command> result = new List<Command>();

            // �� ���� ��ɾ� ���ڿ��� ���� ó���� �����մϴ�.
            foreach (string cmd in data)
            {
                Command command = new Command();


                // '('�� ��ġ�� ã�� ��ɾ� �̸��� �μ� �κ��� �и��մϴ�.
                int index = cmd.IndexOf(ARGUMENTSCONTAINER_ID);
                command.name = cmd.Substring(0, index).Trim();

                // ��ɾ� �̸��� "[wait]"�� �����ϴ� ��� waitForCompletion�� true�� �����ϰ�, "[wait]" �κ��� �����մϴ�.
                if (command.name.ToLower().StartsWith(WAITCOMMAND_ID))
                {
                    command.name = command.name.Substring(WAITCOMMAND_ID.Length);
                    command.waitForCompletion = true;
                }
                // "[wait]"�� �������� �ʴ� ��� waitForCompletion�� false�Դϴ�.
                else
                    command.waitForCompletion = false;

                // �μ� �κ��� �м��Ͽ� command.arguments�� �����մϴ�.
                command.arguments = GetArgs(cmd.Substring(index + 1, cmd.Length - index - 2));
                // �м� �Ϸ�� ��ɾ ��� ����Ʈ�� �߰��մϴ�.
                result.Add(command);
            }

            return result;
        }

        /// <summary>
        /// �־��� �μ� ���ڿ��� �м��Ͽ� �μ� ����Ʈ�� �����մϴ�.
        /// �μ� ���ڿ� ���� �ֵ���ǥ�� ���ڿ� �μ��� ��踦 ��Ÿ���ϴ�.
        /// �ֵ���ǥ ���� ������ �μ��� �Ϻη� �ν��ϸ�, �ֵ���ǥ ���� ������ �μ��� �����ϴ� �� ���˴ϴ�.
        /// </summary>
        /// <param name="args">�м��� �μ� ���ڿ��Դϴ�.
        /// ��: "\"Hello, World!\", 1, 2"</param>
        /// <returns>������ �μ� ����Ʈ�Դϴ�.</returns>
        private string[] GetArgs(string args)
        {
            // �μ��� ���� ����Ʈ�� ���� �μ��� ������ StringBuilder�� �����մϴ�.
            List<string> argList = new List<string>();
            StringBuilder currentArg = new StringBuilder();
            bool inQuotes = false;

            // args ���ڿ��� �� ���ڸ� ��ȸ�ϸ� �μ��� �����մϴ�.
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == '"')
                {
                    // �ֵ���ǥ�� ������ inQuotes ���¸� ����մϴ�.
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && args[i] == ' ')
                {
                    // �ֵ���ǥ ���� ������ ������ ������� ������ �μ��� ����Ʈ�� �߰��ϰ�, currentArg�� �ʱ�ȭ�մϴ�.
                    argList.Add(currentArg.ToString());
                    currentArg.Clear();
                    continue;
                }


                // �� ���� ��� ���� ���ڸ� currentArg�� �߰��մϴ�.
                currentArg.Append(args[i]);
            }

            // ������ �μ��� ����Ʈ�� �߰��մϴ�.
            if (currentArg.Length > 0)
                argList.Add(currentArg.ToString());

            return argList.ToArray();
        }
    }

}
