namespace Shared.Data
{
    public class Content
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public bool IsTitle { get; set; }
        public Content(string text)
        {
            Text = text;
            var matchResult = Text.ExtractChapter();
            Title = matchResult.Value;
            IsTitle = matchResult.Success;
        }
    }
}
