using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COMMANDS;

namespace Testing
{
    /// <summary>
    /// CMD_DatabaseExtension_Examples는 CMD_DatabaseExtension 클래스를 상속하여
    /// CommandDatabase를 확장하는 구체적인 예제 클래스입니다.
    /// 이 클래스는 특정 커맨드를 CommandDatabase에 추가하는 실제 방법을 보여줍니다.
    /// </summary>
    public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtension
    {
        /// <summary>
        /// CommandDatabase를 확장하는 메서드입니다. 
        /// 이 메서드는 "print"와 같은 여러 커맨드를 CommandDatabase에 추가합니다.
        /// </summary>
        /// <param name="database">확장할 CommandDatabase 인스턴스입니다.</param>
        new public static void Extend(CommandDatabase database)
        {

            // "print", "print_1p", "print_mp"라는 이름의 커맨드를 추가합니다.

            // 이 커맨드는 매개변수 없이 실행되며 이 커맨드가 호출되면, PrintDefaultMassage 메서드가 실행됩니다.
            database.AddCommand("print", new Action(PrintDefaultMassage));
            // 이 커맨드는 하나의 매개변수를 받아 실행되며, 이 커맨드가 호출되면, PrintUsermessage 메서드가 실행됩니다.
            database.AddCommand("print_1p", new Action<string>(PrintUsermessage));
            //  이 커맨드는 여러 개의 매개변수를 받아 실행되며, PrintLines 메서드가 실행됩니다.
            database.AddCommand("print_mp", new Action<string[]>(PrintLines));

            // "lambda", "lambda_1p", "lambda_mp" 라는 이름의 커맨드를 추가합니다.

            // 이 커맨드는 람다 함수를 사용하여 매개변수 없이 실행됩니다.
            database.AddCommand("lambda", new Action(() => { Debug.Log("Printing a default message to console from lambda command."); }));
            // 이 커맨드는 람다 함수를 사용하여 하나의 매개변수를 받아 실행됩니다.
            database.AddCommand("lambda_1p", new Action<string>((arg) => { Debug.Log($"Log User Lambda Message: '{arg}'"); }));
            //  이 커맨드는 람다 함수를 사용하여 여러 개의 매개변수를 받아 실행됩니다.
            database.AddCommand("lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

            // 코루틴을 사용하여 "process", "process_1p", "process_mp" 라는 이름의 커맨드를 추가합니다

            // 이 커맨드는 매개변수 없이 실행되는 코루틴입니다.
            database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
            // 이 커맨드는 하나의 매개변수를 받아 실행되는 코루틴입니다.
            database.AddCommand("process_1p", new Func<string, IEnumerator>(LineProcess));
            //이 커맨드는 여러 개의 매개변수를 받아 실행되는 코루틴입니다.
            database.AddCommand("process_mp", new Func<string[], IEnumerator>(MultiLineProcess));

            //SpecialExample
            database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));

        }

        /// <summary>
        /// 기본 메시지를 콘솔에 출력하는 메서드입니다.
        /// "print" 커맨드로 이 메서드가 호출됩니다.
        /// </summary>
        private static void PrintDefaultMassage()
        {
            Debug.Log("Printing a default message to console.");
        }

        /// <summary>
        /// PrintUsermessage 메서드는 "print_1p" 커맨드에 연결된 메서드입니다.
        /// 이 메서드는 사용자 지정 메시지를 콘솔에 출력합니다.
        /// </summary>
        /// <param name="message">출력할 사용자 지정 메시지입니다.</param>
        private static void PrintUsermessage(string message)
        {
            Debug.Log($"User Message: '{message}'");
        }

        /// <summary>
        /// PrintLines 메서드는 "print_mp" 커맨드에 연결된 메서드입니다.
        /// 이 메서드는 여러 개의 문자열을 순서대로 콘솔에 출력합니다.
        /// </summary>
        /// <param name="lines">출력할 문자열 배열입니다.</param>
        private static void PrintLines(string[] lines)
        {
            int i = 1;
            foreach (string line in lines)
            {
                Debug.Log($"{i++}. '{line}'");
            }
        }

        /// <summary>
        /// SimpleProcess 메서드는 "process" 커맨드에 연결된 코루틴입니다.
        /// 이 코루틴은 5번 반복하며, 각 반복마다 메시지를 콘솔에 출력합니다.
        /// </summary>
        /// <returns>IEnumerator 코루틴입니다.</returns>
        private static IEnumerator SimpleProcess()
        {
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"Process Running... [{i}]");
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// LineProcess 메서드는 "process_1p" 커맨드에 연결된 코루틴입니다.
        /// 이 코루틴은 입력받은 횟수만큼 반복하며, 각 반복마다 메시지를 콘솔에 출력합니다.
        /// </summary>
        /// <param name="data">코루틴이 반복할 횟수를 나타내는 문자열입니다. 이 문자열은 정수로 변환 가능해야 합니다.</param>
        /// <returns>IEnumerator 코루틴입니다.</returns>
        private static IEnumerator LineProcess(string data)
        {
            if (int.TryParse(data, out int num))
            {
                for (int i = 1; i <= num; i++)
                {
                    Debug.Log($"Process Running... [{i}]");
                    yield return new WaitForSeconds(1);
                }
            }
        }

        /// <summary>
        /// MultiLineProcess 메서드는 "process_mp" 커맨드에 연결된 코루틴입니다.
        /// 이 코루틴은 입력받은 각 문자열을 순서대로 콘솔에 출력합니다.
        /// </summary>
        /// <param name="data">출력할 문자열 배열입니다.</param>
        /// <returns>IEnumerator 코루틴입니다.</returns>
        private static IEnumerator MultiLineProcess(string[] data)
        {
            foreach (string line in data)
            {
                Debug.Log($"Process Message: '{line}'");
                yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// MoveCharacter 메서드는 코루틴으로, 특정 방향으로 캐릭터를 이동시키는 기능을 합니다.
        /// 이 메서드는 'moveCharDemo'라는 커맨드에 연결되어 있습니다.
        /// </summary>
        /// <param name="direction">캐릭터가 이동할 방향을 나타내는 문자열입니다. "left" 또는 "right"를 전달해야 합니다.</param>
        /// <returns>IEnumerator 코루틴입니다.</returns>
        private static IEnumerator MoveCharacter(string direction)
        {
            // 문자열 파라미터를 소문자로 변환하여 'left'인지 판별합니다.
            bool left = direction.ToLower() == "left";

            // 필요한 변수를 가져옵니다. 이 변수들은 나중에는 다른 곳에서 정의시킬 겁니다.
            // 'Image'라는 이름의 GameObject의 Transform 컴포넌트를 가져옵니다.
            Transform character = GameObject.Find("Image").transform;
            // 캐릭터의 이동 속도입니다.
            float moveSpeed = 15;

            // 이미지의 목표 위치를 계산합니다. 왼쪽이면 -8, 오른쪽이면 8입니다.
            float targetX = left ? -8 : 8;

            // 이미지의 현재 위치를 가져옵니다.
            float currentX = character.position.x;


            // 이미지를 서서히 목표 위치로 이동시킵니다.
            while (Mathf.Abs(targetX - currentX) > 0.1f)
            {
                //Debug.Log($"Moving character to {(left ? "left" : "right")}[{currentX}/{targetX}]");

                // 현재 위치를 목표 위치로부터 deltaTime * moveSpeed 만큼 이동시킵니다.
                currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
                // 캐릭터의 위치를 갱신합니다.
                character.position = new Vector3(currentX, character.position.y, character.position.z);
                // 다음 프레임까지 대기합니다.
                yield return null;
            }
        }

    }

}
