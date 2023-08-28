using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using COMMANDS;

namespace Testing
{
    /// <summary>
    /// CMD_DatabaseExtension_Examples�� CMD_DatabaseExtension Ŭ������ ����Ͽ�
    /// CommandDatabase�� Ȯ���ϴ� ��ü���� ���� Ŭ�����Դϴ�.
    /// �� Ŭ������ Ư�� Ŀ�ǵ带 CommandDatabase�� �߰��ϴ� ���� ����� �����ݴϴ�.
    /// </summary>
    public class CMD_DatabaseExtension_Examples : CMD_DatabaseExtension
    {
        /// <summary>
        /// CommandDatabase�� Ȯ���ϴ� �޼����Դϴ�. 
        /// �� �޼���� "print"�� ���� ���� Ŀ�ǵ带 CommandDatabase�� �߰��մϴ�.
        /// </summary>
        /// <param name="database">Ȯ���� CommandDatabase �ν��Ͻ��Դϴ�.</param>
        new public static void Extend(CommandDatabase database)
        {

            // "print", "print_1p", "print_mp"��� �̸��� Ŀ�ǵ带 �߰��մϴ�.

            // �� Ŀ�ǵ�� �Ű����� ���� ����Ǹ� �� Ŀ�ǵ尡 ȣ��Ǹ�, PrintDefaultMassage �޼��尡 ����˴ϴ�.
            database.AddCommand("print", new Action(PrintDefaultMassage));
            // �� Ŀ�ǵ�� �ϳ��� �Ű������� �޾� ����Ǹ�, �� Ŀ�ǵ尡 ȣ��Ǹ�, PrintUsermessage �޼��尡 ����˴ϴ�.
            database.AddCommand("print_1p", new Action<string>(PrintUsermessage));
            //  �� Ŀ�ǵ�� ���� ���� �Ű������� �޾� ����Ǹ�, PrintLines �޼��尡 ����˴ϴ�.
            database.AddCommand("print_mp", new Action<string[]>(PrintLines));

            // "lambda", "lambda_1p", "lambda_mp" ��� �̸��� Ŀ�ǵ带 �߰��մϴ�.

            // �� Ŀ�ǵ�� ���� �Լ��� ����Ͽ� �Ű����� ���� ����˴ϴ�.
            database.AddCommand("lambda", new Action(() => { Debug.Log("Printing a default message to console from lambda command."); }));
            // �� Ŀ�ǵ�� ���� �Լ��� ����Ͽ� �ϳ��� �Ű������� �޾� ����˴ϴ�.
            database.AddCommand("lambda_1p", new Action<string>((arg) => { Debug.Log($"Log User Lambda Message: '{arg}'"); }));
            //  �� Ŀ�ǵ�� ���� �Լ��� ����Ͽ� ���� ���� �Ű������� �޾� ����˴ϴ�.
            database.AddCommand("lambda_mp", new Action<string[]>((args) => { Debug.Log(string.Join(", ", args)); }));

            // �ڷ�ƾ�� ����Ͽ� "process", "process_1p", "process_mp" ��� �̸��� Ŀ�ǵ带 �߰��մϴ�

            // �� Ŀ�ǵ�� �Ű����� ���� ����Ǵ� �ڷ�ƾ�Դϴ�.
            database.AddCommand("process", new Func<IEnumerator>(SimpleProcess));
            // �� Ŀ�ǵ�� �ϳ��� �Ű������� �޾� ����Ǵ� �ڷ�ƾ�Դϴ�.
            database.AddCommand("process_1p", new Func<string, IEnumerator>(LineProcess));
            //�� Ŀ�ǵ�� ���� ���� �Ű������� �޾� ����Ǵ� �ڷ�ƾ�Դϴ�.
            database.AddCommand("process_mp", new Func<string[], IEnumerator>(MultiLineProcess));

            //SpecialExample
            database.AddCommand("moveCharDemo", new Func<string, IEnumerator>(MoveCharacter));

        }

        /// <summary>
        /// �⺻ �޽����� �ֿܼ� ����ϴ� �޼����Դϴ�.
        /// "print" Ŀ�ǵ�� �� �޼��尡 ȣ��˴ϴ�.
        /// </summary>
        private static void PrintDefaultMassage()
        {
            Debug.Log("Printing a default message to console.");
        }

        /// <summary>
        /// PrintUsermessage �޼���� "print_1p" Ŀ�ǵ忡 ����� �޼����Դϴ�.
        /// �� �޼���� ����� ���� �޽����� �ֿܼ� ����մϴ�.
        /// </summary>
        /// <param name="message">����� ����� ���� �޽����Դϴ�.</param>
        private static void PrintUsermessage(string message)
        {
            Debug.Log($"User Message: '{message}'");
        }

