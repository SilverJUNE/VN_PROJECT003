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
        string line = "ĳ����\"�̾߱� \\\"�ϴ� �κ���\\\" ���⿡��!\" ���(����� ������ ����)";

        DialogueParser.Parse(line);
    }
}
