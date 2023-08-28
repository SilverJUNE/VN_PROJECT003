using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

public class TestInputPanel : MonoBehaviour
{
    public InputPanel inputPanel;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Running());
    }


    IEnumerator Running()
    {
        Character 하영 = CharacterManager.instance.CreateCharacter("하영", revealAfterCreation: true);

        yield return 하영.Say("안녕 이름이 뭐니?");

        inputPanel.Show("이름이 뭡니까?");

        while (inputPanel.isWaitOnUserInput)
            yield return null;

        string characterName = inputPanel.lastInput;

        yield return 하영.Say($"만나서 너무나 반가워 {characterName}!");
    }
}
