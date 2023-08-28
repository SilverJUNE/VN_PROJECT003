using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// TextArchitect 클래스는 UI 혹은 World Space에서 텍스트를 표시하고 애니메이션을 처리하는 기능을 제공합니다.
/// </summary>
public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro     tmpro_world;

    /// <summary>
    /// UI 혹은 World Space에서 사용할 TMP_Text 객체를 반환합니다.
    /// tmpro_ui가 null이 아니면 tmpro_ui를 반환하고, 그렇지 않으면 tmpro_world를 반환합니다.
    /// </summary>
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;

    // 현재 텍스트를 가져옵니다.
    public string currentText => tmpro.text;

    // 타겟 텍스트를 정의하고 가져옵니다.
    public string targetText { get; private set; }  = "";

    // preText : 이전 텍스트를 정의하고 가져옵니다.
    public string preText { get; private set; }     = "";

    // preTextLength : 이전 텍스트의 길이를 정의합니다.
    private int preTextLength = 0;

    // 전체 타겟 텍스트를 가져옵니다.
    public string fullTargetText => preText + targetText;

    // 텍스트 빌드 메서드를 정의합니다. (인스턴스, 타입라이터, 페이드)
    public enum BuildMethod { instant, typewriter, fade }
    // 기본 텍스트 빌드 메서드를 설정합니다.
    public BuildMethod buildMethod = BuildMethod.typewriter;
    
    // 텍스트 색상을 가져오고 설정합니다.
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    // 텍스트의 속도를 가져오고 설정합니다.
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    // 기본 속도를 정의합니다.
    private const float baseSpeed = 1;
    // 속도 배율을 정의합니다.
    private float speedMultiplier = 1;

    // 한 사이클에 표시할 문자 수를 정의합니다.
    public int charactersPerCycle { get { return speed < 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    // 문자 배율을 정의합니다.
    private int characterMultiplier = 1;

    // 가속 여부를 정의합니다.
    public bool hurryUp = false;

    /// <summary>
    /// TextArchitect의 생성자입니다. UI에서 사용할 TextMeshProUGUI 객체를 초기화합니다.
    /// </summary>
    /// <param name="tmpro_ui">UI에서 사용할 TextMeshProUGUI 객체입니다.</param>
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    /// <summary>
    /// TextArchitect의 생성자입니다. World Space에서 사용할 TextMeshPro 객체를 초기화합니다.
    /// </summary>
    /// <param name="tmpro_world">World Space에서 사용할 TextMeshPro 객체입니다.</param>
    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// 새로운 문자열을 타겟 문자열로 설정하고, 텍스트를 만드는 코루틴을 시작합니다.
    /// </summary>
    /// <param name="text">새로운 타겟 텍스트</param>
    /// <returns>텍스트 만드는 과정의 코루틴</returns>
    public Coroutine Build(string text)
    {
        preText     = "";       // 이전 텍스트를 초기화 합니다.
        targetText  = text;     // 타겟 텍스트를 설정합니다.

        Stop();                 // 현재 실행 중인 코루틴이 있다면 멈춥니다.

        // 새로운 텍스트 구축 코루틴을 시작하고 참조를 buildProcess에 저장합니다.
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// 현재 텍스트 뒤에 추가할 문자열을 타겟 텍스트로 설정하고, 텍스트를 만드는 코루틴을 시작합니다.
    /// </summary>
    /// <param name="text">추가할 타겟 텍스트</param>
    /// <returns>텍스트 만드는 과정의 코루틴</returns>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;   // 현재 텍스트를 이전 텍스트로 설정합니다.
        targetText = text;      // 타겟 텍스트를 설정합니다.

        Stop();                 // 현재 실행 중인 코루틴이 있다면 멈춥니다.

        // 새로운 텍스트 추가 코루틴을 시작하고 참조를 buildProcess에 저장합니다.
        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    // 현재 실행 중인 텍스트 구축/추가 코루틴을 참조합니다
    private Coroutine buildProcess = null;
    // 텍스트가 현재 구축 중인지 여부를 반환합니다.
    public bool isBuilding => buildProcess != null;

    // 현재 실행 중인 코루틴을 멈추는 메서드입니다.
    public void Stop()
    {
        if(!isBuilding)
            return;
        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    // 텍스트 구축 코루틴입니다.
    IEnumerator Building()
    {
        Prepare();  // 텍스트 구축을 준비합니다.

        // 선택한 빌드 메서드에 따라 해당 코루틴을 실행합니다.
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.fade:
                yield return Build_Fade();
                break;
        }
        OnComplete();  // 텍스트 구축이 완료되면 실행됩니다.
    }

    // 텍스트 구축 완료 시 실행되는 메서드입니다.
    private void OnComplete()
    {
        buildProcess = null;    // 코루틴 참조를 초기화합니다.
        hurryUp = false;        // 가속 상태를 해제합니다.
    }

    // 텍스트 구축을 강제로 완료하는 메서드입니다.
    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;     // 화면에 표시할 수 있는 최대 문자 수를 설정합니다. 이 경우 모든 문자가 표시되도록 설정합니다.
                break;
            case BuildMethod.fade:
                tmpro.ForceMeshUpdate();                                        // 텍스트 메시를 강제로 업데이트합니다.
                break;
        }

        Stop();             // 현재 실행 중인 코루틴을 멈춥니다.
        OnComplete();       // 텍스트 구축 완료 시킵니다.
    }

    // 텍스트 구축을 준비하는 메서드입니다. 선택한 빌드 메서드에 따라 해당 준비 함수를 호출합니다.
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

    // 인스턴트 텍스트 구축을 준비하는 메서드입니다
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;   // 텍스트의 색상을 설정합니다.
        tmpro.text = fullTargetText; // 전체 타겟 텍스트를 설정합니다.
        tmpro.ForceMeshUpdate();     // 텍스트 메시를 강제로 업데이트합니다.
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;  // 모든 문자를 표시하도록 설정합니다.
    }

    // 타입라이터 방식의 텍스트 구축을 준비하는 메서드입니다.
    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;   // 텍스트의 색상을 설정합니다.
        tmpro.maxVisibleCharacters = 0;  // 초기에는 아무 문자도 표시하지 않습니다.
        tmpro.text = preText;        // 이전 텍스트를 설정합니다.

        if (preText != "")
        {
            tmpro.ForceMeshUpdate();  // 텍스트 메시를 강제로 업데이트합니다.
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;  // 이전 텍스트의 문자 수만큼 표시하도록 설정합니다.
        }

        tmpro.text += targetText;    // 타겟 텍스트를 추가합니다.
        tmpro.ForceMeshUpdate();     // 텍스트 메시를 강제로 업데이트합니다.
    }

    // 페이드 방식의 텍스트 구축을 준비하는 메서드입니다.
    private void Prepare_Fade()
    {
        tmpro.text = preText;  // 현재 텍스트에 이전 텍스트를 덧붙입니다.
        if (preText != "")     // 이전 텍스트가 있을 경우,
        {
            tmpro.ForceMeshUpdate();                            // 텍스트의 메시를 강제로 업데이트합니다.
            preTextLength = tmpro.textInfo.characterCount;      // 이전 텍스트의 문자 개수를 저장합니다.
        }
        else
            preTextLength = 0;                                  // 이전 텍스트가 없을 경우, 길이를 0으로 설정합니다.

        tmpro.text += targetText;                               // 타겟 텍스트를 현재 텍스트에 덧붙입니다.
        tmpro.maxVisibleCharacters = int.MaxValue;              // 보여줄 수 있는 최대 문자 수를 설정합니다. (이 경우 모든 문자를 보여줄 수 있습니다.)
        tmpro.ForceMeshUpdate();                                // 텍스트의 메시를 강제로 업데이트합니다.

        TMP_TextInfo textInfo = tmpro.textInfo;                 // 현재 텍스트의 정보를 가져옵니다.

        Color colorVisible = new Color(textColor.r, textColor.g, textColor.b, 1);  // 보이는 색상을 설정합니다.
        Color colorHidden = new Color(textColor.r, textColor.g, textColor.b, 0);   // 숨겨진 색상을 설정합니다.

        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;  // 문자의 색상 정보를 가져옵니다.

        for (int i = 0; i < textInfo.characterCount; i++)  // 모든 문자에 대해
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];  // 문자의 정보를 가져옵니다.

            if (!charInfo.isVisible)  // 문자가 보이지 않을 경우, 다음 문자로 넘어갑니다.
                continue;

            if (i < preTextLength)  // 이전 텍스트의 문자일 경우,
            {
                for (int v = 0; v < 4; v++)  // 각 문자의 모든 버텍스에 대해
                    vertexColors[charInfo.vertexIndex + v] = colorVisible;  // 보이는 색상을 설정합니다.
            }
            else  // 타겟 텍스트의 문자일 경우,
            {
                for (int v = 0; v < 4; v++)  // 각 문자의 모든 버텍스에 대해
                    vertexColors[charInfo.vertexIndex + v] = colorHidden;  // 숨겨진 색상을 설정합니다.
            }
        }
        tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);  // 색상 정보를 업데이트합니다.
    }

    // 타입라이터 방식의 텍스트 구축 코루틴입니다
    private IEnumerator Build_Typewriter()
    {
        while(tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / speed);
        }
    }

    // 페이드 방식의 텍스트 구축 코루틴입니다.
    private IEnumerator Build_Fade()
    {
        int minRange = preTextLength;  // 범위의 최소값을 이전 텍스트의 길이로 설정합니다.
        int maxRange = preTextLength;  // 현재 처리 범위의 최대값을 이전 텍스트의 길이로 설정합니다.

        byte alphaTreshold = 15;  // 알파값의 임계치를 설정합니다. (이 값 이상일 때 텍스트가 보이기 시작합니다.)

        TMP_TextInfo textInfo = tmpro.textInfo;  // 텍스트의 정보를 가져옵니다.

        // 문자의 색상 정보를 가져옵니다.
        Color32[] vertexColors = textInfo.meshInfo[textInfo.characterInfo[0].materialReferenceIndex].colors32;
        float[] alphas = new float[textInfo.characterCount];  // 각 문자의 알파값을 저장하는 배열을 생성합니다.

        // 페이드 효과를 계속 적용합니다.
        while (true)
        {
            // 페이드 속도를 설정합니다. (텍스트가 나타나는 속도를 조절합니다.)
            float fadeSpeed = ((hurryUp ? charactersPerCycle * 5 : charactersPerCycle) * speed) * 4f;

            // 처리 범위 내의 모든 문자에 대해
            for (int i = minRange; i < maxRange; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];  // 문자의 정보를 가져옵니다.

                // 문자가 보이지 않을 경우, 다음 문자로 넘어갑니다.
                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                // 문자의 알파값을 증가시킵니다.
                alphas[i] = Mathf.MoveTowards(alphas[i], 255, fadeSpeed);

                // 각 문자의 모든 버텍스에 대해
                for (int v = 0; v < 4; v++)
                    // 알파값을 설정합니다.
                    vertexColors[charInfo.vertexIndex + v].a = (byte)alphas[i];

                // 알파값이 최대값(255)에 도달하면 범위의 최소값을 증가시킵니다.
                if (alphas[i] >= 255)
                    minRange++;
            }

            // 색상 정보를 업데이트합니다.
            tmpro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            // 마지막 문자가 보이지 않거나 알파값이 임계치 이상인 경우
            bool lastCharacterIsInvisible = !textInfo.characterInfo[maxRange - 1].isVisible;
            if (alphas[maxRange - 1]>alphaTreshold || lastCharacterIsInvisible)
            {
                // 처리 범위의 최대값을 증가시키거나 페이드 효과를 종료합니다.
                if (maxRange < textInfo.characterCount)
                    maxRange++;
                // 모든 문자의 알파값이 최대값(255)에 도달하거나 마지막 문자가 보이지 않는 경우, 페이드 효과를 종료합니다.
                else if (alphas[maxRange - 1] > 255 || lastCharacterIsInvisible)
                    break;
            }
            // 다음 프레임까지 대기합니다. (프레임마다 페이드 효과를 업데이트합니다.)
            yield return new WaitForEndOfFrame();
        }
    }

}
