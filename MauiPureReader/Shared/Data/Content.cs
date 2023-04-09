using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Data
{
    public class Chapter
    {
        public string BookId { get; set; }
        public int ChapterId { get; set; }
        public int MyProperty { get; set; }
    }
    public class Content
    {
        public string BookId { get; set; }
        public int LineIndex { get; set; }
        public string Text { get; set; }
        public bool IsTitle { get; set; }
        public Content(string text)
        {
            var matchResult = text.ExtractChapter();
            IsTitle = matchResult.Success;
            Text = text;
        }
    }
}
