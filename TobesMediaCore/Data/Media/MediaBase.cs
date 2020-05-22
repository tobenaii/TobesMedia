namespace TobesMediaCore.Data.Media
{
    public abstract class MediaBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string PosterURL { get; private set; }

        public MediaBase(string name, string description, string posterURL)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
        }
    }
}
