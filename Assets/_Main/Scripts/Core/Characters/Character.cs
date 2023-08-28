using DG.Tweening;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 'Character'는 모든 캐릭터 유형의 기본 클래스입니다. 이 클래스는 추상 클래스로, 
    /// 이 클래스의 인스턴스를 직접 생성할 수 없습니다. 대신 이 클래스를 상속하는 하위 클래스를 생성해야 합니다.
    /// </summary>
    public abstract class Character 
    {
        public  const   bool   ENABLE_ON_START = false;
        private const   float  UNHIGHLIGHTED_DARKEN_STREENGTH = 0.65f;
        public  const   bool   DEFAULT_ORIENTATION_IS_FACING_LEFT = true;
        public  const   string ANIMATON_REFRESH_TRIGGER= "Refresh";

        public  string name = "";
        public  string displayName = "";
        public  RectTransform root = null;
        public  CharacterConfigData config;
        public  Animator animator;

        public      Color   color { get; protected set; } = Color.white;
        protected   Color   displayColor        => highlighted ? highlightedColor : unhighlightedColor;
        protected   Color   highlightedColor    => color;
        protected   Color   unhighlightedColor  => new Color(color.r * UNHIGHLIGHTED_DARKEN_STREENGTH, color.g * UNHIGHLIGHTED_DARKEN_STREENGTH, color.b * UNHIGHLIGHTED_DARKEN_STREENGTH, color.a);
        public      bool    highlighted { get; protected set; } = true;
        protected   bool    facingLeft = DEFAULT_ORIENTATION_IS_FACING_LEFT;
        public      int     priority { get; protected set; }

        protected   CharacterManager characterManager   => CharacterManager.instance;
        public      DialogueSystem dialogueSystem       => DialogueSystem.instance;


        protected Coroutine co_revealing, co_hiding, co_moving, co_changingColor, co_highlighting, co_flipping;
        
        public  bool         isRevealing     => co_revealing     != null;
        public  bool         isHiding        => co_hiding        != null;
        public  bool         isMoving        => co_moving        != null;
        public  bool         isChangingColor => co_changingColor != null;
        public  bool         isFlipping      => co_flipping      != null;


        public  bool isHighlighting   => (highlighted && co_highlighting != null);
        public  bool isUnhighlighting => (!highlighted && co_highlighting != null);
        public virtual bool isVisible { get; set; }
        public bool isFacingLeft => facingLeft;
        public bool isFacingRight => !facingLeft;


        /// <summary>
        /// 'Character' 클래스의 생성자입니다.
        /// 캐릭터의 이름을 초기화하고, 주어진 GameObject로부터 RectTransform과 Animator를 얻습니다.
        /// </summary>
        /// <param name="name">캐릭터의 이름입니다.</param>
        /// <param name="config">캐릭터의 구성 데이터입니다.</param>
        /// <param name="prefab">캐릭터를 표현하는 GameObject입니다.</param>
        public Character(string name, CharacterConfigData config, GameObject prefab)
        {
            this.name   = name;
            displayName = name;

            this.config = config;

            if(prefab != null)
            {
                Transform parentPanel = (config.characterType == CharacterType.Live2D ? characterManager.characterPanelLive2D : characterManager.characterPanel);
               
                GameObject ob = Object.Instantiate(prefab, parentPanel);
                ob.name = characterManager.FormatCharacterPath(characterManager.characterPrefabNameFormat, name);
                ob.SetActive(true);
                root = ob.GetComponent<RectTransform>();
                animator = root.GetComponentInChildren<Animator>();
            }
        }

        /// <summary>
        /// 대사를 출력하는 메서드입니다.
        /// 대사는 문자열 리스트 형태로 입력받습니다.
        /// </summary>
        /// <param name="dialogue">출력할 대사 목록입니다.</param>
        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });
        
        public Coroutine Say(List<string> dialogue) 
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        // 텍스트 사용자 정의 설정 메서드들
        public  void SetNameFont(TMP_FontAsset font)     => config.nameFont      = font;
        public  void SetNameColor(Color color)           => config.nameColor     = color;
        public  void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont  = font;
        public  void SetDialogueColor(Color color)       => config.dialogueColor = color;

        /// <summary>
        /// 캐릭터 설정 데이터를 초기 상태로 재설정하는 메서드입니다.
        /// </summary>
        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name, getOriginal: true);

        /// <summary>
        /// 화면의 텍스트 사용자 정의 설정을 업데이트하는 메서드입니다.
        /// </summary>
        public void UpdateTextCustomizationOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);


        // 캐릭터의 표시 및 숨김에 대한 메서드들
        public virtual Coroutine Show(float speedMultiplier = 1f)
        {
            if (isRevealing)
                characterManager.StopCoroutine(co_revealing);

            if (isHiding)
                characterManager.StopCoroutine(co_hiding);

            co_revealing = characterManager.StartCoroutine(ShowingOrHiding(true, speedMultiplier));

            return co_revealing;
        }
        public virtual Coroutine Hide(float speedMultiplier = 1f)
        {
            if (isHiding)
                characterManager.StopCoroutine(co_hiding);

            if (isRevealing)
                characterManager.StopCoroutine(co_revealing);

            co_hiding = characterManager.StartCoroutine(ShowingOrHiding(false, speedMultiplier));

            return co_hiding;
        }

        /// <summary>
        /// 캐릭터가 표시되거나 숨겨지는 동안 수행되는 코루틴입니다.
        /// </summary>
        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            Debug.Log("Show/Hide cannot be called from a base character type.");
            yield return null;
        }

        /// <summary>
        /// 캐릭터의 위치를 설정하는 가상 메서드입니다.
        /// 이 메서드는 파생 클래스에서 재정의해야 합니다.
        /// </summary>
        /// <param name="position">설정할 위치입니다. Vector2 형식입니다.</param>
        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;

            // 주어진 위치를 캐릭터의 상대 앵커 타겟으로 변환합니다.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        /// <summary>
        /// 캐릭터의 위치를 이동시키는 가상 메서드입니다. 
        /// 이 메서드는 파생 클래스에서 재정의해야 합니다.
        /// </summary>
        /// <param name="position">이동할 목표 위치입니다. Vector2 형식입니다.</param>
        /// <param name="speed">이동 속도입니다. 기본값은 2f입니다.</param>
        /// <param name="smooth">이동이 부드럽게 진행될지 여부를 결정합니다. 기본값은 false입니다.</param>
        /// <returns>코루틴을 반환합니다. 이동 중인 경우 코루틴을 중지할 수 있습니다.</returns>
        public virtual Coroutine MovePosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;

            // 이미 이동 중인 경우, 이동 코루틴을 중지합니다.
            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            // 새로운 이동 코루틴을 시작합니다.
            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return co_moving;
        }

        /// <summary>
        /// 이동 코루틴을 처리하는 메서드입니다. 이 메서드는 캐릭터의 현재 위치에서 목표 위치까지 이동하는 동안 반복적으로 실행됩니다.
        /// </summary>
        /// <param name="position">이동할 목표 위치입니다.</param>
        /// <param name="speed">이동 속도입니다.</param>
        /// <param name="smooth">이동이 부드럽게 진행될지 여부를 결정합니다.</param>
        /// <returns>IEnumerator 형태를 반환합니다. 이는 코루틴의 일부로, 이 메서드가 프레임 간에 중지되고 다시 시작될 수 있게 해줍니다.</returns>
        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            // 목표 위치를 상대 앵커 타겟으로 변환합니다.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            // 앵커의 최소값과 최대값이 목표값에 도달할 때까지 반복합니다.
            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget) //maxanchor가 minchor와 함께 업데이트될 것이기 때문에 minchor를 평가하는 것만으로도 벗어날 수 있습니다.
            {
                // smooth 파라미터에 따라 앵커의 최소값을 목표 최소값으로 이동시킵니다.
                root.anchorMin = smooth ?
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                  : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                // 앵커의 최대값을 갱신합니다.
                root.anchorMax = root.anchorMin + padding;

                // 이동이 충분히 가까워졌다면 이동을 완료하고 코루틴을 종료합니다.
                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget)<= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                // 다음 프레임까지 대기합니다.
                yield return null;
            }

            Debug.Log("Done moving");
            // 이동 코루틴을 null로 설정합니다.
            co_moving = null;
        }

        /// <summary>
        /// 주어진 위치로 UI 캐릭터를 이동시킵니다. 이동은 지정된 기간 동안 부드럽게 이루어지며, 선택한 easeType에 따라 이동 경로가 달라집니다.
        /// 만약 이 메서드가 호출될 때 이미 이동 중인 경우, 현재 이동을 중지하고 새로운 이동을 시작합니다.
        /// </summary>
        /// <param name="position">UI 캐릭터가 이동할 목표 위치입니다.</param>
        /// <param name="duration">이동에 걸릴 시간입니다.</param>
        /// <param name="easeType">이동 경로를 결정하는 이징(Easing) 함수의 종류입니다. 기본값은 Ease.InOutQuad입니다.</param>
        /// <returns>이 메서드가 시작한 이동 코루틴의 참조입니다. 이 참조를 사용해 이동의 진행 상황을 확인하거나 이동을 중지할 수 있습니다.</returns>
        public virtual Coroutine MovePositionWithEase(Vector2 position, float duration, Ease easeType = Ease.InOutQuad)
        {
            if (root == null)
                return null;

            // 이동 중인 경우, 현재 이동을 중지합니다.
            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            // 새로운 이동을 시작합니다.
            co_moving = characterManager.StartCoroutine(MovingToPositionWithEase(position, duration, easeType));
            return co_moving;
        }

        /// <summary>
        /// 주어진 위치로 UI 캐릭터를 이동시키는 코루틴입니다. 이 코루틴은 MovePositionWithEase 메서드에 의해 시작됩니다.
        /// </summary>
        /// <param name="position">UI 캐릭터가 이동할 목표 위치입니다.</param>
        /// <param name="duration">이동에 걸릴 시간입니다.</param>
        /// <param name="easeType">이동 경로를 결정하는 이징(Easing) 함수의 종류입니다. 기본값은 Ease.InOutQuad입니다.</param>
        /// <returns>IEnumerator 인터페이스를 구현하는 개체입니다. 이 개체를 통해 코루틴의 실행을 제어할 수 있습니다.</returns>
        private IEnumerator MovingToPositionWithEase(Vector2 position, float duration, Ease easeType = Ease.InOutQuad)
        {
            // 기존에 실행 중이던 트윈을 모두 중지합니다.
            DOTween.Kill(root);

            // 목표 위치에 대한 앵커 위치를 계산합니다.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            
            // 각 앵커에 대해 이동을 준비합니다.
            Tweener moveMinAnchor = root.DOAnchorMin(minAnchorTarget, duration).SetEase(easeType);
            Tweener moveMaxAnchor = root.DOAnchorMax(maxAnchorTarget, duration).SetEase(easeType);

            // 이동이 완료될 때까지 기다립니다.
            moveMinAnchor.WaitForCompletion();
            moveMaxAnchor.WaitForCompletion();

            yield return null;

            Debug.Log("Done moving");
            co_moving = null;
        }

        /// <summary>
        /// UI 타겟 위치를 캐릭터의 상대 앵커 타겟으로 변환하는 메서드입니다.
        /// </summary>
        /// <param name="position">UI 타겟 위치입니다.</param>
        /// <returns>변환된 minAnchorTarget과 maxAnchorTarget을 튜플 형식으로 반환합니다.</returns>
        protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            // minAnchorTarget을 계산합니다.
            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            // maxAnchorTarget을 계산합니다.
            Vector2 maxAnchorTarget = minAnchorTarget + padding;

            return (minAnchorTarget, maxAnchorTarget);
        }

        public virtual void SetColor(Color color)
        {
            this.color = color;
        }

        public Coroutine TransitionColor(Color color, float speed = 1f)
        {
            this.color = color;

            if (isChangingColor)
                characterManager.StopCoroutine(co_changingColor);

            co_changingColor = characterManager.StartCoroutine(ChangingColor(speed));

            return co_changingColor;
        }

        public virtual IEnumerator ChangingColor(float speed)
        {
            Debug.Log("Color changing is not applicable on this character type!");
            yield return null;
        }

        public Coroutine Highlight(float speed = 1f, bool immediate = false)
        {
            if (isHighlighting || isUnhighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = true;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }

        public Coroutine UnHighlight(float speed = 1f, bool immediate = false)
        {
            if (isUnhighlighting || isHighlighting)
                characterManager.StopCoroutine(co_highlighting);

            highlighted = false;
            co_highlighting = characterManager.StartCoroutine(Highlighting(speed, immediate));

            return co_highlighting;
        }

        public virtual IEnumerator Highlighting(float speedMultiplier, bool immediate = false)
        {
            Debug.Log("Highlighting is not available on this character type!");
            yield return null;
        }

        public Coroutine Flip(float speed = 1, bool immediate = false)
        {
            if (isFacingLeft)
                return FaceRight(speed, immediate);
            else
                return FaceLeft(speed, immediate);
        }

        public Coroutine FaceLeft(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            facingLeft = true;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public Coroutine FaceRight(float speed = 1, bool immediate = false)
        {
            if (isFlipping)
                characterManager.StopCoroutine(co_flipping);

            facingLeft = false;
            co_flipping = characterManager.StartCoroutine(FaceDirection(facingLeft, speed, immediate));

            return co_flipping;
        }

        public  virtual IEnumerator FaceDirection(bool faceLeft, float speedMultiplier, bool immediate)
        {
            Debug.Log("Cannot flip a character of this type!");
            yield return null;
        }

        public void SetPriority(int priority, bool autoSortCharactersOnUi = true)
        {
            this.priority = priority;

            if (autoSortCharactersOnUi)
                characterManager.SortCharacters();

        }

        public void Animate(string animation)
        {
            animator.SetTrigger(animation);
        }

        public void Animate(string animation, bool state)
        {
            animator.SetBool(animation, state);
            animator.SetTrigger(ANIMATON_REFRESH_TRIGGER);
        }

        public virtual void OnSort(int sortingIndex)
        {
            return;
        }

        public virtual void OnReceiveCastingExpression(int layer, string expression)
        {
            return;
        }

        /// <summary>
        /// 캐릭터 유형을 나타내는 열거형입니다.
        /// </summary>
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
            Live2D,
            Model3D
        }
    }
}

