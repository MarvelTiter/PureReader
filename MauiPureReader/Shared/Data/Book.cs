using MDbEntity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Data
{
    public class Book
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Column(PrimaryKey = true, AutoIncrement = true)]
        public int Id { get; set; }
        /// <summary>
        /// 书名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long BookSize { get; set; }
        /// <summary>
        /// 阅读进度
        /// </summary>
        public int Progress { get; set; }

        [Ignore]
        public string FormatProgress
        {
            get
            {
                if (BookSize == 0) return 0.ToString("p2");
                return (Progress * 1.0 / BookSize).ToString("p2");
            }
        }
    }
}
