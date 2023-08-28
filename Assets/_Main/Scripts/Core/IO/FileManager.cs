using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// FileManager 클래스는 파일을 읽어 처리하는 기능을 제공합니다.
/// </summary>  
public class FileManager
{
    /// <summary>
    /// 지정된 텍스트 파일을 읽어 각 라인을 리스트의 요소로 반환합니다.
    /// </summary>
    /// <param name="filePath">읽어들일 텍스트 파일의 경로입니다.</param>
    /// <param name="includeBlackLines">빈 줄을 리스트에 포함할지 여부를 결정합니다. 기본값은 true입니다.</param>
    /// <returns>텍스트 파일의 각 라인을 요소로 하는 문자열 리스트입니다.</returns>
    public static List<string> ReadTextFile(string filePath, bool includeBlackLines = true)
    {
        // 파일 경로가 '/'로 시작하지 않는다면, 루트 파일 경로를 추가합니다.
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
    /// 지정된 경로의 텍스트 Asset을 읽어 각 라인을 리스트의 요소로 반환합니다.
    /// </summary>
    /// <param name="filePath">읽어들일 텍스트 Asset의 경로입니다.</param>
    /// <param name="includeBlackLines">빈 줄을 리스트에 포함할지 여부를 결정합니다. 기본값은 true입니다.</param>
    /// <returns>텍스트 Asset의 각 라인을 요소로 하는 문자열 리스트입니다.</returns>
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
    /// 텍스트 Asset을 읽어서 리스트 형태로 반환하는 메서드입니다.
    /// </summary>
    /// <param name="asset">읽을 TextAsset입니다.</param>
    /// <param name="includeBlackLines">빈 줄을 포함할지 결정하는 플래그입니다. 기본값은 true입니다.</param>
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