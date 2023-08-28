using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using TMPro;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test_1());
        }

        IEnumerator Test_1()
        {
            Character 주인공 = CharacterManager.instance.CreateCharacter("주인공");
            Character 하영 = CharacterManager.instance.CreateCharacter("하영");
            Character 크리스 = CharacterManager.instance.CreateCharacter("크리스");

            List<string> lines = new List<string>()
            {
                "안녕하시지~!",
                "My name is Elen.",
                "What's your name?",
                "Oh,{wa 1} that's very nice."
            };
            yield return 주인공.Say(lines);

            lines = new List<string>()
            {
                "난, 하영입니다.",
                "더 많은 이야기를 나누기 싶다면{c} 여기로 오도록."
            };

            yield return 하영.Say(lines);

            yield return 크리스.Say("이건 내가 원하는 이름이 아니었어...{a} 그래도 최선을 다할 것을 약속하지");

            Debug.Log("Finished");
        }

        IEnumerator Test_2()
        {
            Character 주인공 = CharacterManager.instance.CreateCharacter("주인공");
            Character 하영 = CharacterManager.instance.CreateCharacter("하영");
            Character 크리스 = CharacterManager.instance.CreateCharacter("크리스");

            List<string> lines = new List<string>()
            {
                "안녕하시지~!",
                "My name is Elen.",
                "What's your name?",
                "Oh,{wa 1} that's very nice."
            };
            yield return 주인공.Say(lines);

            lines = new List<string>()
            {
                "난, 하영입니다.",
                "더 많은 이야기를 나누기 싶다면{c} 여기로 오도록."
            };

            yield return 하영.Say(lines);

            하영.SetNameColor(Color.blue);
            하영.SetDialogueColor(Color.green);
            하영.SetNameFont(tempFont);
            하영.SetDialogueFont(tempFont);

            yield return 하영.Say(lines);

            하영.ResetConfigurationData();

            yield return 하영.Say(lines);

            yield return 크리스.Say("이건 내가 원하는 이름이 아니었어...{a} 그래도 최선을 다할 것을 약속하지");

            Debug.Log("Finished");
        }

        IEnumerator Test_3()
        {
            Character guard1 = CreateCharacter("Guard1 as Generic");
            Character guard2 = CreateCharacter("Guard2 as Generic");
            Character guard3 = CreateCharacter("Guard3 as Generic");

            guard1.Show();
            guard2.Show();
            guard3.Show();

            guard1.SetNameFont(tempFont);
            guard1.SetDialogueFont(tempFont);
            guard2.SetDialogueColor(Color.cyan);
            guard3.SetNameColor(Color.red);

            yield return guard1.Say("여기 앞은 갈 수 없다.");
            yield return guard2.Say("호기심은 명만 제촉할 뿐이다.");
            yield return guard3.Say("다만, 호랑이 가죽이 있다면 예외다!");

            yield return null;

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}