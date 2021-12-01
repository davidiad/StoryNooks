using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

namespace StoryNook
{
    public class CTAScreen : BaseScreen
    {
        [SerializeField] private TextMeshProUGUI callToAction;
        [SerializeField] private TextMeshProUGUI workshop;
        private Coroutine welcomeReturn;
        private StoryData currentStoryData;
        private StoryData nextStoryData;
        private StoryPrefabConstructor storyConstructor;

        private void OnEnable()
        {
            if (screenManager == null)
            {
                screenManager = ScreenManager.instance;
            }
            screenManager.currentScreen = (int)Screen.CTA;

            // Update dynamic text on this screen
            currentStoryData = storyData[screenManager.currentFeature];

            callToAction.text = currentStoryData.CallToAction;
            if (callToAction.text == "") { callToAction.text = "To learn how customers can find your business on Google, sign up for our free workshop:"; }
            workshop.text = currentStoryData.Workshop;
            if (workshop.text == "") { workshop.text = "Get Found on Google Search and Maps"; }

            // populate data for Next Story
            storyConstructor = this.gameObject.GetComponentInChildren<StoryPrefabConstructor>();
            int nextStoryID = screenManager.GetNextFeature();
            storyConstructor.storyID = nextStoryID;
            nextStoryData = storyData[nextStoryID]; // Get the story data from the source (StoryData game object with list of StoryData)
            storyConstructor.storyData = nextStoryData;
            storyConstructor.LoadStoryData();
        }

        private void Start()
        {
            AdjustLayoutForScreenSize();
        }

        public override void AdjustLayoutForScreenSize()
        {
            if (screenManager.screenSize != ScreenSize.Pixel_Slate_3000x2000)
            {
                background.transform.localScale = new Vector3(227.56f, 1f, 128f);
                canvasTransform.localScale = new Vector3(0.75f, 0.75f, 0.64f);
            }
        }

        protected override void AfterTransitionActions()
        {
            canvasGroup.interactable = true;
            // Set timer for automatically returning to the Welcome Screen
            welcomeReturn = StartCoroutine(screenManager.ReturnToWelcome(Screen.CTA));
        }
    }
}
