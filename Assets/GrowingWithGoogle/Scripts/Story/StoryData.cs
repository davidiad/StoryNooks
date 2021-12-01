using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoryNook
{
    [CreateAssetMenu(fileName = "StoryData", menuName = "Story Data", order = 51)]
    public class StoryData : ScriptableObject
    {
        [SerializeField]
        private int storyID;

        [SerializeField]
        private Sprite thumbnail;

        [SerializeField]
        private string company;

        [SerializeField]
        private string tagline;

        [SerializeField]
        private string callToAction;

        [SerializeField]
        private string workshop;

        [SerializeField]
        private string srtFileName;

        public int StoryID { get { return storyID; } }
        public Sprite Thumbnail { get { return thumbnail; } }
        public string Company { get { return company; } }
        public string Tagline { get { return tagline; } }
        public string CallToAction { get { return callToAction; } }
        public string Workshop { get { return workshop; } }
        public string SRTFileName { get { return srtFileName; } }

    }
}
