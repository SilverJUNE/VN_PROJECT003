using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 파일 테스트를 위한 클래스를 정의합니다.
public class TestFiles : MonoBehaviour
{
    // 테스트할 파일의 이름을 저장하는 필드입니다.
    [SerializeField]private TextAsset fileName;

    // 게임이 시작될 때 호출되는 메서드입니다.
    void Start()
    {
        // 파일 테스트를 시작하는 코루틴을 호출합니다.
        StartCoroutine(Run());
    }

    // 파일 테스트를 수행하는 코루틴입니다.
    IEnumerator Run()
    {
        /* 엔터 허용
        FileManager를 이용하여 파일의 내용을 읽어옵니다.(+ 엔터 허용)
        List<string> lines = FileManager.ReadTextFile(fileName, true);
        */
        /* 엔터 미허용
        FileManager를 이용하여 파일의 내용을 읽어옵니다.
         */
        List<string> lines = FileManager.ReadTextAsset(fileName, false);

        // 읽어온 각 라인을 출력합니다.
        foreach (string line in lines)
            Debug.Log(line);

        yield return null;
    }
}
