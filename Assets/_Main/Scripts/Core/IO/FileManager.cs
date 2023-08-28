using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// FileManager Ŭ������ ������ �о� ó���ϴ� ����� �����մϴ�.
/// </summary>  
public class FileManager
{
    /// <summary>
    /// ������ �ؽ�Ʈ ������ �о� �� ������ ����Ʈ�� ��ҷ� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="filePath">�о���� �ؽ�Ʈ ������ ����Դϴ�.</param>
    /// <param name="includeBlackLines">�� ���� ����Ʈ�� �������� ���θ� �����մϴ�. �⺻���� true�Դϴ�.</param>
    /// <returns>�ؽ�Ʈ ������ �� ������ ��ҷ� �ϴ� ���ڿ� ����Ʈ�Դϴ�.</returns>
    public static List<string> ReadTextFile(string filePath, bool includeBlackLines = true)
    {
        // ���� ��ΰ� '/'�� �������� �ʴ´ٸ�, ��Ʈ ���� ��θ� �߰��մϴ�.
        if (!filePath.StartsWith('/'))
            filePath = FilePaths.root + filePath;

        List<string> lines = new List<string>();
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (includeBlackLines || !string.IsNullOrWhiteSpace(line))
                        lines.Add(line);
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogError($"File not found: '{ex.FileName}'");
        }

        return lines;
    }

    /// <summary>
    /// ������ ����� �ؽ�Ʈ Asset�� �о� �� ������ ����Ʈ�� ��ҷ� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="filePath">�о���� �ؽ�Ʈ Asset�� ����Դϴ�.</param>
    /// <param name="includeBlackLines">�� ���� ����Ʈ�� �������� ���θ� �����մϴ�. �⺻���� true�Դϴ�.</param>
    /// <returns>�ؽ�Ʈ Asset�� �� ������ ��ҷ� �ϴ� ���ڿ� ����Ʈ�Դϴ�.</returns>
    public static List<string> ReadTextAsset(string filePath, bool includeBlackLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(filePath);
        if (asset == null)
        {
            Debug.LogError($"Asset not found: '{filePath}'");
            return null;
        }

        return ReadTextAsset(asset, includeBlackLines);
    }

    /// <summary>
    /// �ؽ�Ʈ Asset�� �о ����Ʈ ���·� ��ȯ�ϴ� �޼����Դϴ�.
    /// </summary>
    /// <param name="asset">���� TextAsset�Դϴ�.</param>
    /// <param name="includeBlackLines">�� ���� �������� �����ϴ� �÷����Դϴ�. �⺻���� true�Դϴ�.</param>
    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlackLines = true)
    {
        List<string> lines = new List<string>();
        using (StringReader sr = new StringReader(asset.text))
        {
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (includeBlackLines || !string.IsNullOrWhiteSpace(line))
                    lines.Add(line);
            }
        }

        return lines;
    }
}