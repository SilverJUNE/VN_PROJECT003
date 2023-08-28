using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using System;
using UnityEngine.Events;
using CHARACTERS;

namespace COMMANDS
{
    /// <summary>
    /// CommandManager 클래스는 게임에서 사용하는 모든 명령어(캐릭터 움직임, 노래...)을 관리합니다.
    /// </summary>
    public class CommandManager : MonoBehaviour
    {
        private const char    SUB_COMMAND_IDENTIFIER       = '.';
        public  const string  DATABASE_CHARACTERS_BASE    = "characters"; 
        public  const string  DATABASE_CHARACTERS_SPRITE  = "characters_sprite";
        public  const string  DATABASE_CHARACTERS_LIVE2D  = "characters_live2D";
        public  const string  DATABASE_CHARACTERS_MODEL3D = "characters_model3D";

        /// <summary>
        /// CommandManager의 현재 인스턴스를 가져옵니다.
        /// </summary>
        public static CommandManager instance { get; private set; }

        // 명령어 데이터베이스를 저장하는 private 변수입니다.
        private CommandDatabase database;

        private Dictionary<string, CommandDatabase> subDatabases = new Dictionary<string, CommandDatabase>();

        private List<CommandProcess> activeProcesses = new List<CommandProcess>();
        private CommandProcess topProcess => activeProcesses.Last();

        /// <summary>
        /// MonoBehaviour의 Awake 이벤트에서 호출됩니다. 
        /// CommandManager의 인스턴스를 초기화하고, 데이터베이스를 만들고, 해당 어셈블리에서 확장 메서드를 찾아 데이터베이스를 확장합니다.
        /// </summary>
        private void Awake()
        {
            // CommandManager의 인스턴스가 없다면 인스턴스를 초기화하고, 명령어 데이터베이스를 생성합니다.
            if (instance == null)
            {
                instance = this;

                database = new CommandDatabase();

                Assembly assembly = Assembly.GetExecutingAssembly();
                // 현재 실행 중인 어셈블리에서 CMD_DatabaseExtension의 하위 클래스를 모두 찾아 확장 메서드를 찾습니다.
                Type[] extensionTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(CMD_DatabaseExtension))).ToArray();    // 뭔 뜻인지 모르겠다.

