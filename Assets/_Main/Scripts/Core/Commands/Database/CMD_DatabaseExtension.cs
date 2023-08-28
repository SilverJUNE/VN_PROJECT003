using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// CMD_DatabaseExtension�� CommandDatabase�� Ȯ���ϴ� �߻� Ŭ�����Դϴ�.
    /// ��ӹ��� Ŭ������ �̸� ���� CommandDatabase�� Ȯ���� �� �ֽ��ϴ�.
    /// </summary>
    public abstract class CMD_DatabaseExtension
    {
        /// <summary>
        /// CommandDatabase�� Ȯ���ϴ� �޼����Դϴ�. 
        /// �� �޼���� �⺻������ �ƹ� �۾��� �������� ������, 
        /// �� Ŭ������ ����ϴ� Ŭ�������� �� �޼��带 �������̵��Ͽ� Ư�� ������ �߰��� �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="databases">Ȯ���Ϸ��� CommandDatabase �ν��Ͻ��Դϴ�.</param>
        public static void Extend(CommandDatabase databases) { }

        public static CommandParameters ConvertDataToParameters(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
    }
}

