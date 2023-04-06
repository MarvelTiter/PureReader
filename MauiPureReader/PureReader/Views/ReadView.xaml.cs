using PureReader.ViewModels;

namespace PureReader.Views;

public partial class ReadView : ContentPage
{

    public ReadView(ReadViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    int preIndex = -1;
    private void Scroller_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        if (DeviceInfo.Current.Platform != DevicePlatform.WinUI)
        {
            return;
        }

        if (e.FirstVisibleItemIndex != preIndex)
        {
            preIndex = e.FirstVisibleItemIndex;
        }
        else
        {
            return;
        }
        //NOTE: workaround on windows to fire collectionview itemthresholdreached command, because it does not work on windows
        if (sender is CollectionView cv && cv is IElementController element)
        {
            var count = element.LogicalChildren.Count;
            if (count - (e.LastVisibleItemIndex + 1) < cv.RemainingItemsThreshold)
            {
                if (cv.RemainingItemsThresholdReachedCommand.CanExecute(null))
                {
                    cv.RemainingItemsThresholdReachedCommand.Execute(null);
                }
            }
        }
    }
}