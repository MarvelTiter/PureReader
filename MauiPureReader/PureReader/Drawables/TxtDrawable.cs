using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;
using Shared.Data;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Drawables
{
    internal class TestDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float topOffset = 0f;
            for (int i = 0; i < 10; i++)
            {
                canvas.DrawString($"Hello {i}", 0, topOffset, HorizontalAlignment.Left);
                topOffset += 20f;
            }
        }
    }

    public class TxtDrawable : IDrawable
    {
        private readonly ReadSetting setting;
        float topOffset = 0f;
        public float DragOffset { get; set; }
        public CacheContentManager CacheMsg { get; set; }
        public TxtDrawable(ReadSetting setting)
        {
            this.setting = setting;
        }

        Dictionary<int, float> position = new Dictionary<int, float>();
        int lineCursor = 0;
        public void FixContents()
        {

        }

        /// <summary>
        /// 判断是否可以滑动重绘
        /// </summary>
        /// <param name="delta">滑动偏移量，大于0，往下滑动，小于0，往上滑动</param>
        /// <returns></returns>
        public bool CanDraw(float delta)
        {
            return true;
        }

        public (float FirstOffset, int Cursor) PreviousInfo { get; set; } = (0, 0);
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (CacheMsg == null) return;
            canvas.FontSize = setting.FontSize;
            topOffset += DragOffset;
            var index = PreviousInfo.Cursor;
            if (topOffset > 0)
            {

            }
            var contents = CacheMsg.GetContents(index);

            foreach (var line in contents)
            {
                var sSize = canvas.GetStringSize(line.Text, setting.Font, setting.FontSize, HorizontalAlignment.Left, VerticalAlignment.Top);
                var rows = (int)((Math.Floor(sSize.Width) / (dirtyRect.Width)) + 1);
                var height = (setting.LineSpacing) * rows;
                if (topOffset + height < 0)
                {
                    topOffset += height + setting.LineSpacing;
                    continue;
                }
                //Debug.WriteLine($"TopOffset: {topOffset}, Index: {line.LineIndex}");
                canvas.DrawString(line.Text, 0, topOffset, dirtyRect.Width, height, HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.OverflowBounds, 100);
                topOffset += height + setting.LineSpacing;
                if (topOffset > dirtyRect.Height)
                {

                    break;
                }
            }

            //Debug.WriteLine($"End Draw======================================");
        }
    }
}
