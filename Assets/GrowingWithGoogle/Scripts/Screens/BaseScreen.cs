// Base class for StoryNook screens

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace StoryNook
{
    public class BaseScreen : MonoBehaviour
    {
        protected ScreenManager screenManager;
        protected List<StoryData> storyData;

        public Screen thisScreen; // public so button can access

        public GameObject background; // used when fading the background
        protected Material material;  // used when fading the background

        public CanvasGroup canvasGroup; // used to turn interactable on or off
        public RectTransform canvasTransform; // used to adjust the UI depending on screen size

        private void Awake()
        {
            // In case a screen is set active before the Scene Manager exists, set it inactive
            if (ScreenManager.instance == null)
            {
                this.gameObject.SetActive(false);
            }

            screenManager = ScreenManager.instance;
            storyData = GameObject.FindWithTag("StoryData").GetComponent<StoryDataHolder>().storyDataHolder;

            if (background != null)
            {
                MeshRenderer meshRenderer = background.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    material = meshRenderer.sharedMaterial;
                }
            }

            // Assumes each screen has one and only one canvas, and that it has a Canvas Group
            if (canvasGroup == null) // can also set in Inspector
            {
                canvasGroup = gameObject.GetComponentInChildren<CanvasGroup>();
            }

            // override for screen child classes
            AwakeActions();
        }

        public virtual void AdjustLayoutForScreenSize()
        {

        }

        // Fade out transition screen
        public virtual void FadeTransitionScreen(Screen nextScreen)
        {
            StartCoroutine(FadeOut(nextScreen));
        }

        protected IEnumerator FadeOut(Screen nextScreen)
        {
            float startingValue = 1f; // For fading out, alpha starts at 1. tint starts at 1 (0.75 for Welcome Home color)
            float endingValue = 0f;
            float lerpValue = 0f; // lerpValue always starts at 0 and goes to 1. currentValue goes the other way
            float currentValue = startingValue;
            while (lerpValue <= 1f)
            {
                if (lerpValue > 1f) { lerpValue = 1f; }
                lerpValue += Time.deltaTime / screenManager.transitionTime;
                //currentValue = Mathf.SmoothStep(startingValue, endingValue, lerpValue); // Use smoothstep instead of Lerp to ease in and out
                currentValue = Mathf.Lerp(startingValue, endingValue, lerpValue);

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = currentValue;
                }

                yield return null;
            }
            canvasGroup.alpha = 0f; //ensure that the values are set all the way to the ending value every time

            // Optional override for custom actions per screen called here
            screenManager.screens[(int)nextScreen].GetComponent<BaseScreen>().AfterTransitionActions();

            screenManager.screenshotCam.activeTexture.Release();
            Destroy(screenManager.screenshotCam.activeTexture);
            Destroy(screenManager.rendTexMat.GetTexture("_MainTex"));
            this.gameObject.SetActive(false);
        }

        // override point for individual screens to do stuff specific to that screen
        protected virtual void AfterTransitionActions()
        {

        }

        // override point for individual screens to do stuff specific to that screen
        protected virtual void AwakeActions()
        {

        }

        // helper function
        protected IEnumerator Wait(float duration)
        {
            yield return new WaitForSeconds(duration);
        }

    }
}
