using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Testing
{
    public class TestChoicePanel : MonoBehaviour
    {

        [SerializeField] private TextAsset fileToRead = null;
        ChoicePanel panel;

        // Start is called before the first frame update
        void Start()
        {
            panel = ChoicePanel.instance;
            StartCoroutine(Running());
        }

        IEnumerator Running()
        {
            string[] choices = new string[]
            {
                " ???와 같이간다.",
                " A와 같이 간다...",
                " 아무와도 같이 가지 않는다.",
                " 그냥 길게 적어서 이해시키자 결국 가오가 중요하지 이건 비주얼 노벨이니까 알아서 해보자 그래 화이팅"
            };

            panel.Show("누구와 함께 하시겠습니까?", choices);

            while (panel.isWaitingOnUserChoice)
                yield return null;

            var decision = panel.lastDecision;

            Debug.Log($"Made choice {decision.answerIndex} '{decision.choices[decision.answerIndex]}'");

        }

        private void StartChoiceSelection_1()
        {
            List<string> lines = FileManager.ReadTextAsset(fileToRead);
        }
    }
}