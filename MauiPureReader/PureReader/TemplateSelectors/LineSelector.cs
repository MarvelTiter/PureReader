using Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PureReader.TemplateSelectors
{
    internal class LineSelector : DataTemplateSelector
    {
        public DataTemplate TitleTemplate { get; set; }
        public DataTemplate LineContentTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var c = (Content)item;
            return c.IsTitle ? TitleTemplate : LineContentTemplate;
        }
    }
}
