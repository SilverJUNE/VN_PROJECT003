using CHARACTERS;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TESTING
{
    public class TestCharacterMoves : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            //Character 하영 = CharacterManager.instance.CreateCharacter("하영");
            //Character 다른이름으로바꿔도됨 = CharacterManager.instance.CreateCharacter("크리스틴");
            //Character 주인공 = CharacterManager.instance.CreateCharacter("여기는 Character Configuration Asset 있는 그대로 둬야함");
            StartCoroutine(Test());
        }
        IEnumerator Test()
        {
            Character guard1 = CreateCharacter("Guard1 as Generic");
            Character 하영 = CreateCharacter("하영");
            Character 크리스틴 = CreateCharacter("크리스틴");

            guard1.SetPosition(Vector2.zero);
            하영.SetPosition(new Vector2(0.5f,0.5f));
            크리스틴.SetPosition(Vector2.one);

            크리스틴.Show();

            yield return guard1.Show();

            yield return guard1.MovePosition(Vector2.one, smooth: true);
            yield return guard1.MovePosition(Vector2.zero);

            yield return 하영.Show();

            yield return 하영.MovePositionWithEase(new Vector2(0, 0.5f), 1f, easeType: Ease.InOutQuad);
            yield return 하영.MovePositionWithEase(new Vector2(1f, 0.5f), 2f, easeType: Ease.InSine);

            guard1.SetNameFont(tempFont);
            guard1.SetDialogueFont(tempFont);
            하영.SetDialogueColor(Color.cyan);
            크리스틴.SetNameColor(Color.red);

            yield return guard1.Say("여기 앞은 갈 수 없다.");
            yield return 하영.Say("호기심은 명만 제촉할 뿐이다.");
            yield return 크리스틴.Say("다만, 호랑이 가죽이 있다면 예외다!");

            yield return null;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}