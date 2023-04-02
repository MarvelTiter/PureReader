using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiPureReader.Services
{
    internal class ThemeService
    {
        /// <summary>
        /// 获取当前系统主题
        /// </summary>
        /// <returns></returns>
        public AppTheme GetAppTheme()
        {
            return Application.Current!.RequestedTheme;
        }

        /// <summary>
        /// 系统主题切换
        /// </summary>
        /// <param name="handler"></param>
        public void ThemeChanged(EventHandler<AppThemeChangedEventArgs> handler)
        {
            Application.Current!.RequestedThemeChanged += handler;
        }
    }
}
