using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Data
{
    public class Chapter
    {
        public int Id { get; set; }
        public string BookId { get; set; }
        public string Title { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
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
        public Content()
        {
            
        }
    }
}
