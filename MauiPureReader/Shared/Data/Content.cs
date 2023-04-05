using CommunityToolkit.Mvvm.ComponentModel;

namespace Shared.Data
{
    public partial class Content : ObservableObject
    {
        [ObservableProperty]
        private string text;
        [ObservableProperty]
        private string title;
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
