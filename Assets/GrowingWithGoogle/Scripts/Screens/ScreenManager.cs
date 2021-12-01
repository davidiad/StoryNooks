using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace StoryNook
{
    public enum Screen
    {
        Startup,
        Welcome,
        Feature,
        Stories,
        CTA,
        Transition
    };

    public enum ScreenSize
    {
        Pixel_Slate_3000x2000,
        HD_1920x1080
    };

    public class ScreenManager : MonoBehaviour

    {
        public static ScreenManager instance = null;

        public bool debugOn;
        public ScreenSize screenSize;

        // **** for screen capture used in cross fade transitions *****
        public Camera screenshotCam;
        public Material rendTexMat;
        private GameObject transitionScreenObject;
        private BaseScreen transitionScreen;
        // ************************************************************

        public float returnToWelcomeTime = 4.0f;
        public int currentScreen;
        public int startingFeature; // first video to play
        public int currentFeature;
        public int numFeatures = 3; // the number of videos that are 'featured' (they can appear under Next Story)

        public float volume = 0.75f;

        public GameObject storyPrefab;
        public List<GameObject> screens;
        public List<VideoClip> clips;
        public StorySettings[] storySettings;

        public float transitionTime = 0.1f;
        public bool captionOn = false;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            else if (instance != this)
            {
                Destroy(gameObject);
            }

            // Enforce that all screens are inactive at start
            // If the ScreenManager instance is null when Screens try to access it at launch, they will set themselves inactive
            foreach (GameObject screen in screens)
            {
                screen.SetActive(false);
            }

            SetScreenSize(); // Layout needs to adapt to one of 2 screen sizes
        }

        private void SetScreenSize()
        {
            if (UnityEngine.Screen.width == 3000)
            {
                screenSize = ScreenSize.Pixel_Slate_3000x2000;
            }
            else
            {
                screenSize = ScreenSize.HD_1920x1080;
            }
        }

        private void Start()
        {
            storySettings = new StorySettings[clips.Count];
            for (int i = 0; i < clips.Count; i++)
            {
                storySettings[i] = new StorySettings();
            }
            ResetStorySettings();

            transitionScreenObject = screens[(int)Screen.Transition];
            transitionScreen = transitionScreenObject.GetComponent<BaseScreen>();

            currentScreen = (int)Screen.Startup;
            screens[0].SetActive(true);

            currentFeature = startingFeature;
        }

        public void ResetStorySettings()
        {
            for (int i = 0; i < clips.Count; i++)
            {
                storySettings[i].StoryID = i;
                storySettings[i].StartingFeature = false;
                storySettings[i].Played = false;
            }
            storySettings[startingFeature].StartingFeature = true;
            captionOn = false;
        }

        public void TransitionScreens(Screen previousScreen, Screen nextScreen)
        {
            // Note: UI opacity can be animated through the canvas group;
            // however, when UI elements overlap, their opacities change at different rates, making an ugly transition.
            // Instead, this approach takes a snapshot of the previous screen; turns off the previous screen; and
            // adds the snapshot to a transition screen which is in front of the new screen.
            // After the transition screen is faded out, it is set inactive.

            // Make transition screen active and set alpha of transition screen to 0
            transitionScreenObject.SetActive(true);
            transitionScreen.canvasGroup.alpha = 0;

            // Take a snaphot of screen with render texture, convert to Texture2D, and apply to object(s)
            RenderTexture renTex = screenshotCam.activeTexture;

            Texture2D t2d = renTex.ToTexture2D();
            rendTexMat.SetTexture("_MainTex", t2d);

            screens[(int)previousScreen].SetActive(false);
            transitionScreen.canvasGroup.alpha = 1f;

            // turn off interaction until the transition is finished
            screens[(int)nextScreen].GetComponent<BaseScreen>().canvasGroup.interactable = false;


            // The next screen is set active (still blocked from view by trans screen at this point)
            screens[(int)nextScreen].SetActive(true);
            currentScreen = (int)nextScreen;

            // From the trans screen, call coroutine on itself to fade
            // When coroutine finished, call any needed action on next screen, update current screen in screen manager
            // Transition screen then sets itself inactive
            transitionScreen.FadeTransitionScreen(nextScreen);
        }

        public int GetNextFeature()
        {
            storySettings[currentFeature].Played = true;

            currentFeature++;
            if (currentFeature >= numFeatures)
            {
                currentFeature = currentFeature % numFeatures;
            }
            return currentFeature;
        }

        public IEnumerator ReturnToWelcome(Screen callingScreen)
        {
            yield return new WaitForSeconds(returnToWelcomeTime);

            ResetStorySettings();
            currentFeature = startingFeature; // after a timeout, always return to the starting feature chosen at startup
            TransitionScreens(callingScreen, Screen.Welcome);
        }

        // helper function for transitions
        public IEnumerator WaitOneFrame()
        {
            yield return null;
        }
    }
}
