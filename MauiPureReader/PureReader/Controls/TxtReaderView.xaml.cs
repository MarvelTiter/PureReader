using PureReader.Drawables;
using Shared.Data;
using Shared.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

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

    public ICommand Command
    {
        get { return (ICommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(TxtReaderView));


    private static void OnBookChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not TxtReaderView reader)
            return;
        reader.Init();
    }

    public ObservableCollection<TxtDrawable> Pages { get; set; } = new ObservableCollection<TxtDrawable>();

    public TxtDrawable Drawer { get; set; } = new TxtDrawable(new ReadSetting());

    private void Init()
    {
        if (CurrentBook == null || Service == null) return;
        Drawer.Init(CurrentBook, Service);
    }

    public TxtReaderView()
    {
        InitializeComponent();
    }

    bool enableDrag = false;
    PointF? start;
    float preOffset = 0f;
    private void Graphics_StartInteraction(object sender, TouchEventArgs e)
    {
        if (e.Touches.Length != 1) return;
        enableDrag = true;
        start = e.Touches[0];
    }
    private void Graphics_DragInteraction(object sender, TouchEventArgs e)
    {
        if (!enableDrag) return;
        if (!start.HasValue) return;
        var offset = e.Touches[0] - start.Value;
        if (!Drawer.CanDraw(offset.Height)) return;
        if (Math.Abs(offset.Height - preOffset) < 1) return;
        preOffset = offset.Height;
        Drawer.DragOffset = offset.Height;
        Graphics.Invalidate();
    }

    private void Graphics_EndInteraction(object sender, TouchEventArgs e)
    {
        enableDrag = false;
        start = null;
        preOffset = 0f;
        Drawer.FixContents();
        Command?.Execute(Drawer.Cursor);
    }

    private void Graphics_CancelInteraction(object sender, EventArgs e)
    {
        enableDrag = false;
        start = null;
        preOffset = 0f;
        Drawer.FixContents();
        Command?.Execute(Drawer.Cursor);
    }
}