                foreach (Type extension in extensionTypes)
                {
                    // "Extend" 메서드를 찾아 데이터베이스를 확장합니다.
                    MethodInfo extendMethod = extension.GetMethod("Extend");
                    /* 
                    "Extend" 메서드를 실행하여 데이터베이스를 확장합니다.
                     데이터베이스는 object 배열로 전달되어야 합니다. 이는 Invoke 메서드의 요구 사항입니다.
                    */
                    extendMethod.Invoke(null, new object[] { database });
                }

            }
            // 이미 인스턴스가 있는 경우, 현재 게임 오브젝트를 즉시 파괴합니다.
            else
                DestroyImmediate(gameObject);
        }

        /// <summary>
        /// 주어진 명령어 이름에 대한 명령을 실행합니다.
        /// </summary>
        /// <param name="commandName">명령어 이름입니다.</param>
        /// <param name="args">명령어에 전달될 인수입니다.</param>
        public CoroutineWrapper Execute(string commandName, params string[] args)
        {
            if (commandName.Contains(SUB_COMMAND_IDENTIFIER))
                return ExecuteSubCommand(commandName, args);

            Delegate command = database.GetCommand(commandName);

            // 명령어가 데이터베이스에 없다면 null을 반환합니다.
            if (command == null)
                return null;

            // 명령어가 데이터베이스에 있다면 해당 명령어를 실행합니다.
            return StartProcess(commandName, command, args);
        }

        private CoroutineWrapper ExecuteSubCommand(string commandName, string[] args)
        {
            string[] parts           = commandName.Split(SUB_COMMAND_IDENTIFIER);
            string   databaseName    = string.Join(SUB_COMMAND_IDENTIFIER, parts.Take(parts.Length - 1));
            string   subCommandName  = parts.Last();

            if(subDatabases.ContainsKey(databaseName))
            {
                Delegate command = subDatabases[databaseName].GetCommand(subCommandName);
                if(command != null)
                {
                    return StartProcess(commandName, command, args);
                }
                else
                {
                    Debug.LogError($"No command called '{subCommandName}' was found in sub database '{databaseName}'");
                    return null;
                }
            }

            string characterName = databaseName;
            // If we've made it here then we should try to run as character command
            if(CharacterManager.instance.HasCharacter(characterName))
            {
                List<string> newArgs = new List<string>(args);
                newArgs.Insert(0, characterName);
                args = newArgs.ToArray();

                return ExecuteCharacterCommand(subCommandName, args);
            }

            Debug.LogError($"No sub database called '{databaseName}' exists! Command '{subCommandName}' could not be run");
            return null;

        }

        private CoroutineWrapper ExecuteCharacterCommand(string commandName, params string[] args)
        {
            Delegate command = null;

            CommandDatabase db = subDatabases[DATABASE_CHARACTERS_BASE];
            if(db.HasCommand(commandName))
            {
                command = db.GetCommand(commandName);
                return StartProcess(commandName, command, args);
            }

            CharacterConfigData characterConfigData = CharacterManager.instance.GetCharacterConfig(args[0]);

            switch (characterConfigData.characterType)
            {
                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    db = subDatabases[DATABASE_CHARACTERS_SPRITE];
                    break;
                case Character.CharacterType.Live2D:
                    db = subDatabases[DATABASE_CHARACTERS_LIVE2D];
                        break;
                case Character.CharacterType.Model3D:
                    db = subDatabases[DATABASE_CHARACTERS_MODEL3D];
                    break;
            }
            
            command = db.GetCommand(commandName);

            if (command != null)
                return StartProcess(commandName, command, args);

            Debug.LogError($"Command Manager was unable to execute command '{commandName}' on character '{args[0]}'. The character name or command may be invailed ");
            return null;

        }

        private CoroutineWrapper StartProcess(string commandName, Delegate command, string[] args)
        {
            System.Guid processID = System.Guid.NewGuid();
            CommandProcess cmd = new CommandProcess(processID, commandName, command, null, args, null);
            activeProcesses.Add(cmd);

            // 새로운 프로세스를 시작합니다.
            Coroutine co = StartCoroutine(RunningProcess(cmd));

            cmd.runningProcess = new CoroutineWrapper(this, co);

            return cmd.runningProcess;
        }

        public void StopCurrentProcess()
        {
            // 현재 실행 중인 프로세스가 있다면 코루틴을 중지합니다.
            if (topProcess != null)
                KillProcess(topProcess);
        }

        public void StopAllProcesses()
        {
            foreach (var c in activeProcesses)
            {
                if (c.runningProcess != null && !c.runningProcess.IsDone)
                    c.runningProcess.Stop();

                c.onTerminateAction?.Invoke();
            }

            activeProcesses.Clear();
        }

        private IEnumerator RunningProcess(CommandProcess process)
        {
            // 별도의 코루틴을 만들어 실행 중인 프로세스를 완료하고, 자신을 리셋합니다.
            /*
            "여기서 별도의 코루틴을 만든 유일한 이유는 
            커맨드 매니저에서 실행중인 프로세스가 호출된 프로세스를 완료하는 것을 기다리고,
            그 자신을 리셋하는 것 이외에는 아무것도 하지 않는다는 것을 분명하게 하기 위해서였습니다. 
            (이건 그저 나의 선호에 의한 것으로 지워도 되는 코드 입니다)"
            */
            yield return WaitingForProcessToComplete(process.command, process.args);

            KillProcess(process);
        }

        public void KillProcess(CommandProcess cmd)
        {
            activeProcesses.Remove(cmd);

            if (cmd.runningProcess != null && !cmd.runningProcess.IsDone)
                cmd.runningProcess.Stop();

            cmd.onTerminateAction?.Invoke();
        }

        private IEnumerator WaitingForProcessToComplete(Delegate command, string[] args)
        {
            // 명령어의 타입에 따라 해당 명령어를 실행합니다.
            if (command is Action)
                command.DynamicInvoke();

            else if (command is Action<string>)
                command.DynamicInvoke(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Action<string[]>)
                command.DynamicInvoke((object)args);

            else if (command is Func<IEnumerator>)
                yield return ((Func<IEnumerator>)command)();

            else if (command is Func<string, IEnumerator>)
                yield return ((Func<string, IEnumerator>)command)(args.Length == 0 ? string.Empty : args[0]);

            else if (command is Func<string[], IEnumerator>)
                yield return ((Func<string[], IEnumerator>)command)(args);
        }

        public void AddTerminationActionToCurrentProcess(UnityAction action)
        {
            CommandProcess process = topProcess;

            if (process == null)
                return;

            process.onTerminateAction = new UnityEvent();
            process.onTerminateAction.AddListener(action);
        }

        public CommandDatabase CreateSubDatabase(string name)
        {
            name = name.ToLower();

            if(subDatabases.TryGetValue(name, out CommandDatabase db))
            {
                Debug.LogWarning($"A database by the name of '{name}' already exists!");
                return db;
            }

            CommandDatabase newDatabase = new CommandDatabase();
            subDatabases.Add(name, newDatabase);

            return newDatabase;
        }
    }
}
