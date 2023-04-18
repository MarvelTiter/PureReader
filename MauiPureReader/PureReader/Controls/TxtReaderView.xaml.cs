using PureReader.Drawables;
using Shared.Data;
using Shared.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

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

    public ObservableCollection<TxtDrawable> Pages { get; set; } = new ObservableCollection<TxtDrawable>();

    public TxtDrawable Drawer { get; set; } = new TxtDrawable(new ReadSetting());

    private async void Init()
    {
        if (CurrentBook == null || Service == null) return;
        System.Diagnostics.Debug.WriteLine("Init......................");
        var contents = await Service.GetBookContents(CurrentBook.Id, 0, 100);
        Drawer.UpdateContents(contents);
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

    private void GraphicsView_Loaded(object sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("GraphicsView_Loaded......................");
        var g = sender as GraphicsView;
        g.Invalidate();
    }
    bool enableDrag = false;
    private void Graphics_StartInteraction(object sender, TouchEventArgs e)
    {
        enableDrag = true;
        Debug.WriteLine($"Tap Start: {e.Touches[0]}");
    }

    private void Graphics_DragInteraction(object sender, TouchEventArgs e)
    {
        if (!enableDrag) return;
        Debug.WriteLine($"Drag: {e.Touches[0]}");
    }

    private void Graphics_EndInteraction(object sender, TouchEventArgs e)
    {
        enableDrag = false;
        Debug.WriteLine($"Tap End: {e.Touches[0]}");
    }
}