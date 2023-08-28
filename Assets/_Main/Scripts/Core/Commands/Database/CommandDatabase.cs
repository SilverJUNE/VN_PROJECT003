using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// CommandDatabase 클래스는 게임의 모든 명령을 관리하는 딕셔너리를 유지합니다.
    /// </summary>
    public class CommandDatabase
    {
        // 모든 명령을 저장하는 딕셔너리입니다.
        private Dictionary<string, Delegate> database = new Dictionary<string, Delegate>();

        /// <summary>
        /// 주어진 이름의 명령이 데이터베이스에 존재하는지 확인합니다.
        /// </summary>
        /// <param name="commandName">확인하려는 명령의 이름입니다.</param>
        /// <returns>명령이 데이터베이스에 존재하면 true를 반환하고, 그렇지 않으면 false를 반환합니다.</returns>
        public bool HasCommand(string commandName) => database.ContainsKey(commandName.ToLower());

        /// <summary>
        /// 새 명령을 데이터베이스에 추가합니다. 이미 존재하는 명령의 이름으로 명령을 추가하려고 하면 오류 메시지를 출력합니다.
        /// </summary>
        /// <param name="commandName">추가하려는 명령의 이름입니다.</param>
        /// <param name="command">추가하려는 명령입니다.</param>
        public void AddCommand(string commandName, Delegate command)
        {
            commandName = commandName.ToLower();

            if (!database.ContainsKey(commandName))
            {
                database.Add(commandName, command);
            }
            else
                Debug.LogError($"Command already exists in the database '{commandName}'");
        }

        /// <summary>
        /// 주어진 이름의 명령을 데이터베이스에서 검색합니다. 명령이 존재하지 않으면 오류 메시지를 출력하고 null을 반환합니다.
        /// </summary>
        /// <param name="commandName">검색하려는 명령의 이름입니다.</param>
        /// <returns>데이터베이스에서 찾은 명령. 명령이 없으면 null을 반환합니다.</returns>
        public Delegate GetCommand(string commandName)
        {
            commandName = commandName.ToLower();

            if (!database.ContainsKey(commandName))
            {
                Debug.LogError($"Command '{commandName}' does not exist in the database!");
                return null;
            }

            return database[commandName];
        }
    }

}

