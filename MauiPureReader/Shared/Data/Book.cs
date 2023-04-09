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
        public int BookSize { get; set; }
        /// <summary>
        /// 阅读进度
        /// </summary>
        public int FirstLine { get; set; }
        /// <summary>
        /// 阅读进度
        /// </summary>
        public int LastLine { get; set; }

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
        /// MD5
        /// </summary>
        public string MD5 { get; set; }
        /// <summary>
        /// 是否解析完成
        /// </summary>
        public bool Done { get; set; }

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
