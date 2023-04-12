using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Utils
{
    internal static class FileResultExtensions
    {
        public static async Task<string> GetFileMd5Value(this FileResult file)
        {
			try
			{
				using var fs = await file.OpenReadAsync();
				using var md5 = MD5.Create();
				var hash = md5.ComputeHash(fs);
				return hash.Aggregate(string.Empty, (res, b) => res + b.ToString("X2"));
			}
			catch
			{
				throw;
			}
        }
    }
}
