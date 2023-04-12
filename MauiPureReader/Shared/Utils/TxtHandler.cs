using Microsoft.Maui.Controls.Shapes;
using Microsoft.VisualBasic;
using Shared.Data;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Shared.Utils
{
    public interface IFileHandler
    {
        Task Solve(Book book, CancellationToken token);
    }
    public class TxtHandler : IFileHandler
    {
        private readonly BookService bookService;
        private readonly FileService fileService;

        //public static async Task Solve(Stream fs, IList<Content> collection)
        //{
        //    StreamReader sr = null;
        //    try
        //    {
        //        sr = new StreamReader(fs);
        //        if (sr.CurrentEncoding == Encoding.UTF8)
        //        {
        //            var chArr = new char[1024];
        //            sr.Read(chArr, 0, chArr.Length);
        //            var buffer1 = Encoding.UTF8.GetBytes(chArr);
        //            var buffer2 = new byte[buffer1.Length];
        //            fs.Position = 0;
        //            fs.Read(buffer2, 0, buffer2.Length);
        //            var same = true;
        //            for (int i = 0; i < buffer1.Length; i++)
        //            {
        //                if (buffer1[i] != buffer2[i])
        //                {
        //                    same = false;
        //                    break;
        //                }
        //            }
        //            if (!same)
        //            {
        //                fs.Position = 0;
        //                sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
        //            }
        //        }
        //        var buffer = new byte[fs.Length];
        //        fs.Read(buffer, 0, buffer.Length);
        //        sr = new StreamReader(new MemoryStream(buffer), Encoding.GetEncoding("GBK"));
        //        while (sr.Peek() > -1)
        //        {
        //            var line = await sr.ReadLineAsync();
        //            if (!string.IsNullOrWhiteSpace(line))
        //            {
        //                collection.Add(new Content(line));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = Shell.Current.DisplayAlert("文件打开异常", ex.Message, "ok");
        //    }
        //    finally
        //    {
        //        sr?.Dispose();
        //    }
        //}

        //public static async Task Solve(Stream fs, ObservableCollection<Content> collection)
        //{
        //    StreamReader sr = null;
        //    try
        //    {
        //        sr = new StreamReader(fs);
        //        if (sr.CurrentEncoding == Encoding.UTF8)
        //        {
        //            var chArr = new char[1024];
        //            sr.Read(chArr, 0, chArr.Length);
        //            var buffer1 = Encoding.UTF8.GetBytes(chArr);
        //            var buffer2 = new byte[buffer1.Length];
        //            fs.Position = 0;
        //            fs.Read(buffer2, 0, buffer2.Length);
        //            var same = true;
        //            for (int i = 0; i < buffer1.Length; i++)
        //            {
        //                if (buffer1[i] != buffer2[i])
        //                {
        //                    same = false;
        //                    break;
        //                }
        //            }
        //            if (!same)
        //            {
        //                fs.Position = 0;
        //                sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
        //            }
        //        }
        //        //var all = await sr.ReadToEndAsync();
        //        while (sr.Peek() > -1)
        //        {
        //            string line = await sr.ReadLineAsync();
        //            if (!string.IsNullOrWhiteSpace(line))
        //            {
        //                collection.Add(new Content(line));
        //            }
        //            //line = await sr.ReadLineAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Shell.Current.DisplayAlert("文件打开异常", ex.Message, "ok");
        //    }
        //    finally
        //    {
        //        sr?.Dispose();
        //    }
        //}

        public TxtHandler(BookService bookService, FileService fileService)
        {
            this.bookService = bookService;
            this.fileService = fileService;
        }

        public Task Solve(Book book, CancellationToken token)
        {
            return Task.Run(async () =>
            {
                var fs = fileService.OpenFile(book.FilePath);
                StreamReaderWithRealPosition sr = null;
                List<Content> temp = new();
                try
                {
                    book.BookSize = fs.Length;
                    sr = OpenStreamWithEncoding(fs);
                    sr.Seek(book.Offset);
                    while (sr.Peek() > -1)
                    {
                        if (token.IsCancellationRequested)
                        {
                            break;
                        }
                        var line = sr.ReadLine();
                        
                        var match = line.ExtractChapter();
                        //if (match.Success)
                        //{

                        //}
                        temp.Add(new Content
                        {
                            BookId = book.Id,
                            LineIndex = book.Lines,
                            Text = line,
                            IsTitle = match.Success,
                        });
                        book.Lines++;
                        if (temp.Count == 10)
                        {
                            await bookService.SaveContents(temp);
                            temp.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = Shell.Current.DisplayAlert("文件打开异常", ex.Message, "ok");
                }
                finally
                {
                    book.Offset = sr.Position;
                    sr?.Dispose();
                    fs?.Dispose();
                    if (temp.Count > 0)
                    {
                        await bookService.SaveContents(temp);
                    }
                    await bookService.UpdateBookInfo(book);
                }
            }, token);
        }

        private static StreamReaderWithRealPosition OpenStreamWithEncoding(Stream fs)
        {
            var sr = new StreamReader(fs);
            var encoding = sr.CurrentEncoding;
            if (sr.CurrentEncoding == Encoding.UTF8)
            {
                var chArr = new char[1024];
                sr.Read(chArr, 0, chArr.Length);
                var buffer1 = Encoding.UTF8.GetBytes(chArr);
                var buffer2 = new byte[buffer1.Length];
                fs.Position = 0;
                fs.Read(buffer2, 0, buffer2.Length);
                var same = true;
                for (int i = 0; i < buffer1.Length; i++)
                {
                    if (buffer1[i] != buffer2[i])
                    {
                        same = false;
                        break;
                    }
                }
                if (!same)
                {
                    fs.Position = 0;
                    return new StreamReaderWithRealPosition(fs, Encoding.GetEncoding("GB18030"));
                }
            }
            return new StreamReaderWithRealPosition(fs, encoding);
        }
    }
}
