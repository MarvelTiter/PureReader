using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Graphics;
using Shared.Data;
using Shared.Services;
using Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Drawables
{
    public class TxtDrawable : IDrawable
    {
        class PreviousInfo
        {
            public int Cursor { get; set; }
            public float TopOffset { get; set; }
        }
        private readonly ReadSetting setting;
        private CacheContentManager cache;

        PreviousInfo previousInfo;
        public float DragOffset { get; set; }
        public Book Current { get; set; }
        public TxtDrawable(ReadSetting setting)
        {
            this.setting = setting;
        }
        public int Cursor => previousInfo.Cursor;
        public void FixContents()
        {
            previousInfo.TopOffset = temp.TopOffset;
            previousInfo.Cursor = temp.Cursor;
        }

        /// <summary>
        /// 判断是否可以滑动重绘
        /// </summary>
        /// <param name="delta">滑动偏移量，大于0，往下滑动，小于0，往上滑动</param>
        /// <returns></returns>
        public bool CanDraw(float delta)
        {
            if (delta > 0 && previousInfo.Cursor > 0)
                return true;
            else if (delta < 0 && previousInfo.Cursor < Current.Lines)
                return true;
            return false;
        }

        PreviousInfo temp = new PreviousInfo();

        private float GetParagraphHeight(ICanvas canvas, string text, float canvasWidth)
        {
            var size = canvas.GetStringSize(text, setting.Font, setting.FontSize, HorizontalAlignment.Left, VerticalAlignment.Top);
            var rows = (int)((Math.Floor(size.Width) / (canvasWidth)) + 1);
            return setting.FontSize * rows;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (cache == null) return;
            canvas.FontSize = setting.FontSize;
            var topOffset = previousInfo.TopOffset + DragOffset;
            var index = previousInfo.Cursor;
            Debug.WriteLine($"TopOffset: {topOffset}, DragOffset: {DragOffset}, Cursor: {index}");
            while (topOffset > 0)
            {
                index -= 1;
                var line = cache.GetSingle(index);
                if (line == null)
                {
                    topOffset = 0;
                    break;
                }
                var ph = GetParagraphHeight(canvas, line.Text, dirtyRect.Width);
                topOffset -= ph;
            }
            var firstLine = true;
            var contents = cache.GetContents(index);
            foreach (var line in contents)
            {
                if (string.IsNullOrWhiteSpace(line.Text)) continue;
                var height = GetParagraphHeight(canvas, line.Text, dirtyRect.Width);
                if (topOffset + height < 0)
                {
                    topOffset += height + setting.LineSpacing;
                    continue;
                }
                if (firstLine)
                {
                    temp.TopOffset = topOffset;
                    temp.Cursor = line.LineIndex;
                    firstLine = false;
                    Debug.WriteLine($"First TopOffset: {topOffset}, Cursor: {line.LineIndex}");
                }
                canvas.DrawString(line.Text, 0, topOffset, dirtyRect.Width, height, HorizontalAlignment.Left, VerticalAlignment.Top, TextFlow.OverflowBounds, setting.LineSpacing);
                topOffset += height + setting.LineSpacing;
                if (topOffset > dirtyRect.Height)
                {
                    break;
                }
            }
            cache.CheckCacheCapacity(temp.Cursor);
        }

        internal void Init(Book currentBook, BookService service)
        {
            Current = currentBook;
            cache = new CacheContentManager(currentBook, service);
            _ = cache.LoadContentsAsync(currentBook.LineCursor, CancellationToken.None);
            previousInfo = new PreviousInfo
            {
                TopOffset = 0,
                Cursor = currentBook.LineCursor
            };
        }
    }
}
