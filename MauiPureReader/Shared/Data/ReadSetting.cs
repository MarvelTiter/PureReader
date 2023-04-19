using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Data
{
    public class ReadSetting
    {
        public float FontSize { get; set; } = 18f;
        public float LineSpacing { get; set; } = 18f;
        public string FontString { get; set; }

        public int Margin { get; set; } = 10;

        public Microsoft.Maui.Graphics.Font Font => string.IsNullOrEmpty(FontString) ? Microsoft.Maui.Graphics.Font.Default : new Microsoft.Maui.Graphics.Font(FontString);
    }
}
