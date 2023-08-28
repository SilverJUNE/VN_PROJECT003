using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using CHARACTERS;

namespace TESTING
{   
    public class TestDialogueFiles : MonoBehaviour
    {
        [SerializeField] private TextAsset fileToRead = null;

        private void Start()
        {
            StartConversation_1();

            //StartCoroutine(Test_1());
        }

        private void StartConversation_1()
        {
            List<string> lines = FileManager.ReadTextAsset(fileToRead);

            DialogueSystem.instance.Say(lines);
        }

        private void StartConversation_2()
        {
            List<string> lines = FileManager.ReadTextAsset(fileToRead);

            /* 디버그 체킹
            foreach (string line in lines) 
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                Debug.Log($"Segmenting line '{line}'");
                DIALOGUE_LINE dlLine = DialogueParser.parse(line);

                int i = 0;
                foreach(DL_DIALOGUE_DATA.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
                {
                    Debug.Log($"segment [{i++}]='{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $" {segment.signalDelay}" : $"")}]");
                }
               
            }
            */

            DialogueSystem.instance.Say(lines);
        }

        private void StartConversation_3()
        {
            List<string> lines = FileManager.ReadTextAsset(fileToRead, false);

            /* 디버그 체킹
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                DIALOGUE_LINE dl = DialogueParser.Parse(line);

                Debug.Log($"{dl.speaker.name} as [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}]at {dl.speaker.castPosition}");

                List<(int l, string ex)> expr = dl.speaker.CastExpressions;
                for (int c = 0; c < expr.Count; c++)
                {
                    Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
                }
            }
            */

            DialogueSystem.instance.Say(lines);
        }

        IEnumerator Test_1()
        {
            Character 크리스틴 = CharacterManager.instance.CreateCharacter("크리스틴");

            yield return new WaitForSeconds(1f);

            yield return 크리스틴.Show();
            yield return new WaitForSeconds(1f);
            yield return 크리스틴.Hide();
            yield return new WaitForSeconds(1f);
            yield return 크리스틴.Show();

            yield return 크리스틴.Say("안녕~ 난 바빠서 먼저 간다.");

            yield return new WaitForSeconds(1f);
            yield return 크리스틴.Hide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                DialogueSystem.instance.dialogueContainer.Hide();
            
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                DialogueSystem.instance.dialogueContainer.Show();
        }





    }
}



