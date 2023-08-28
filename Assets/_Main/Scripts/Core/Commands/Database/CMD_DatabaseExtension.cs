using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COMMANDS
{
    /// <summary>
    /// CMD_DatabaseExtension은 CommandDatabase를 확장하는 추상 클래스입니다.
    /// 상속받은 클래스는 이를 통해 CommandDatabase를 확장할 수 있습니다.
    /// </summary>
    public abstract class CMD_DatabaseExtension
    {
        /// <summary>
        /// CommandDatabase를 확장하는 메서드입니다. 
        /// 이 메서드는 기본적으로 아무 작업도 수행하지 않지만, 
        /// 이 클래스를 상속하는 클래스에서 이 메서드를 오버라이드하여 특정 동작을 추가할 수 있습니다.
        /// </summary>
        /// <param name="databases">확장하려는 CommandDatabase 인스턴스입니다.</param>
        public static void Extend(CommandDatabase databases) { }

        public static CommandParameters ConvertDataToParameters(string[] data, int startingIndex = 0) => new CommandParameters(data, startingIndex);
    }
}

