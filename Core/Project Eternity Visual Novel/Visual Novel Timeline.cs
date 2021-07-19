namespace ProjectEternity.GameScreens.VisualNovelScreen
{
    /// <summary>
    /// Represent a basic timeline.
    /// </summary>
    public struct VisualNovelTimeline
    {
        public int CurrentTime;
        public int MaxTime;
        //Characters to draw.
        private int[,] CharacterChoices;

        //What each scene will draw for text.
        public string[] CharacterTexts;

        //Which background to draw for each scene.
        private int[] BackgroundChoices;

        //Which point to skip to next.
        public int[] Waypoints;

        /// <summary>
        /// Create a new VisualNovelTimeline with a list of scenes to draw with each characters, text and background to use.
        /// If no character is required for a side, 0 should be used.
        /// </summary>
        /// <param name="CharacterChoices">Array of 3 choices. 1st choice is the character on the left.
        /// 2nd choice is the character on the right.
        /// 3rd choice is which character is talking(0 is left, 1 is right).
        /// The characters choices use the index of the character list + 1.</param>
        /// <param name="CharacterTexts">The text to draw.</param>
        /// <param name="BackgroundChoices">The background to draw.</param>
        /// <param name="Waypoints">List of waypoint from which to skip to.</param>
        public VisualNovelTimeline(int[,] CharacterChoices, string[] CharacterTexts, int[] BackgroundChoices, int[] Waypoints)
        {
            this.CharacterChoices = CharacterChoices;
            this.CharacterTexts = CharacterTexts;
            this.BackgroundChoices = BackgroundChoices;
            this.Waypoints = Waypoints;
            CurrentTime = 0;
            MaxTime = 0;
        }

        /// <summary>
        /// Return the number of scenes the VisualNovelTimeline contains.
        /// </summary>
        public int Size
        {
            get { return CharacterTexts.Length; }
        }

        /// <summary>
        /// Return the text index of the text list in the current scene.
        /// </summary>
        public string Text
        {
            get { return CharacterTexts[CurrentTime]; }
        }

        /// <summary>
        /// Return the background index of the background list in the current scene.
        /// </summary>
        public int Background
        {
            get { return BackgroundChoices[CurrentTime]; }
        }

        /// <summary>
        /// Returns which character is active(0 for the first, 1 for the second)
        /// </summary>
        public int CharacterActive
        {
            get { return CharacterChoices[CurrentTime, 2]; }
        }

        /// <summary>
        /// Return the index of the first character of the character list in the current scene.
        /// </summary>
        public int Character1Index
        {
            get { return CharacterChoices[CurrentTime, 0] - 1; }
        }

        /// <summary>
        /// Return the index of the second character of the character list in the current scene.
        /// </summary>
        public int Character2Index
        {
            get { return CharacterChoices[CurrentTime, 1] - 1; }
        }
    };
}
