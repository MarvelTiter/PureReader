using Microsoft.Maui.Graphics;
using Shared.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Drawables
{
    internal class TxtDrawable : IDrawable
    {
        private readonly IList<Content> lines;
        private readonly ReadSetting setting;
        float topOffset = 0f;
        public int Height => (int)topOffset;
        public TxtDrawable(IList<Content> lines, ReadSetting setting)
        {
            this.lines = lines;
            this.setting = setting;
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //canvas.SetFillPaint()
            canvas.FontSize = setting.FontSize;
            foreach (var line in lines)
            {
                //var head = canvas.GetStringSize("开头", setting.Font, setting.FontSize, HorizontalAlignment.Left, VerticalAlignment.Top);
                var sSize = canvas.GetStringSize(line.Text, setting.Font, setting.FontSize, HorizontalAlignment.Left, VerticalAlignment.Top);
                var rows = (int)((Math.Floor(sSize.Width) / (dirtyRect.Width)) + 1);
                var height = (setting.LineSpacing) * rows;
                if (line.Text.Contains("妖魔鬼怪"))
                {
                    Debug.WriteLine(sSize);
                    Debug.WriteLine(dirtyRect);
                }
                canvas.DrawString(line.Text, 0, topOffset, dirtyRect.Width, height, HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.OverflowBounds, 100);
                //canvas.DrawString(line.Text, 0, topOffset, HorizontalAlignment.Left);

                topOffset += height + setting.LineSpacing;
            }
        }
    }
}
