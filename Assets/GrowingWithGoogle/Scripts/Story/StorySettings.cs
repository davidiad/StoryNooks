namespace StoryNook
{
    public class StorySettings
    {
        public int StoryID { get; set; }
        public bool StartingFeature { get; set; } // is this the story to start with at launch?
        public bool Played { get; set; }
    }
}
