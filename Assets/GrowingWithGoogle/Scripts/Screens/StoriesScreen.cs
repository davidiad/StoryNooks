using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StoryNook
{

    public class StoriesScreen : BaseScreen
    {
        public ScrollRect scrollRect; // to adjust for 1080p screen size
        public RectTransform content; // to adjust for 1080p screen size

        private Coroutine welcomeReturn;
        private bool recentInteraction;

        private void OnEnable()
        {
            if (screenManager == null)
            {
                screenManager = ScreenManager.instance;
            }
            screenManager.currentScreen = (int)Screen.Stories;

            // Set timer for automatically returning to the Welcome Screen
            recentInteraction = false;
        }

        private void Start()
        {
            AdjustLayoutForScreenSize();
        }

        public override void AdjustLayoutForScreenSize()
        {
            if (screenManager.screenSize != ScreenSize.Pixel_Slate_3000x2000)
            {
                canvasTransform.localScale = new Vector3(0.75f, 0.75f, 0.64f);
                canvasTransform.anchoredPosition3D = new Vector3(0f, -110f, 0f);
                scrollRect.normalizedPosition = new Vector2(0, 1);
                content.sizeDelta = new Vector2(-6f, 5420f);
            }
        }

        protected override void AfterTransitionActions()
        {
            canvasGroup.interactable = true;
            StartWelcomeTimer();
        }

        // If the user is actively interacting with the screen, reset the return to Welcome page timer
        // Call on value changed of scroll rect
        public void ResetWelcomeTimer()
        {
            if (recentInteraction == false && welcomeReturn != null)
            {
                StopCoroutine(welcomeReturn);
                recentInteraction = true;

                // Add a pause before restarting the coroutine, to avoid many multiple calls to stop and start again
                StartCoroutine(PauseInteractionCheck(2f));

                StartWelcomeTimer();
            }
        }

        private void StartWelcomeTimer()
        {
            welcomeReturn = StartCoroutine(screenManager.ReturnToWelcome(Screen.Stories));
        }

        private IEnumerator PauseInteractionCheck(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            recentInteraction = false;
        }

    }
}
