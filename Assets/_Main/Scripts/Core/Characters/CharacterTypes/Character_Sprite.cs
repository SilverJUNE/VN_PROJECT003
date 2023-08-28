using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CHARACTERS
{
    /// <summary>
    /// Character_Sprite 클래스는 스프라이트 기반의 캐릭터를 정의합니다.
    /// </summary>
    public class Character_Sprite : Character
    {
        // 캐릭터 렌더러의 부모 이름과 스프라이트 시트명 등 상수를 정의합니다.
        private const string    SPRITE_RENDERERD_PARENT_NAME        = "Renderers";
        private const string    SPRITESHEET_DEFAULT_SHEETNAME       = "Default";
        private const char      SPRITESHEET_TEX_SPRITE_DELIMITTER    = '-';

        /// <summary>
        /// 캐릭터의 루트 CanvasGroup을 가져옵니다.
        /// </summary>
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        /// <summary>
        /// 캐릭터의 스프라이트 레이어 리스트를 정의합니다.
        /// </summary>
        public List<CharacterSpriteLayer> layers = new List<CharacterSpriteLayer>();

        /// <summary>
        /// 캐릭터의 아트 에셋 디렉토리 경로를 저장합니다.
        /// </summary>
        private string artAssetsDirectory = "";

        /// <summary>
        /// 캐릭터가 현재 표시 중인지 여부를 반환하거나 설정합니다.
        /// </summary>
        public override bool isVisible
        {
            get { return isRevealing || rootCG.alpha > 0; }
            set { rootCG.alpha = value ? 1 : 0; }
        }

        /// <summary>
        /// Character_Sprite 클래스의 생성자입니다.
        /// </summary>
        /// <param name="name">캐릭터의 이름입니다.</param>
        /// <param name="config">캐릭터의 설정 정보입니다.</param>
        /// <param name="prefab">캐릭터의 프리팹 객체입니다.</param>
        /// <param name="rootAssetsFolder">캐릭터의 루트 에셋 폴더 경로입니다.</param>
        public Character_Sprite(string name, CharacterConfigData config, GameObject prefab, string rootAssetsFolder) : base(name, config, prefab)
        {
            // 생성자에서 CanvasGroup의 alpha 값을 설정하고, 레이어를 가져오는 함수를 호출합니다.
            rootCG.alpha = ENABLE_ON_START ? 1: 0;
            artAssetsDirectory = rootAssetsFolder + "/Images";

            // 캐릭터의 레이어를 가져옵니다.
            GetLayers();

            Debug.Log($"Created Sprite Character: '{name}'");
        }

        /// <summary>
        /// 캐릭터의 스프라이트 레이어를 가져오는 메서드입니다.
        /// </summary>
        private void GetLayers()
        {
            Transform rendererRoot = animator.transform.Find(SPRITE_RENDERERD_PARENT_NAME);

            if (rendererRoot == null)
                return;

            for(int i = 0; i < rendererRoot.transform.childCount; i++)
            {
                Transform child = rendererRoot.transform.GetChild(i);

                Image rendererImage = child.GetComponentInChildren<Image>();

                if(rendererImage != null)
                {
                    CharacterSpriteLayer layer = new CharacterSpriteLayer(rendererImage, i);
                    layers.Add(layer);
                    child.name = $"Layer: {i}";
                }
            }
        }

        /// <summary>
        /// 지정된 스프라이트를 지정된 레이어에 설정하는 메서드입니다.
        /// </summary>
        public void SetSprite(Sprite sprite, int layer = 0)
        {
            layers[layer].SetSprite(sprite);
        }

        /// <summary>
        /// 지정된 스프라이트 이름에 해당하는 스프라이트를 가져오는 메서드입니다.
        /// </summary>
        public Sprite GetSprite(string spriteName)
        {
            // 이 함수는 스프라이트 이름을 인자로 받아 해당 스프라이트를 반환합니다.
            // 캐릭터의 타입이 스프라이트 시트인 경우와 아닌 경우에 따라 스프라이트를 로드하는 방식이 다릅니다.
            if (config.characterType == CharacterType.SpriteSheet)
            {
                string[] data       = spriteName.Split(SPRITESHEET_TEX_SPRITE_DELIMITTER);
                Sprite[] spriteArray = new Sprite[0];

                if (data.Length == 2)
                {
                    string textureName = data[0];
                    spriteName = data[1];
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{textureName}");
                }
                else
                {
                    spriteArray = Resources.LoadAll<Sprite>($"{artAssetsDirectory}/{SPRITESHEET_DEFAULT_SHEETNAME}");

                }
                if (spriteArray.Length == 0)
                    Debug.LogWarning($"Character '{name}' does not have a default art asset called '{SPRITESHEET_DEFAULT_SHEETNAME}'");

                return Array.Find(spriteArray, sprite => sprite.name == spriteName);
            }
            else
            {
                return Resources.Load<Sprite>($"{artAssetsDirectory}/{spriteName}");
            }
        }

        /// <summary>
        /// 지정된 스프라이트로 전환하는 코루틴을 시작하는 메서드입니다.
        /// </summary>
        public Coroutine TransitionSprite(Sprite sprite, int layer =0, float speed = 1)
        {
            // 이 함수는 지정된 스프라이트로 전환하는 코루틴을 시작합니다.
            CharacterSpriteLayer spriteLayer = layers[layer];
            
            return spriteLayer.TransitionSprite(sprite, speed);
        }

        /// <summary>
        /// 캐릭터를 나타내거나 숨기는 코루틴입니다.
        /// </summary>
        /// <param name="show">캐릭터를 보여줄지 여부를 나타내는 부울 값입니다.</param>
        /// <returns>IEnumerator 코루틴입니다.</returns>
        public override IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            // 이 함수는 캐릭터를 보여주거나 숨기는 코루틴을 실행합니다.
            // show 변수가 true인 경우 캐릭터를 보여주고, false인 경우 캐릭터를 숨깁니다.
            float targetAlpha = show ? 1f : 0;  // 목표 알파값을 설정합니다.
            CanvasGroup self = rootCG;

            // 현재 알파값이 목표 알파값과 같아질 때까지 알파값을 조정합니다.
            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime);
                yield return null;
            }

            // 알파값 조정이 완료되면 co_revealing과 co_hiding을 null로 설정합니다.
            co_revealing = null;
            co_hiding = null;
        }

        /// <summary>
        /// 지정된 색상으로 캐릭터의 색상을 설정하는 메서드입니다.
        /// </summary>
        public override void SetColor(Color color)
        {
            // 이 함수는 캐릭터의 색상을 설정합니다.
            base.SetColor(color);

            color = displayColor;

            foreach (CharacterSpriteLayer layer in layers)
            {
                layer.StopChangingColor();
                layer.SetColor(color);
            }
        }

        /// <summary>
        /// 캐릭터의 색상을 변경하는 코루틴입니다.
        /// </summary>
        public override IEnumerator ChangingColor(float speed)
        {
            // 이 함수는 캐릭터의 색상을 변경하는 코루틴을 실행합니다.
            foreach (CharacterSpriteLayer layer in layers)
                layer.TransitionColor(displayColor, speed);

            yield return null;

            while(layers.Any(l => l.isChangingColor))
                yield return null;

            co_changingColor = null;
        }

        /// <summary>
        /// 캐릭터를 하이라이트하는 코루틴입니다.
        /// </summary>
        public override IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            // 이 함수는 캐릭터를 하이라이트하는 코루틴을 실행합니다.
            Color targetColor = displayColor;

            foreach(CharacterSpriteLayer layer in layers)
            {
                if (immediate)
                    layer.SetColor(displayColor);
                else
                    layer.TransitionColor(targetColor, speedMultiplier);
            }


            yield return null;

            while (layers.Any(l => l.isChangingColor))
                yield return null;

            co_highlighting = null;
        }

        /// <summary>
        /// 캐릭터가 특정 방향을 바라보게 하는 코루틴입니다.
        /// </summary>
        public override IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            // 이 함수는 캐릭터가 특정 방향을 바라보게 하는 코루틴을 실행합니다.
            // faceLeft 변수가 true인 경우 캐릭터가 왼쪽을, false인 경우 오른쪽을 바라봅니다.
            foreach (CharacterSpriteLayer layer in layers)
            {
                if (faceLeft)
                    layer.FaceLeft(speedMultiplier, immediate);
                else
                    layer.FaceRight(speedMultiplier, immediate);
            }

            yield return null;

            while(layers.Any(l => l.isFlipping))
                yield return null;

            co_flipping = null;
        }

        /// <summary>
        /// 캐릭터가 표정을 바꾸는 코루틴입니다.
        /// </summary>
        public override void OnReceiveCastingExpression(int layer, string expression)
        {
            // 이 함수는 캐릭터가 표정을 바꾸는 코루틴을 실행합니다.
            Sprite sprite = GetSprite(expression);

            if(sprite == null)
            {
                Debug.LogWarning($"Sprite '{expression}' could not be found for character '{name}'");
                return;
            }

            TransitionSprite(sprite, layer);
        }

    }
}