using PureReader.Drawables;
using Shared.Data;
using Shared.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PureReader.Controls;
public partial class TxtReaderView : ContentView
{
    public Book CurrentBook
    {
        get { return (Book)GetValue(CurrentBookProperty); }
        set { SetValue(CurrentBookProperty, value); }
    }
    public static readonly BindableProperty CurrentBookProperty =
          BindableProperty.Create(nameof(CurrentBook), typeof(Book), typeof(TxtReaderView), propertyChanged: OnBookChanged);

    public BookService Service
    {
        get { return (BookService)GetValue(ServiceProperty); }
        set { SetValue(ServiceProperty, value); }
    }
    public static readonly BindableProperty ServiceProperty =
          BindableProperty.Create(nameof(Service), typeof(BookService), typeof(TxtReaderView));


    private static void OnBookChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TxtReaderView reader)
            return;
        reader.Init();
    }

    public ObservableCollection<IDrawable> Pages { get; set; } = new ObservableCollection<IDrawable>();

    private async void Init()
    {
        if (CurrentBook == null || Service == null) return;
        var contents = await Service.GetBookContents(CurrentBook.Id, 0, 100);
        Pages.Clear();
        Pages.Add(new TxtDrawable(contents, new ReadSetting()));
        OnPropertyChanged(nameof(Pages));
    }

    public TxtReaderView()
    {
        InitializeComponent();
    }
    double offset;
    int preIndex = -1;
    private void Wrapper_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        offset = e.VerticalOffset;
        if (e.FirstVisibleItemIndex != preIndex)
        {
            preIndex = e.FirstVisibleItemIndex;
        }
    }
}