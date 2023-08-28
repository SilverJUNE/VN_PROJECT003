using DG.Tweening;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CHARACTERS
{
    /// <summary>
    /// 'Character'�� ��� ĳ���� ������ �⺻ Ŭ�����Դϴ�. �� Ŭ������ �߻� Ŭ������, 
    /// �� Ŭ������ �ν��Ͻ��� ���� ������ �� �����ϴ�. ��� �� Ŭ������ ����ϴ� ���� Ŭ������ �����ؾ� �մϴ�.
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
        /// 'Character' Ŭ������ �������Դϴ�.
        /// ĳ������ �̸��� �ʱ�ȭ�ϰ�, �־��� GameObject�κ��� RectTransform�� Animator�� ����ϴ�.
        /// </summary>
        /// <param name="name">ĳ������ �̸��Դϴ�.</param>
        /// <param name="config">ĳ������ ���� �������Դϴ�.</param>
        /// <param name="prefab">ĳ���͸� ǥ���ϴ� GameObject�Դϴ�.</param>
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
        /// ��縦 ����ϴ� �޼����Դϴ�.
        /// ���� ���ڿ� ����Ʈ ���·� �Է¹޽��ϴ�.
        /// </summary>
        /// <param name="dialogue">����� ��� ����Դϴ�.</param>
        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });
        
        public Coroutine Say(List<string> dialogue) 
        {
            dialogueSystem.ShowSpeakerName(displayName);
            UpdateTextCustomizationOnScreen();
            return dialogueSystem.Say(dialogue);
        }

        // �ؽ�Ʈ ����� ���� ���� �޼����
        public  void SetNameFont(TMP_FontAsset font)     => config.nameFont      = font;
        public  void SetNameColor(Color color)           => config.nameColor     = color;
        public  void SetDialogueFont(TMP_FontAsset font) => config.dialogueFont  = font;
        public  void SetDialogueColor(Color color)       => config.dialogueColor = color;

        /// <summary>
        /// ĳ���� ���� �����͸� �ʱ� ���·� �缳���ϴ� �޼����Դϴ�.
        /// </summary>
        public void ResetConfigurationData() => config = CharacterManager.instance.GetCharacterConfig(name, getOriginal: true);

        /// <summary>
        /// ȭ���� �ؽ�Ʈ ����� ���� ������ ������Ʈ�ϴ� �޼����Դϴ�.
        /// </summary>
        public void UpdateTextCustomizationOnScreen() => dialogueSystem.ApplySpeakerDataToDialogueContainer(config);


        // ĳ������ ǥ�� �� ���迡 ���� �޼����
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
        /// ĳ���Ͱ� ǥ�õǰų� �������� ���� ����Ǵ� �ڷ�ƾ�Դϴ�.
        /// </summary>
        public virtual IEnumerator ShowingOrHiding(bool show, float speedMultiplier = 1f)
        {
            Debug.Log("Show/Hide cannot be called from a base character type.");
            yield return null;
        }

        /// <summary>
        /// ĳ������ ��ġ�� �����ϴ� ���� �޼����Դϴ�.
        /// �� �޼���� �Ļ� Ŭ�������� �������ؾ� �մϴ�.
        /// </summary>
        /// <param name="position">������ ��ġ�Դϴ�. Vector2 �����Դϴ�.</param>
        public virtual void SetPosition(Vector2 position)
        {
            if (root == null)
                return;

            // �־��� ��ġ�� ĳ������ ��� ��Ŀ Ÿ������ ��ȯ�մϴ�.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);

            root.anchorMin = minAnchorTarget;
            root.anchorMax = maxAnchorTarget;
        }

        /// <summary>
        /// ĳ������ ��ġ�� �̵���Ű�� ���� �޼����Դϴ�. 
        /// �� �޼���� �Ļ� Ŭ�������� �������ؾ� �մϴ�.
        /// </summary>
        /// <param name="position">�̵��� ��ǥ ��ġ�Դϴ�. Vector2 �����Դϴ�.</param>
        /// <param name="speed">�̵� �ӵ��Դϴ�. �⺻���� 2f�Դϴ�.</param>
        /// <param name="smooth">�̵��� �ε巴�� ������� ���θ� �����մϴ�. �⺻���� false�Դϴ�.</param>
        /// <returns>�ڷ�ƾ�� ��ȯ�մϴ�. �̵� ���� ��� �ڷ�ƾ�� ������ �� �ֽ��ϴ�.</returns>
        public virtual Coroutine MovePosition(Vector2 position, float speed = 2f, bool smooth = false)
        {
            if (root == null)
                return null;

            // �̹� �̵� ���� ���, �̵� �ڷ�ƾ�� �����մϴ�.
            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            // ���ο� �̵� �ڷ�ƾ�� �����մϴ�.
            co_moving = characterManager.StartCoroutine(MovingToPosition(position, speed, smooth));

            return co_moving;
        }

        /// <summary>
        /// �̵� �ڷ�ƾ�� ó���ϴ� �޼����Դϴ�. �� �޼���� ĳ������ ���� ��ġ���� ��ǥ ��ġ���� �̵��ϴ� ���� �ݺ������� ����˴ϴ�.
        /// </summary>
        /// <param name="position">�̵��� ��ǥ ��ġ�Դϴ�.</param>
        /// <param name="speed">�̵� �ӵ��Դϴ�.</param>
        /// <param name="smooth">�̵��� �ε巴�� ������� ���θ� �����մϴ�.</param>
        /// <returns>IEnumerator ���¸� ��ȯ�մϴ�. �̴� �ڷ�ƾ�� �Ϻη�, �� �޼��尡 ������ ���� �����ǰ� �ٽ� ���۵� �� �ְ� ���ݴϴ�.</returns>
        private IEnumerator MovingToPosition(Vector2 position, float speed, bool smooth)
        {
            // ��ǥ ��ġ�� ��� ��Ŀ Ÿ������ ��ȯ�մϴ�.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            Vector2 padding = root.anchorMax - root.anchorMin;

            // ��Ŀ�� �ּҰ��� �ִ밪�� ��ǥ���� ������ ������ �ݺ��մϴ�.
            while (root.anchorMin != minAnchorTarget || root.anchorMax != maxAnchorTarget) //maxanchor�� minchor�� �Բ� ������Ʈ�� ���̱� ������ minchor�� ���ϴ� �͸����ε� ��� �� �ֽ��ϴ�.
            {
                // smooth �Ķ���Ϳ� ���� ��Ŀ�� �ּҰ��� ��ǥ �ּҰ����� �̵���ŵ�ϴ�.
                root.anchorMin = smooth ?
                    Vector2.Lerp(root.anchorMin, minAnchorTarget, speed * Time.deltaTime)
                  : Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed * Time.deltaTime * 0.35f);

                // ��Ŀ�� �ִ밪�� �����մϴ�.
                root.anchorMax = root.anchorMin + padding;

                // �̵��� ����� ��������ٸ� �̵��� �Ϸ��ϰ� �ڷ�ƾ�� �����մϴ�.
                if (smooth && Vector2.Distance(root.anchorMin, minAnchorTarget)<= 0.001f)
                {
                    root.anchorMin = minAnchorTarget;
                    root.anchorMax = maxAnchorTarget;
                    break;
                }

                // ���� �����ӱ��� ����մϴ�.
                yield return null;
            }

            Debug.Log("Done moving");
            // �̵� �ڷ�ƾ�� null�� �����մϴ�.
            co_moving = null;
        }

        /// <summary>
        /// �־��� ��ġ�� UI ĳ���͸� �̵���ŵ�ϴ�. �̵��� ������ �Ⱓ ���� �ε巴�� �̷������, ������ easeType�� ���� �̵� ��ΰ� �޶����ϴ�.
        /// ���� �� �޼��尡 ȣ��� �� �̹� �̵� ���� ���, ���� �̵��� �����ϰ� ���ο� �̵��� �����մϴ�.
        /// </summary>
        /// <param name="position">UI ĳ���Ͱ� �̵��� ��ǥ ��ġ�Դϴ�.</param>
        /// <param name="duration">�̵��� �ɸ� �ð��Դϴ�.</param>
        /// <param name="easeType">�̵� ��θ� �����ϴ� ��¡(Easing) �Լ��� �����Դϴ�. �⺻���� Ease.InOutQuad�Դϴ�.</param>
        /// <returns>�� �޼��尡 ������ �̵� �ڷ�ƾ�� �����Դϴ�. �� ������ ����� �̵��� ���� ��Ȳ�� Ȯ���ϰų� �̵��� ������ �� �ֽ��ϴ�.</returns>
        public virtual Coroutine MovePositionWithEase(Vector2 position, float duration, Ease easeType = Ease.InOutQuad)
        {
            if (root == null)
                return null;

            // �̵� ���� ���, ���� �̵��� �����մϴ�.
            if (isMoving)
                characterManager.StopCoroutine(co_moving);

            // ���ο� �̵��� �����մϴ�.
            co_moving = characterManager.StartCoroutine(MovingToPositionWithEase(position, duration, easeType));
            return co_moving;
        }

        /// <summary>
        /// �־��� ��ġ�� UI ĳ���͸� �̵���Ű�� �ڷ�ƾ�Դϴ�. �� �ڷ�ƾ�� MovePositionWithEase �޼��忡 ���� ���۵˴ϴ�.
        /// </summary>
        /// <param name="position">UI ĳ���Ͱ� �̵��� ��ǥ ��ġ�Դϴ�.</param>
        /// <param name="duration">�̵��� �ɸ� �ð��Դϴ�.</param>
        /// <param name="easeType">�̵� ��θ� �����ϴ� ��¡(Easing) �Լ��� �����Դϴ�. �⺻���� Ease.InOutQuad�Դϴ�.</param>
        /// <returns>IEnumerator �������̽��� �����ϴ� ��ü�Դϴ�. �� ��ü�� ���� �ڷ�ƾ�� ������ ������ �� �ֽ��ϴ�.</returns>
        private IEnumerator MovingToPositionWithEase(Vector2 position, float duration, Ease easeType = Ease.InOutQuad)
        {
            // ������ ���� ���̴� Ʈ���� ��� �����մϴ�.
            DOTween.Kill(root);

            // ��ǥ ��ġ�� ���� ��Ŀ ��ġ�� ����մϴ�.
            (Vector2 minAnchorTarget, Vector2 maxAnchorTarget) = ConvertUITargetPositionToRelativeCharacterAnchorTargets(position);
            
            // �� ��Ŀ�� ���� �̵��� �غ��մϴ�.
            Tweener moveMinAnchor = root.DOAnchorMin(minAnchorTarget, duration).SetEase(easeType);
            Tweener moveMaxAnchor = root.DOAnchorMax(maxAnchorTarget, duration).SetEase(easeType);

            // �̵��� �Ϸ�� ������ ��ٸ��ϴ�.
            moveMinAnchor.WaitForCompletion();
            moveMaxAnchor.WaitForCompletion();

            yield return null;

            Debug.Log("Done moving");
            co_moving = null;
        }

        /// <summary>
        /// UI Ÿ�� ��ġ�� ĳ������ ��� ��Ŀ Ÿ������ ��ȯ�ϴ� �޼����Դϴ�.
        /// </summary>
        /// <param name="position">UI Ÿ�� ��ġ�Դϴ�.</param>
        /// <returns>��ȯ�� minAnchorTarget�� maxAnchorTarget�� Ʃ�� �������� ��ȯ�մϴ�.</returns>
        protected (Vector2, Vector2) ConvertUITargetPositionToRelativeCharacterAnchorTargets(Vector2 position)
        {
            Vector2 padding = root.anchorMax - root.anchorMin;

            float maxX = 1f - padding.x;
            float maxY = 1f - padding.y;

            // minAnchorTarget�� ����մϴ�.
            Vector2 minAnchorTarget = new Vector2(maxX * position.x, maxY * position.y);
            // maxAnchorTarget�� ����մϴ�.
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
        /// ĳ���� ������ ��Ÿ���� �������Դϴ�.
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

