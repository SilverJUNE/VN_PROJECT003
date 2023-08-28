using AUDIO;
using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Testing
{
    public class TestAudio : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Running_3());
        }

        Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        IEnumerator Running_1()
        {
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character Me = CreateCharacter("Me");
            크리스틴.Show();

            yield return new WaitForSeconds(0.5f);

            AudioManager.instance.PlaySoundEffect("Audio/SFX/RadioStatic", loop: true);
            AudioManager.instance.PlayVoice("Audio/Voices/exclamation", loop: true);

            yield return new WaitForSeconds(1f);
            크리스틴.Animate("Hop");
            크리스틴.TransitionSprite(크리스틴.GetSprite("1"));
            크리스틴.TransitionSprite(크리스틴.GetSprite("Raelin_8"), 1);
            크리스틴.Say("이 시리즈 더 내놔!!!");

            yield return Me.Say("일단 저 라디오를 꺼야해!!");


            
            yield return new WaitForSeconds(5f);

            크리스틴.Say("알았어!");

            AudioManager.instance.StopSoundEffect("RadioStatic");
            AudioManager.instance.StopSoundEffect("exclamation");
        }

        IEnumerator Running_2()
        {
            yield return new WaitForSeconds(1);

            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;

            크리스틴.Show();

            yield return DialogueSystem.instance.Say("네레이션", "네 배가 어딨는지 알 수 있을까?");

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/5");
            AudioManager.instance.PlayTrack("Audio/Music/Calm", volumeCap: 0.5f);
            AudioManager.instance.PlayVoice("Audio/Voices/exclamation");

            크리스틴.SetSprite(크리스틴.GetSprite("1"), 0);
            크리스틴.SetSprite(크리스틴.GetSprite("Raelin_8"), 1);
            크리스틴.MovePosition(new Vector2(0.7f, 0), speed: 0.5f);
            yield return 크리스틴.Say("예, 당연하죠!");

            yield return 크리스틴.Say("이제 엔진 룸을 소개하죠");

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/EngineRoom");
            AudioManager.instance.PlayTrack("Audio/Music/Calm2", volumeCap: 0.8f);

            yield return null;
        }

        IEnumerator Running_3()
        {
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;
            Character 주인공 = CreateCharacter("주인공");
            하영.Show();

            GraphicPanelManager.instance.GetPanel("background").GetLayer(0, true).SetTexture("Graphics/BG Images/villagenight");

            AudioManager.instance.PlayTrack("Audio/Ambience/RainyMood", 0);
            AudioManager.instance.PlayTrack("Audio/Music/Calm", 1, pitch: 0.7f);

            yield return 하영.Say("우리는 여러 채널을 가진 노래가 가능해요!");

            AudioManager.instance.StopTrack(1);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}