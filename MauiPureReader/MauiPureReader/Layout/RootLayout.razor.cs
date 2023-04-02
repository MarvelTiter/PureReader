using BlazorComponent;
using MauiPureReader.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Layout
{
    public partial class RootLayout
    {
        public bool IsDark { get; set; } = true;
        [Inject]
        internal ThemeService ThemeService { get; set; }

        /// <summary>
        /// 处理系统主题切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlerAppThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            IsDark = e.RequestedTheme == AppTheme.Dark;
            InvokeAsync(StateHasChanged);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // 获取系统主题
                var appTheme = ThemeService.GetAppTheme();
                // 根据系统主题是否为Dark，为IsDark属性赋值
                IsDark = appTheme == AppTheme.Dark;
                ThemeService.ThemeChanged(HandlerAppThemeChanged);
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
