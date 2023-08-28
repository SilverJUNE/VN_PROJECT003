using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// CharacterManager 클래스는 다양한 타입의 캐릭터를 생성하고 관리합니다.
    /// </summary>
    public class CharacterManager : MonoBehaviour
    {
        /// <summary>
        /// CharacterManager의 Singleton 인스턴스입니다.
        /// 이 인스턴스는 게임 내에서 유일하게 존재하며, 다른 클래스에서 이 클래스의 메서드를 사용할 수 있게 합니다.
        /// </summary>
        public static CharacterManager instance { get; private set; }
        
        public Character[] allCharacters => characters.Values.ToArray();
        /// <summary>
        /// 캐릭터들의 이름과 해당 캐릭터 객체를 매핑하여 저장하는 딕셔너리입니다.
        /// 이를 통해 캐릭터의 이름으로 캐릭터 객체를 쉽게 찾을 수 있습니다.
        /// </summary>
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        /// <summary>
        /// 캐릭터 설정 정보를 가져옵니다. 이 설정 정보는 대사 시스템의 설정에서 가져옵니다.
        /// </summary>
        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        // 캐릭터의 경로와 이름을 정의하는 상수입니다.
        private const string CHARACTER_CASTING_ID = " as ";
        private const string CHARACTER_NAME_ID = "<charname>";          //Resource.Load("Characters/<charname>") become Resource.Load(("Characters/하영")
        public  string       characterRootPathFormat      => $"Characters/{CHARACTER_NAME_ID}";
        public  string       characterPrefabNameFormat    => $"Character - [{CHARACTER_NAME_ID}]";
        public  string       characterPrefabPathFormat    => $"{characterRootPathFormat}/{characterPrefabNameFormat}";

        // 캐릭터가 배치될 패널을 지정합니다.
        [SerializeField] private RectTransform _characterpanel = null;
        [SerializeField] private RectTransform _characterpanel_live2D = null;
        [SerializeField] private Transform _characterpanel_model3D = null;
        public RectTransform characterPanel => _characterpanel;
        public RectTransform characterPanelLive2D => _characterpanel_live2D;
        public Transform characterPanelModel3D => _characterpanel_model3D;

        /// <summary>
        /// 게임이 시작될 때 Singleton 인스턴스를 초기화합니다.
        /// </summary>
        private void Awake()
        {
            instance = this;
        }

        // 캐릭터 설정 데이터를 가져오는 메서드입니다.
        public CharacterConfigData GetCharacterConfig(string characterName, bool getOriginal = false)
        {
            if (!getOriginal)
            {
                Character character = GetCharacter(characterName);

                if (character != null)
                    return character.config;

            }

            return config.GetConfig(characterName);

        }

        // 캐릭터를 가져오는 메서드입니다. 만약 존재하지 않는다면 새로 생성합니다.
        public Character GetCharacter(string characterName, bool createIfDoesNotExist = false)
        {
            if (characters.ContainsKey(characterName.ToLower()))
                return characters[characterName.ToLower()];
            else if (createIfDoesNotExist)
                return CreateCharacter(characterName);

            return null;
        }

        public bool HasCharacter(string characterName) => characters.ContainsKey(characterName.ToLower());

        /// <summary>
        /// 캐릭터를 생성하는 메서드입니다. 만약 동일한 이름의 캐릭터가 이미 존재한다면, 경고 메시지를 출력하고 생성하지 않습니다.
        /// </summary>
        /// <param name="characterName">생성할 캐릭터의 이름입니다.</param>
        /// <returns>생성된 캐릭터 객체입니다. 이미 존재하는 이름이었다면 null을 반환합니다.</returns>
        public Character CreateCharacter(string characterName, bool revealAfterCreation = false)
        {
            if(characters.ContainsKey(characterName.ToLower())) 
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists. Did not create the character.");
                return null;
            }

            // 캐릭터 정보를 가져옵니다.
            CHARACTER_INFO info = GetCharacterInfo(characterName);

            // 캐릭터를 생성합니다.
            Character character = CreateCharacterFromInfo(info);

            // 생성된 캐릭터를 딕셔너리에 추가합니다.
            characters.Add(info.name.ToLower(), character);

            if (revealAfterCreation)
                character.Show();

            return character;
        }



        /// <summary>
        /// 주어진 캐릭터 이름에 해당하는 캐릭터 정보를 가져옵니다.
        /// </summary>
        /// <param name="characterName">정보를 가져올 캐릭터의 이름입니다.</param>
        /// <returns>캐릭터 정보입니다.</returns>
        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            string[] nameData = characterName.Split(CHARACTER_CASTING_ID, System.StringSplitOptions.RemoveEmptyEntries);
            result.name = nameData[0];
            result.castingName = nameData.Length > 1 ? nameData[1] : result.name;

            result.config = config.GetConfig(result.castingName);

            result.prefab = GetPrefabForCharacter(result.castingName);

            result.rootCharacterFolder = FormatCharacterPath(characterRootPathFormat, result.castingName);

            return result;
        }

        // 캐릭터의 프리팹을 가져오는 메서드입니다.
        private GameObject GetPrefabForCharacter(string characterName)
        {
            string prefabPath = FormatCharacterPath(characterPrefabPathFormat, characterName);
            return Resources.Load<GameObject>(prefabPath);
        }

        // 캐릭터의 경로를 정의하는 메서드입니다.
        public string FormatCharacterPath(string path, string characterName) => path.Replace(CHARACTER_NAME_ID, characterName);

        /// <summary>
        /// 캐릭터 정보를 기반으로 캐릭터를 생성합니다.
        /// </summary>
        /// <param name="info">캐릭터를 생성하기 위한 정보입니다.</param>
        /// <returns>생성된 캐릭터입니다.</returns>
        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            CharacterConfigData config = info.config;

            switch (info.config.characterType)
            {
                case Character.CharacterType.Text:
                    return new Character_Text(info.name, config);

                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new Character_Sprite(info.name, config, info.prefab, info.rootCharacterFolder);
                
                /*
                case Character.CharacterType.Live2D:
                    return new Character_Live2D(info.name, config, info.prefab, info.rootCharacterFolder);
                */

                case Character.CharacterType.Model3D:
                    return new Character_Model3D(info.name, config, info.prefab, info.rootCharacterFolder);

                default:
                    return null;
            }   
        }

        public void SortCharacters()
        {
            List<Character> activeCharacters = characters.Values.Where(c => c.root.gameObject.activeInHierarchy && c.isVisible).ToList();
            List<Character> inactiveCharacters = characters.Values.Except(activeCharacters).ToList();

            activeCharacters.Sort((a, b) => a.priority.CompareTo(b.priority));
            activeCharacters.Concat(inactiveCharacters);

            SortCharacter(activeCharacters);
        }

        public void SortCharacters(string[] characterNames)
        {
            List<Character> sortedCharacters = new List<Character>();

            sortedCharacters = characterNames
                .Select (name => GetCharacter(name))
                .Where  (character => character != null)
                .ToList ();

            List<Character> remainingCharacters = characters.Values
                .Except(sortedCharacters)
                .OrderBy(character => character.priority)
                .ToList();

            sortedCharacters.Reverse();

            int startingPriority = remainingCharacters.Count > 0 ? remainingCharacters.Max(c => c.priority) : 0;
            for (int i = 0; i < sortedCharacters.Count; i++)
            {
                Character character = sortedCharacters[i];
                character.SetPriority(startingPriority + i + 1, autoSortCharactersOnUi: false);
            }

            List<Character> allCharacters = remainingCharacters.Concat(sortedCharacters).ToList();
            SortCharacter(allCharacters);
            

        }

        private void SortCharacter(List<Character> charactersSortingOrder)
        {
            int i = 0;
            foreach (Character character in charactersSortingOrder)
            {
                Debug.Log($"{character.name} priority is {character.priority}");
                character.root.SetSiblingIndex(i++);
                character.OnSort(i);
            }
        }

        public int GetCharacterCountFromCharacterType(Character.CharacterType charType)
        {
            int count = 0;
            foreach(var c in characters.Values)
            {
                if (c.config.characterType == charType)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 캐릭터 정보를 저장하는 내부 클래스입니다.
        /// 이 클래스는 캐릭터를 생성하기 위해 필요한 정보를 보관합니다.
        /// </summary>
        private class CHARACTER_INFO
        {
            public string name = "";
            public string castingName = "";

            public string rootCharacterFolder = "";

            public CharacterConfigData config = null;

            public GameObject prefab = null;
        }
    }
}


