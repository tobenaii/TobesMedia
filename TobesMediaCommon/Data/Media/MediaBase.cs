namespace TobesMediaCore.Data.Media
{
    public class MediaBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string PosterURL { get; private set; }
        public string ID { get; private set; }

        public MediaBase(string name, string description, string posterURL, string imdbID)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
            ID = imdbID;
        }
    }
}
