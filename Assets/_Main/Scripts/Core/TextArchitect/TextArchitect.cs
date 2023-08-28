using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextArchitect Ŭ������ UI Ȥ�� World Space���� �ؽ�Ʈ�� ǥ���ϰ� �ִϸ��̼��� ó���ϴ� ����� �����մϴ�.
/// </summary>
public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro     tmpro_world;

    /// <summary>
    /// UI Ȥ�� World Space���� ����� TMP_Text ��ü�� ��ȯ�մϴ�.
    /// tmpro_ui�� null�� �ƴϸ� tmpro_ui�� ��ȯ�ϰ�, �׷��� ������ tmpro_world�� ��ȯ�մϴ�.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    // ���� �ؽ�Ʈ�� �����ɴϴ�.
    public string currentText => tmpro.text;

    // Ÿ�� �ؽ�Ʈ�� �����ϰ� �����ɴϴ�.
    public string targetText { get; private set; }  = "";

    // preText : ���� �ؽ�Ʈ�� �����ϰ� �����ɴϴ�.
    public string preText { get; private set; }     = "";

    // preTextLength : ���� �ؽ�Ʈ�� ���̸� �����մϴ�.
    private int preTextLength = 0;

    // ��ü Ÿ�� �ؽ�Ʈ�� �����ɴϴ�.
    public string fullTargetText => preText + targetText;

    // �ؽ�Ʈ ���� �޼��带 �����մϴ�. (�ν��Ͻ�, Ÿ�Զ�����, ���̵�)
    public enum BuildMethod { instant, typewriter, fade }
    // �⺻ �ؽ�Ʈ ���� �޼��带 �����մϴ�.
    public BuildMethod buildMethod = BuildMethod.typewriter;
    
    // �ؽ�Ʈ ������ �������� �����մϴ�.
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    // �ؽ�Ʈ�� �ӵ��� �������� �����մϴ�.
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    // �⺻ �ӵ��� �����մϴ�.
    private const float baseSpeed = 1;
    // �ӵ� ������ �����մϴ�.
    private float speedMultiplier = 1;

    // �� ����Ŭ�� ǥ���� ���� ���� �����մϴ�.
    public int charactersPerCycle { get { return speed < 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    // ���� ������ �����մϴ�.
    private int characterMultiplier = 1;

    // ���� ���θ� �����մϴ�.
    public bool hurryUp = false;

    /// <summary>
    /// TextArchitect�� �������Դϴ�. UI���� ����� TextMeshProUGUI ��ü�� �ʱ�ȭ�մϴ�.
    /// </summary>
    /// <param name="tmpro_ui">UI���� ����� TextMeshProUGUI ��ü�Դϴ�.</param>
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    /// <summary>
    /// TextArchitect�� �������Դϴ�. World Space���� ����� TextMeshPro ��ü�� �ʱ�ȭ�մϴ�.
    /// </summary>
    /// <param name="tmpro_world">World Space���� ����� TextMeshPro ��ü�Դϴ�.</param>
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// ���ο� ���ڿ��� Ÿ�� ���ڿ��� �����ϰ�, �ؽ�Ʈ�� ����� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <param name="text">���ο� Ÿ�� �ؽ�Ʈ</param>
    /// <returns>�ؽ�Ʈ ����� ������ �ڷ�ƾ</returns>
    public Coroutine Build(string text)
    {
        preText     = "";       // ���� �ؽ�Ʈ�� �ʱ�ȭ �մϴ�.
        targetText  = text;     // Ÿ�� �ؽ�Ʈ�� �����մϴ�.

        Stop();                 // ���� ���� ���� �ڷ�ƾ�� �ִٸ� ����ϴ�.

        // ���ο� �ؽ�Ʈ ���� �ڷ�ƾ�� �����ϰ� ������ buildProcess�� �����մϴ�.
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// ���� �ؽ�Ʈ �ڿ� �߰��� ���ڿ��� Ÿ�� �ؽ�Ʈ�� �����ϰ�, �ؽ�Ʈ�� ����� �ڷ�ƾ�� �����մϴ�.
    /// </summary>
    /// <param name="text">�߰��� Ÿ�� �ؽ�Ʈ</param>
    /// <returns>�ؽ�Ʈ ����� ������ �ڷ�ƾ</returns>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;   // ���� �ؽ�Ʈ�� ���� �ؽ�Ʈ�� �����մϴ�.
        targetText = text;      // Ÿ�� �ؽ�Ʈ�� �����մϴ�.

        Stop();                 // ���� ���� ���� �ڷ�ƾ�� �ִٸ� ����ϴ�.

        // ���ο� �ؽ�Ʈ �߰� �ڷ�ƾ�� �����ϰ� ������ buildProcess�� �����մϴ�.
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    // ���� ���� ���� �ؽ�Ʈ ����/�߰� �ڷ�ƾ�� �����մϴ�
    private Coroutine buildProcess = null;
    // �ؽ�Ʈ�� ���� ���� ������ ���θ� ��ȯ�մϴ�.
    public bool isBuilding => buildProcess != null;

    // ���� ���� ���� �ڷ�ƾ�� ���ߴ� �޼����Դϴ�.
    public void Stop()
    {
        if(!isBuilding)
            return;
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    // �ؽ�Ʈ ���� �ڷ�ƾ�Դϴ�.
    IEnumerator Building()
    {
        Prepare();  // �ؽ�Ʈ ������ �غ��մϴ�.

        // ������ ���� �޼��忡 ���� �ش� �ڷ�ƾ�� �����մϴ�.
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }
        OnComplete();  // �ؽ�Ʈ ������ �Ϸ�Ǹ� ����˴ϴ�.
    }

    // �ؽ�Ʈ ���� �Ϸ� �� ����Ǵ� �޼����Դϴ�.
    private void OnComplete()
    {
        buildProcess = null;    // �ڷ�ƾ ������ �ʱ�ȭ�մϴ�.
        hurryUp = false;        // ���� ���¸� �����մϴ�.
    }

    // �ؽ�Ʈ ������ ������ �Ϸ��ϴ� �޼����Դϴ�.
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;     // ȭ�鿡 ǥ���� �� �ִ� �ִ� ���� ���� �����մϴ�. �� ��� ��� ���ڰ� ǥ�õǵ��� �����մϴ�.
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();                                        // �ؽ�Ʈ �޽ø� ������ ������Ʈ�մϴ�.
                break;
        }

        Stop();             // ���� ���� ���� �ڷ�ƾ�� ����ϴ�.
        OnComplete();       // �ؽ�Ʈ ���� �Ϸ� ��ŵ�ϴ�.
    }

    // �ؽ�Ʈ ������ �غ��ϴ� �޼����Դϴ�. ������ ���� �޼��忡 ���� �ش� �غ� �Լ��� ȣ���մϴ�.
    private void Prepare()
    {
        switch (buildMethod) 
        { 
            case BuildMethod.instant:
                Prepare_Instant(); 
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
             case BuildMethod.fade: 
                Prepare_Fade();
                break;
        }
    }

    // �ν���Ʈ �ؽ�Ʈ ������ �غ��ϴ� �޼����Դϴ�
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;   // �ؽ�Ʈ�� ������ �����մϴ�.
        tmpro.text = fullTargetText; // ��ü Ÿ�� �ؽ�Ʈ�� �����մϴ�.
        tmpro.ForceMeshUpdate();     // �ؽ�Ʈ �޽ø� ������ ������Ʈ�մϴ�.
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;  // ��� ���ڸ� ǥ���ϵ��� �����մϴ�.
    }

    // Ÿ�Զ����� ����� �ؽ�Ʈ ������ �غ��ϴ� �޼����Դϴ�.
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;   // �ؽ�Ʈ�� ������ �����մϴ�.
        tmpro.maxVisibleCharacters = 0;  // �ʱ⿡�� �ƹ� ���ڵ� ǥ������ �ʽ��ϴ�.
        tmpro.text = preText;        // ���� �ؽ�Ʈ�� �����մϴ�.

        if (preText != "")
        {
            tmpro.ForceMeshUpdate();  // �ؽ�Ʈ �޽ø� ������ ������Ʈ�մϴ�.
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;  // ���� �ؽ�Ʈ�� ���� ����ŭ ǥ���ϵ��� �����մϴ�.
        }

        tmpro.text += targetText;    // Ÿ�� �ؽ�Ʈ�� �߰��մϴ�.
        tmpro.ForceMeshUpdate();     // �ؽ�Ʈ �޽ø� ������ ������Ʈ�մϴ�.
    }

    // ���̵� ����� �ؽ�Ʈ ������ �غ��ϴ� �޼����Դϴ�.
    private void Prepare_Fade()
    {
        tmpro.text = preText;  // ���� �ؽ�Ʈ�� ���� �ؽ�Ʈ�� �����Դϴ�.
        if (preText != "")     // ���� �ؽ�Ʈ�� ���� ���,
        {
            tmpro.ForceMeshUpdate();                            // �ؽ�Ʈ�� �޽ø� ������ ������Ʈ�մϴ�.
            preTextLength = tmpro.textInfo.characterCount;      // ���� �ؽ�Ʈ�� ���� ������ �����մϴ�.
        }
        else
            preTextLength = 0;                                  // ���� �ؽ�Ʈ�� ���� ���, ���̸� 0���� �����մϴ�.

        tmpro.text += targetText;                               // Ÿ�� �ؽ�Ʈ�� ���� �ؽ�Ʈ�� �����Դϴ�.
        tmpro.maxVisibleCharacters = int.MaxValue;              // ������ �� �ִ� �ִ� ���� ���� �����մϴ�. (�� ��� ��� ���ڸ� ������ �� �ֽ��ϴ�.)
        tmpro.ForceMeshUpdate();                                // �ؽ�Ʈ�� �޽ø� ������ ������Ʈ�մϴ�.

        TMP_TextInfo textInfo = tmpro.textInfo;                 // ���� �ؽ�Ʈ�� ������ �����ɴϴ�.

        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);  // ���̴� ������ �����մϴ�.
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);   // ������ ������ �����մϴ�.

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;  // ������ ���� ������ �����ɴϴ�.

        for (int i = 0; i < textInfo.characterCount; i++)  // ��� ���ڿ� ����
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];  // ������ ������ �����ɴϴ�.

            if (!charInfo.isVisible)  // ���ڰ� ������ ���� ���, ���� ���ڷ� �Ѿ�ϴ�.
                continue;

            if (i < preTextLength)  // ���� �ؽ�Ʈ�� ������ ���,
            {
                for (int v = 0; v < 4; v++)  // �� ������ ��� ���ؽ��� ����
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;  // ���̴� ������ �����մϴ�.
            }
            else  // Ÿ�� �ؽ�Ʈ�� ������ ���,
            {
                for (int v = 0; v < 4; v++)  // �� ������ ��� ���ؽ��� ����
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;  // ������ ������ �����մϴ�.
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);  // ���� ������ ������Ʈ�մϴ�.
    }

    // Ÿ�Զ����� ����� �ؽ�Ʈ ���� �ڷ�ƾ�Դϴ�
    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    // ���̵� ����� �ؽ�Ʈ ���� �ڷ�ƾ�Դϴ�.
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;  // ������ �ּҰ��� ���� �ؽ�Ʈ�� ���̷� �����մϴ�.
        int maxRange = preTextLength;  // ���� ó�� ������ �ִ밪�� ���� �ؽ�Ʈ�� ���̷� �����մϴ�.

        byte alphaTreshold = 15;  // ���İ��� �Ӱ�ġ�� �����մϴ�. (�� �� �̻��� �� �ؽ�Ʈ�� ���̱� �����մϴ�.)

        TMP_TextInfo textInfo = tmpro.textInfo;  // �ؽ�Ʈ�� ������ �����ɴϴ�.

        // ������ ���� ������ �����ɴϴ�.
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];  // �� ������ ���İ��� �����ϴ� �迭�� �����մϴ�.

        // ���̵� ȿ���� ��� �����մϴ�.
        while (true)
        {
            // ���̵� �ӵ��� �����մϴ�. (�ؽ�Ʈ�� ��Ÿ���� �ӵ��� �����մϴ�.)
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 4f;

            // ó�� ���� ���� ��� ���ڿ� ����
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];  // ������ ������ �����ɴϴ�.

                // ���ڰ� ������ ���� ���, ���� ���ڷ� �Ѿ�ϴ�.
                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // ������ ���İ��� ������ŵ�ϴ�.
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                // �� ������ ��� ���ؽ��� ����
                for (int v = 0; v < 4; v++)
                    // ���İ��� �����մϴ�.
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                // ���İ��� �ִ밪(255)�� �����ϸ� ������ �ּҰ��� ������ŵ�ϴ�.
                if (alphas[i] >= 255)
                    minRange++;
            }

            // ���� ������ ������Ʈ�մϴ�.
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // ������ ���ڰ� ������ �ʰų� ���İ��� �Ӱ�ġ �̻��� ���
            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1]>alphaTreshold || lastCharacterIsInvisible)
            {
                // ó�� ������ �ִ밪�� ������Ű�ų� ���̵� ȿ���� �����մϴ�.
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                // ��� ������ ���İ��� �ִ밪(255)�� �����ϰų� ������ ���ڰ� ������ �ʴ� ���, ���̵� ȿ���� �����մϴ�.
                else if (alphas[maxRange - 1] > 255 || lastCharacterIsInvisible)
                    break;
            }
            // ���� �����ӱ��� ����մϴ�. (�����Ӹ��� ���̵� ȿ���� ������Ʈ�մϴ�.)
            yield return new WaitForEndOfFrame();
        }
    }

}
