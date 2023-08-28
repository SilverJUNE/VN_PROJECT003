using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } = null;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController cg;

    public string lastInput { get; private set; } = string.Empty;

    public bool isWaitOnUserInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteractableState(active: false);   
       /*Ȯ�� ��ư ���� ���� ������ �ȳ����� �ϴ� �ڵ�
        acceptButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        */

        acceptButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        cg.Show();
        cg.SetInteractableState(active: true);
        isWaitOnUserInput = true;
    }
    
    public void Hide() 
    {
        cg.Hide();
        cg.SetInteractableState(active: false);
        isWaitOnUserInput=false;
    }

    public void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        lastInput = inputField.text;
        Hide();
    }


    /*Ȯ�� ��ư ���� ���� ������ �ȳ����� �ϴ� �ڵ�
    public void OnInputChanged(string value)
    {
        acceptButton.gameObject.SetActive(HasValidText());
    }
    
    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
    */


}
