using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

public class TestParsing : MonoBehaviour
{
    private void Start()
    {
        SendFileToParse();
    }

    private void SendFileToParse()
    {
        List<string> lines = FileManager.ReadTextAsset("testFile",false);

        foreach(string line in lines)
        {
            DIALOGUE_LINE dl = DialogueParser.Parse(line);
        }
    }

    private void TestParse_1()
    {
        string line = "캐릭터\"이야기 \\\"하는 부분은\\\" 여기에요!\" 명령(명령의 내용은 여기)";

        DialogueParser.Parse(line);
    }
}