        /// <summary>
        /// PrintLines �޼���� "print_mp" Ŀ�ǵ忡 ����� �޼����Դϴ�.
        /// �� �޼���� ���� ���� ���ڿ��� ������� �ֿܼ� ����մϴ�.
        /// </summary>
        /// <param name="lines">����� ���ڿ� �迭�Դϴ�.</param>
        private static void PrintLines(string[] lines)
        {
            int i = 1;
            foreach (string line in lines)
            {
                Debug.Log($"{i++}. '{line}'");
            }
        }

        /// <summary>
        /// SimpleProcess �޼���� "process" Ŀ�ǵ忡 ����� �ڷ�ƾ�Դϴ�.
        /// �� �ڷ�ƾ�� 5�� �ݺ��ϸ�, �� �ݺ����� �޽����� �ֿܼ� ����մϴ�.
        /// </summary>
        /// <returns>IEnumerator �ڷ�ƾ�Դϴ�.</returns>
        private static IEnumerator SimpleProcess()
        {
            for (int i = 1; i <= 5; i++)
            {
                Debug.Log($"Process Running... [{i}]");
                yield return new WaitForSeconds(1);
            }
        }

        /// <summary>
        /// LineProcess �޼���� "process_1p" Ŀ�ǵ忡 ����� �ڷ�ƾ�Դϴ�.
        /// �� �ڷ�ƾ�� �Է¹��� Ƚ����ŭ �ݺ��ϸ�, �� �ݺ����� �޽����� �ֿܼ� ����մϴ�.
        /// </summary>
        /// <param name="data">�ڷ�ƾ�� �ݺ��� Ƚ���� ��Ÿ���� ���ڿ��Դϴ�. �� ���ڿ��� ������ ��ȯ �����ؾ� �մϴ�.</param>
        /// <returns>IEnumerator �ڷ�ƾ�Դϴ�.</returns>
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
        /// MultiLineProcess �޼���� "process_mp" Ŀ�ǵ忡 ����� �ڷ�ƾ�Դϴ�.
        /// �� �ڷ�ƾ�� �Է¹��� �� ���ڿ��� ������� �ֿܼ� ����մϴ�.
        /// </summary>
        /// <param name="data">����� ���ڿ� �迭�Դϴ�.</param>
        /// <returns>IEnumerator �ڷ�ƾ�Դϴ�.</returns>
        private static IEnumerator MultiLineProcess(string[] data)
        {
            foreach (string line in data)
            {
                Debug.Log($"Process Message: '{line}'");
                yield return new WaitForSeconds(0.5f);
            }
        }

        /// <summary>
        /// MoveCharacter �޼���� �ڷ�ƾ����, Ư�� �������� ĳ���͸� �̵���Ű�� ����� �մϴ�.
        /// �� �޼���� 'moveCharDemo'��� Ŀ�ǵ忡 ����Ǿ� �ֽ��ϴ�.
        /// </summary>
        /// <param name="direction">ĳ���Ͱ� �̵��� ������ ��Ÿ���� ���ڿ��Դϴ�. "left" �Ǵ� "right"�� �����ؾ� �մϴ�.</param>
        /// <returns>IEnumerator �ڷ�ƾ�Դϴ�.</returns>
        private static IEnumerator MoveCharacter(string direction)
        {
            // ���ڿ� �Ķ���͸� �ҹ��ڷ� ��ȯ�Ͽ� 'left'���� �Ǻ��մϴ�.
            bool left = direction.ToLower() == "left";

            // �ʿ��� ������ �����ɴϴ�. �� �������� ���߿��� �ٸ� ������ ���ǽ�ų �̴ϴ�.
            // 'Image'��� �̸��� GameObject�� Transform ������Ʈ�� �����ɴϴ�.
            Transform character = GameObject.Find("Image").transform;
            // ĳ������ �̵� �ӵ��Դϴ�.
            float moveSpeed = 15;

            // �̹����� ��ǥ ��ġ�� ����մϴ�. �����̸� -8, �������̸� 8�Դϴ�.
            float targetX = left ? -8 : 8;

            // �̹����� ���� ��ġ�� �����ɴϴ�.
            float currentX = character.position.x;


            // �̹����� ������ ��ǥ ��ġ�� �̵���ŵ�ϴ�.
            while (Mathf.Abs(targetX - currentX) > 0.1f)
            {
                //Debug.Log($"Moving character to {(left ? "left" : "right")}[{currentX}/{targetX}]");

                // ���� ��ġ�� ��ǥ ��ġ�κ��� deltaTime * moveSpeed ��ŭ �̵���ŵ�ϴ�.
                currentX = Mathf.MoveTowards(currentX, targetX, moveSpeed * Time.deltaTime);
                // ĳ������ ��ġ�� �����մϴ�.
                character.position = new Vector3(currentX, character.position.y, character.position.z);
                // ���� �����ӱ��� ����մϴ�.
                yield return null;
            }
        }

    }

}
