
using System.Text;

namespace Shared.Services;

public class FileService
{
    public Task<IEnumerable<FileResult>> PickerFilesAsync()
    {
        var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "public.plain-text" } }, // UTType values
                    { DevicePlatform.Android, new[] { "text/plain", "application/epub+zip" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".txt", ".epub" } }, // file extension
                });
        return FilePicker.Default.PickMultipleAsync(new PickOptions
        {
            FileTypes = customFileType
        });
    }

    public Stream OpenFile(string file)
    {
        return File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}