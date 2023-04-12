using MDbEntity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Shared.Data
{
    public class Book
    {
        /// <summary>
        /// 主键ID (MD5)
        /// </summary>
        [Column(PrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 书名
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int BookSize { get; set; }

        private int lineCursor = -1;
        public int LineCursor
        {
            get => lineCursor; set
            {
                if (value < 0) value = 0;
                lineCursor = value;
            }
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 已缓存索引
        /// </summary>
        public int CacheIndex { get; set; }

        /// <summary>
        /// 是否解析完成
        /// </summary>
        [Ignore]
        public bool Done { get => CacheIndex == BookSize && BookSize > 0 && CacheIndex > 0; }

        [Ignore]
        public string FormatProgress
        {
            get
            {
                if (BookSize == 0) return 0.ToString("p2");
                return (LineCursor * 1.0 / BookSize).ToString("p2");
            }
        }
    }
}
