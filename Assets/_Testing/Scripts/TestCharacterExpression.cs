using CHARACTERS;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

namespace TESTING
{
    public class TestCharacterExpression : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            //Character 하영 = CharacterManager.instance.CreateCharacter("하영");
            //Character 다른이름으로바꿔도됨 = CharacterManager.instance.CreateCharacter("크리스틴");
            //Character 주인공 = CharacterManager.instance.CreateCharacter("여기는 Character Configuration Asset 있는 그대로 둬야함");
            StartCoroutine(Test_14());
        }

        IEnumerator Test_1()
        {
            Character_Sprite 임시캐릭터 = CreateCharacter("임시크리스") as Character_Sprite;
           // Character guard1 = CreateCharacter("Guard1 as Generic");
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;


            //guard1.SetPosition(Vector2.zero);
            임시캐릭터.SetPosition(Vector2.zero);
            하영.SetPosition(new Vector2(0.5f, 0.5f));
            크리스틴.SetPosition(Vector2.one);

            //guard1.Show();
            크리스틴.Show();
            임시캐릭터.Show();

            Sprite bodySprite = 임시캐릭터.GetSprite("02");
            Sprite faceSprite = 임시캐릭터.GetSprite("19");

            임시캐릭터.SetSprite(bodySprite, 0);
            임시캐릭터.SetSprite(faceSprite, 1);

            bodySprite = 임시캐릭터.GetSprite("01");
            faceSprite = 임시캐릭터.GetSprite("10");

            yield return new WaitForSeconds(2f);

            임시캐릭터.SetSprite(bodySprite, 0);
            임시캐릭터.SetSprite(faceSprite, 1);

            yield return 하영.Show();

            yield return 하영.MovePositionWithEase(new Vector2(0, 0.5f), 1f, easeType: Ease.InOutQuad);
            yield return 하영.MovePositionWithEase(new Vector2(1f, 0.5f), 2f, easeType: Ease.InSine);

            임시캐릭터.SetNameFont(tempFont);
            임시캐릭터.SetDialogueFont(tempFont);
            하영.SetDialogueColor(Color.cyan);
            크리스틴.SetNameColor(Color.red);

            yield return 임시캐릭터.Say("여기 앞은 갈 수 없다.");
            yield return 하영.Say("호기심은 명만 제촉할 뿐이다.");
            yield return 크리스틴.Say("다만, 호랑이 가죽이 있다면 예외다!");

            yield return null;

        }

        IEnumerator Test_2()
        {
            Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;

            yield return new WaitForSeconds(1f);

            Sprite s1 = Guard.GetSprite("Characters-Monk"); // 나중에 변형 시켜서 Guard.GetSprite("tex:Characters sprite:Monk"); 로 변형시켜야 함(개인적인 취향)
            Guard.TransitionSprite(s1);

            Debug.Log($"Visible = {Guard.isVisible}");

            yield return null;
        }

        IEnumerator Test_3()
        {
            //Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character_Sprite 임시캐릭터_stella = CreateCharacter("임시크리스") as Character_Sprite;
            임시캐릭터_stella.isVisible = false;

            //Character_Sprite 하영               = CreateCharacter("하영") as Character_Sprite;

            yield return new WaitForSeconds(1f);

            Sprite bodySprite = 크리스틴.GetSprite("2"); // 나중에 변형 시켜서 Guard.GetSprite("tex:Characters sprite:Monk"); 로 변형시켜야 함(개인적인 취향)
            Sprite faceSprite = 크리스틴.GetSprite("Raelin_6");
            크리스틴.TransitionSprite(bodySprite);
            yield return 크리스틴.TransitionSprite(faceSprite, 1, 0.3f);

            yield return new WaitForSeconds(5f);

            크리스틴.MovePositionWithEase(Vector2.zero, 2f, DG.Tweening.Ease.InBounce);
            임시캐릭터_stella.Show();
            yield return 임시캐릭터_stella.MovePositionWithEase(new Vector2(1, 0), 2f, DG.Tweening.Ease.InExpo);

            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_19"), layer: 1);

            bodySprite = 임시캐릭터_stella.GetSprite("03");
            faceSprite = 임시캐릭터_stella.GetSprite("19");
            임시캐릭터_stella.TransitionSprite(bodySprite);
            임시캐릭터_stella.TransitionSprite(faceSprite, layer: 1);

            yield return null;

        }

        IEnumerator Test_4()
        {
            //Character_Sprite Guard = CreateCharacter("Guard as Generic") as Character_Sprite;
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            //Character_Sprite 임시캐릭터_stella = CreateCharacter("임시크리스") as Character_Sprite;
            //임시캐릭터_stella.isVisible = false;

            //Character_Sprite 하영               = CreateCharacter("하영") as Character_Sprite;

            yield return new WaitForSeconds(1f);

            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_19"), 1);
            yield return 크리스틴.TransitionSprite(크리스틴.GetSprite("2"));

            yield return null;
        }

        IEnumerator Test_5()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            // Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;

            크리스틴.SetPosition(new Vector2(0, 0));
            //Mao.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(1);

            // Mao.SetMotion("Healing Heart");

            yield return new WaitForSeconds(1);

            // Mao.SetMotion("Proud");

            yield return new WaitForSeconds(1);

            // Mao.SetMotion("Bounce");

            yield return null;
        }

        IEnumerator Test_6()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            //  Character_Live2D Natori = CreateCharacter("Natori") as Character_Live2D;

            크리스틴.SetPosition(new Vector2(0, 0));
            // Natori.SetPosition(new Vector2(1, 0));


            yield return null;

        }

        IEnumerator Test_7()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            //  Character_Live2D Natori = CreateCharacter("Natori") as Character_Live2D;

            크리스틴.SetPosition(new Vector2(0, 0));
            // Natori.SetPosition(new Vector2(1, 0));


            yield return null;
        }

        IEnumerator Test_8()
        {
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;
            // Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;

            하영.SetPosition(Vector2.zero);
            // Mao.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(0.3f);

            하영.Hide();
            // Mao.Hide();

            yield return new WaitForSeconds(0.5f);

            하영.Show();
            //  Mao.Show();
        }

        IEnumerator Test_9()
        {
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;
            //  Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;

            하영.SetPosition(Vector2.zero);
            //  Mao.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(1f);

            하영.TransitionColor(Color.red);
            //  Mao.TransitionColor(Color.red);

            yield return new WaitForSeconds(1f);

            하영.TransitionColor(Color.white);
            //  Mao.TransitionColor(Color.white);
        }

        IEnumerator Test_10()
        {
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;
            //  Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;

            하영.SetPosition(Vector2.zero);
            //  Mao.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(1f);

            하영.TransitionColor(Color.red, 0.1f);
            //  Mao.TransitionColor(Color.red, 0.1f);

            yield return new WaitForSeconds(2f);

            하영.UnHighlight();
            //  Mao.UnHighlight();

            yield return new WaitForSeconds(2f);

            하영.Highlight();
            //  Mao.Highlight();
        }

        IEnumerator Test_11()
        {
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;
            //  Character_Live2D Mao = CreateCharacter("Mao") as Character_Live2D;

            하영.SetPosition(Vector2.zero);
            //  Mao.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(1f);

            하영.FaceRight();
            // Mao.FaceRight();

            yield return new WaitForSeconds(1f);

            하영.Flip();
            // Mao.Flip();
        }

        IEnumerator Test_12()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;

            yield return new WaitForSeconds(1f);

            yield return 크리스틴.UnHighlight();

            yield return new WaitForSeconds(1f);

            yield return 크리스틴.TransitionColor(Color.red);

            yield return new WaitForSeconds(1f);

            yield return 크리스틴.Highlight();

            yield return new WaitForSeconds(1f);

            yield return 크리스틴.TransitionColor(Color.white);

            yield return null;
        }

        IEnumerator Test_13()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;

            크리스틴.SetPosition(Vector2.zero);
            하영.SetPosition(new Vector2(1, 0));

            yield return new WaitForSeconds(1f);

            yield return 하영.Flip(0.3f);

            yield return 크리스틴.FaceRight(immediate: true);

            yield return 크리스틴.FaceLeft(immediate: true);

            크리스틴.UnHighlight();
            yield return 하영.Say("지금부터 모든 명령 체계는 국장인 나 하영의 명령을 따르도록 한다.");

            하영.UnHighlight();
            크리스틴.Highlight();
            yield return 크리스틴.Say("대체 어느 부서의 국장인건데 여기서 명령질이시죠?");

            하영.Highlight();
            크리스틴.UnHighlight();
            yield return 하영.Say("흥, 따르지 않는 사람들은 {a} 모두 엄벌에 처한다!!");

            하영.UnHighlight();
            크리스틴.Highlight();
            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_13"), 1);
            yield return 크리스틴.Say("자...잠깐만!! 초...총은!!");

            크리스틴.TransitionColor(Color.red);

            yield return null;
        }

        IEnumerator Test_14()
        {
            Character Monk = CreateCharacter("Monk as Generic");

            yield return Monk.Say("Normal dialogue configuration");

            Monk.SetDialogueColor(Color.red);
            Monk.SetNameColor(Color.blue);

            yield return Monk.Say("Customized dialogue here");

            Monk.ResetConfigurationData();

            yield return Monk.Say("I should be back to normal");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}