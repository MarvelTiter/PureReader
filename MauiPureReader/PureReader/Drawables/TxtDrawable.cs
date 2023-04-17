using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.Drawables
{
    internal class TxtDrawable : IDrawable
    {
        private readonly IList<Content> lines;

        public TxtDrawable(IList<Content> lines)
        {
            this.lines = lines;
        }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            //canvas.SetFillPaint()
           
        }
    }
}
