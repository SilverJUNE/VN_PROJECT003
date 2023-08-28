using CHARACTERS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestCharacterIndex : MonoBehaviour
    {
        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test());
        }
        IEnumerator Test()
        {
            Character_Sprite Guard1 = CreateCharacter("Guard1 as Generic") as Character_Sprite;
            Character_Sprite Guard2 = CreateCharacter("Guard2 as Generic") as Character_Sprite;
            Character_Sprite 크리스틴 = CreateCharacter("크리스틴") as Character_Sprite;
            Character_Sprite 하영 = CreateCharacter("하영") as Character_Sprite;

            Guard2.SetColor(Color.red);

            크리스틴 .SetPosition(new Vector2(0.3f, 0));
            하영     .SetPosition(new Vector2(0.45f, 0));
            Guard1   .SetPosition(new Vector2(0.6f, 0));
            Guard2   .SetPosition(new Vector2(0.75f, 0));
            
            Guard2.SetPriority(1000);
            하영.SetPriority(15);
            크리스틴.SetPriority(8);
            Guard1.SetPriority(30);

            yield return new WaitForSeconds(1);

            CharacterManager.instance.SortCharacters(new string[] {"하영", "크리스틴" });

            yield return new WaitForSeconds(1f);

            CharacterManager.instance.SortCharacters();

            yield return new WaitForSeconds(1);

            CharacterManager.instance.SortCharacters(new string[] { "크리스틴", "Guard2", "Guard1", "하영"});

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}