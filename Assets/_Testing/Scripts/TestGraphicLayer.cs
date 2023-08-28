using CHARACTERS;
using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class TestGraphicLayer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test_1());
        }

        IEnumerator Running()
        { 
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
            GraphicLayer layer = panel.GetLayer(0, true);

            yield return new WaitForSeconds(1);

            Texture blendTex = Resources.Load<Texture>("Graphics/Transition Effects/hurricane");
            layer.SetTexture("Graphics/BG Images/2", blendingTexture: blendTex);

            yield return new WaitForSeconds(1);

            layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", blendingTexture: blendTex);

            yield return new WaitForSeconds(3);

            layer.currentGraphic.FadeOut();

            yield return new WaitForSeconds(2);

            Debug.Log(layer.currentGraphic);

            //layer.SetVideo("Graphics/BG Videos/Fantasy Landscape");

            //layer.SetVideo("Graphics/BG Videos/Fantasy Landscape", transitionSpeed: 0.01f, useAudio: true);
        }

        IEnumerator RunningLayers()
        {
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
            GraphicLayer layer0 = panel.GetLayer(0, true);
            GraphicLayer layer1 = panel.GetLayer(1, true);

            layer0.SetVideo("Graphics/BG Videos/Nebula");
            layer1.SetTexture("Graphics/BG Images/Spaceshipinterior");

            yield return new WaitForSeconds(2);

            GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
            GraphicLayer cinLayer = cinematic.GetLayer(0, true);

            cinLayer.SetTexture("Graphics/Gallery/pup");

            yield return DialogueSystem.instance.Say("내레이션", "우린 개를 가질 자격이 없다.");

            yield return null;
        }

        IEnumerator Test_1()
        {
            GraphicPanel panel = GraphicPanelManager.instance.GetPanel("Background");
            GraphicLayer layer0 = panel.GetLayer(0, true);
            GraphicLayer layer1 = panel.GetLayer(1, true);

            layer0.SetVideo("Graphics/BG Videos/Nebula");
            layer1.SetTexture("Graphics/BG Images/Spaceshipinterior");

            yield return new WaitForSeconds(2);

            GraphicPanel cinematic = GraphicPanelManager.instance.GetPanel("Cinematic");
            GraphicLayer cinLayer = cinematic.GetLayer(0, true);

            Character 하영 = CharacterManager.instance.CreateCharacter("하영", true);

            yield return 하영.Say("일단 시네마 레이어의 금림을 봅시다.");

            cinLayer.SetTexture("Graphics/Gallery/pup");

            yield return DialogueSystem.instance.Say("내레이션", "우린 개를 가질 자격이 없다.");

            cinLayer.Clear();

            yield return new WaitForSeconds(1f);

            panel.Clear();
        }

    }
}