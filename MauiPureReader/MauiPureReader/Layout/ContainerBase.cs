using Microsoft.AspNetCore.Components;

namespace MauiPureReader.Layout
{
    public class ContainerBase : ComponentBase
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
