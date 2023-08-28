using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���� �׽�Ʈ�� ���� Ŭ������ �����մϴ�.
public class TestFiles : MonoBehaviour
{
    // �׽�Ʈ�� ������ �̸��� �����ϴ� �ʵ��Դϴ�.
    [SerializeField]private TextAsset fileName;

    // ������ ���۵� �� ȣ��Ǵ� �޼����Դϴ�.
    void Start()
    {
        // ���� �׽�Ʈ�� �����ϴ� �ڷ�ƾ�� ȣ���մϴ�.
        StartCoroutine(Run());
    }

    // ���� �׽�Ʈ�� �����ϴ� �ڷ�ƾ�Դϴ�.
    IEnumerator Run()
    {
        /* ���� ���
        FileManager�� �̿��Ͽ� ������ ������ �о�ɴϴ�.(+ ���� ���)
        List<string> lines = FileManager.ReadTextFile(fileName, true);
        */
        /* ���� �����
        FileManager�� �̿��Ͽ� ������ ������ �о�ɴϴ�.
         */
        List<string> lines = FileManager.ReadTextAsset(fileName, false);

        // �о�� �� ������ ����մϴ�.
        foreach (string line in lines)
            Debug.Log(line);

        yield return null;
    }
}
