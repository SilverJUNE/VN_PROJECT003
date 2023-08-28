using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestCharacterAnimation : MonoBehaviour
    {
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character_Sprite 하영     = CreateCharacter("하영")     as Character_Sprite;

            크리스틴.SetPosition(new Vector2(1, 0));
            하영.SetPosition(new Vector2(0, 0));

            yield return new WaitForSeconds(1f);

            크리스틴.Animate("Hop");
            하영.UnHighlight();
            크리스틴.TransitionSprite(크리스틴.GetSprite("1"));
            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_9"), 1);
            yield return 크리스틴.Say("대체 왜 이렇게 추운거야?");

            하영.FaceRight(immediate: true);
            크리스틴.UnHighlight();
            하영.Highlight();
            하영.TransitionSprite(하영.GetSprite("3.웃는 표정2"));
            하영.MovePositionWithEase(new Vector2(0.1f, 0), 0.1f, DG.Tweening.Ease.Linear);
            하영.Animate("Shiver", true);
            yield return 하영.Say("모르지, 그래도 확실한 건 {a} 저기서 불어오는 바람 때문에 추워 죽겠어");

            하영.UnHighlight();
            크리스틴.Highlight();
            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_15"), 1); 
            yield return 크리스틴.Say("어 갑자기 바람이 멈춘것 같은데?");

            크리스틴.UnHighlight();
            하영.Highlight();
            하영.TransitionSprite(하영.GetSprite("1.갸우뚱 하는 표정"));
            하영.Animate("Shiver", false);
            yield return 하영.Say("다행이야. {c} 추워 죽는줄 알았다니까. 다음 부터는 옷이라도 두껍게 입고 와야겠다.");

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}