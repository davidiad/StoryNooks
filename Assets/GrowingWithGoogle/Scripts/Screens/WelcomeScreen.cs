using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

namespace StoryNook
{

    public class SkipTimes // for welcome video loop 
    {
        public float StartSkip { get; set; }
        public float EndSkip { get; set; }

    }

    public class WelcomeScreen : BaseScreen
    {

        private int currentFeature; // Can we use scene manager's currentFeature and remove this one?
        private VideoPlayer player;
        private StoryData currentStoryData;

        [SerializeField] private RectTransform moreStoriesButtonTransform; // used to adjust for UI layout per screen size
        [SerializeField] private TextMeshProUGUI companyLabel;
        [SerializeField] private Text taglineLabel;

        private double clipLength; // temporary use to quickly limit the length of the intro clip

        private SkipTimes[] skipClips; // array of starting and ending times of which parts to skip for intro loop
        private int skipIndex;
        // to hold the skipping times
        private float s;
        private float e;
        private float endtime;

        private void Start()
        {
            player = GetComponent<VideoPlayer>();
            if (player != null) { currentFeature = screenManager.currentFeature; }
            player.targetTexture.Release();
            player.clip = screenManager.clips[currentFeature];
            clipLength = player.clip.length;
            InitSkipTimes();
            AdjustLayoutForScreenSize();
        }

        public override void AdjustLayoutForScreenSize()
        {
            if (screenManager.screenSize != ScreenSize.Pixel_Slate_3000x2000)
            {

                // Note: localPosition gives unpredictable results -- use anchored position instead
                background.transform.localScale = new Vector3(227.56f, 1f, 128f);
                canvasTransform.anchoredPosition3D = new Vector3(0f, 19f, 0f);
                canvasTransform.localScale = new Vector3(0.75f, 0.75f, 0.64f);
                moreStoriesButtonTransform.anchoredPosition3D = new Vector3(1424f, -202f, 0f);

            }
        }

        // set which time points (in seconds) to skip for welcome loop (~15s) version of video
        // TODO: For ease of use, refactor to set play times instead of skip times, and add editor inspector interface
        private void InitSkipTimes()
        {
            switch (currentFeature)
            {
                case 0: // for American Hats

                    skipClips = new SkipTimes[4];
                    for (int i = 0; i < skipClips.Length; i++)
                    {
                        skipClips[i] = new SkipTimes();
                    }

                    skipClips[0].StartSkip = 0f;
                    skipClips[0].EndSkip = 4f;

                    skipClips[1].StartSkip = 6f;
                    skipClips[1].EndSkip = 9f;

                    skipClips[2].StartSkip = 14f;
                    skipClips[2].EndSkip = 83f; // 1:23

                    skipClips[3].StartSkip = 93f; // 1:33
                    skipClips[3].EndSkip = 97f; // 1:37

                    endtime = 100f;
                    break;

                case 1:  // for Honest Chops

                    skipClips = new SkipTimes[5];
                    for (int i = 0; i < skipClips.Length; i++)
                    {
                        skipClips[i] = new SkipTimes();
                    }

                    skipClips[0].StartSkip = 0f;
                    skipClips[0].EndSkip = 9f;

                    skipClips[1].StartSkip = 13f;
                    skipClips[1].EndSkip = 20f;

                    skipClips[2].StartSkip = 21.7f;
                    skipClips[2].EndSkip = 26f;

                    skipClips[3].StartSkip = 31f;
                    skipClips[3].EndSkip = 94f;

                    skipClips[4].StartSkip = 98f;
                    skipClips[4].EndSkip = 111f;

                    endtime = 113f;
                    break;

                case 2: // for Pietro Handbags

                    skipClips = new SkipTimes[4];
                    for (int i = 0; i < skipClips.Length; i++)
                    {
                        skipClips[i] = new SkipTimes();
                    }

                    skipClips[0].StartSkip = 0f;
                    skipClips[0].EndSkip = 20f;

                    skipClips[1].StartSkip = 28f;
                    skipClips[1].EndSkip = 40f;

                    skipClips[2].StartSkip = 41.85f;
                    skipClips[2].EndSkip = 89.9f;

                    skipClips[3].StartSkip = 92.9f;
                    skipClips[3].EndSkip = 124f;

                    endtime = 126.5f;
                    break;
                default:
                    //Debug.Log("Skipping not set for this video");
                    break;
            }
        }

        private void Update()
        {
            PlayWelcomeLoop();
        }

        // Play short loop from full length video
        private void PlayWelcomeLoop()
        {
            if (player.time > endtime)
            {
                player.Stop(); // return to 0
                skipIndex = 0;
                player.Play();
                return;
            }


            s = skipClips[skipIndex].StartSkip;
            e = skipClips[skipIndex].EndSkip;

            if (player.time >= s && player.time < e)
            {

                player.Pause();
                player.time = e + 0.1f; // skip to e, plus add a little to ensure we keep playing
                player.Play();

                skipIndex++;
                skipIndex = skipIndex % skipClips.Length;
            }
        }

        private void OnEnable()
        {
            skipIndex = 0;

            if (screenManager == null)
            {
                screenManager = ScreenManager.instance;
            }
            screenManager.currentScreen = (int)Screen.Welcome;

            currentFeature = screenManager.currentFeature;

            currentStoryData = storyData[currentFeature];

            DisplayTitles();

            if (player != null)
            {
                player.targetTexture.Release();
                player.clip = screenManager.clips[currentFeature];
            }
        }

        protected override void AfterTransitionActions()
        {
            canvasGroup.interactable = true;
        }

        private void DisplayTitles()
        {
            // populate the Company name and tagline labels
            if (currentStoryData != null)
            {
                companyLabel.text = currentStoryData.Company;
                taglineLabel.text = currentStoryData.Tagline;
            }
            else // fallback if story data is not wired correctly
            {
                companyLabel.text = "Tap anywhere to begin";
                taglineLabel.text = "";
            }
        }
    }
}
