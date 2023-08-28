using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem  ds;
        TextArchitect   architect;

        public TextArchitect.BuildMethod bm = TextArchitect.BuildMethod.instant;

        string[] lines = new string[5]
        {
            "일단 한국어로 해봅니다.",
            "글자를 적는 것이 확인 되었으면 좋겠습니다.",
            "최대한 하루만에 모든 것을 끝낼 생각으로 하고 있습니다.",
            "이거 모르는 부분이 꽤 많아요.",
            "이걸 전부 마스터 하면 좋겠지만 필요한 부분만 빼가는 것도 중요한데..."
        };
        private void Start()
        {
                ds = DialogueSystem.instance;
                architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            //  architect.buildMethod = TextArchitect.BuildMethod.instant;
            //  architect.buildMethod = TextArchitect.BuildMethod.typewriter;
                architect.buildMethod = TextArchitect.BuildMethod.fade;
        }

        void Update()
        {
            if(bm != architect.buildMethod)
            {
                architect.buildMethod = bm;
                architect.Stop();
            }

            if (Input.GetKeyDown(KeyCode.S))
                architect.Stop();


            string longLine = "이건 졸라 긴 글입니다. 왜냐하면 긴 글이기 때문이죠 이 말이 무슨 말인지 아시죠? 결국에는 우리는 이걸 시험해야지 되는거에요.";
            if(Input.GetKeyDown(KeyCode.Space)) 
            {
                if (architect.isBuilding)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();
                }
                else
                    architect.Build(longLine);
                  //architect.Build(lines[Random.Range(0, lines.Length)]);
            }
            /* Just for Testing
             Don't worry-we will be switching out the old input system for the new input system in a later episode
             */
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(longLine);
              //architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }

}
