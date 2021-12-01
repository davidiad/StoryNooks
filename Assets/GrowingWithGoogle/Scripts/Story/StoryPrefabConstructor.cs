using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StoryNook
{
    //TODO:// rename to StoryConstructor
    public class StoryPrefabConstructor : MonoBehaviour
    {
        public StoryData storyData;

        public TextMeshProUGUI companyTMP;
        public TextMeshProUGUI taglineTMP;
        public bool played;
        public int storyID;
        public GameObject thumbnailHolder;

        private string company;
        private string tagline;
        private string title;
        private Sprite thumbnail;

        [SerializeField] private GameObject playIcon;
        [SerializeField] private GameObject rewatchIcon;

        private ScreenManager screenManager;

        void Start()
        {
            screenManager = ScreenManager.instance;

            if (storyData != null)
            {
                LoadStoryData();
            }

            UpdateStatus();
        }

        private void OnEnable()
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (screenManager != null)
            {
                played = screenManager.storySettings[storyID].Played;

                // update the play sprite depending on Played bool, to either Play or Rewatch graphics
                playIcon.SetActive(!played);
                rewatchIcon.SetActive(played);
            }
        }

        // public so that CTAScreen can call for data to be loaded
        public void LoadStoryData()
        {
            storyID = storyData.StoryID;
            company = storyData.Company;
            tagline = storyData.Tagline;

            thumbnail = storyData.Thumbnail;
            thumbnailHolder.GetComponent<Image>().sprite = thumbnail;

            companyTMP.text = company;
            taglineTMP.text = tagline;
        }
    }
}
