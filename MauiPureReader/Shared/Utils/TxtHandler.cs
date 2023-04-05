using Shared.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Shared.Utils
{
    public class TxtHandler
    {
        public static void Solve(Stream fs, IList<string> collection)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(fs);                
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
                        sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
                    }
                }
                string line =  sr.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        collection.Add(line);
                    }
                    line = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                _ = Shell.Current.DisplayAlert("文件打开异常", ex.Message, "ok");
            }
            finally
            {
                sr?.Dispose();
            }
        }

        public static async Task Solve(Stream fs, ObservableCollection<Content> collection)
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(fs);
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
                        sr = new StreamReader(fs, Encoding.GetEncoding("GBK"));
                    }
                }
                string line = await sr.ReadLineAsync();
                while (line != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        collection.Add(new Content(line));
                    }
                    line = await sr.ReadLineAsync();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("文件打开异常", ex.Message, "ok");
            }
            finally
            {
                sr?.Dispose();
            }
        }
    }
}
