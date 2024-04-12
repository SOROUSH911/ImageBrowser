using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;

namespace ImageBrowser.Infrastructure.Services;
public class IronOCRService : IOCRService
{
    private readonly IFileProvider _fileProvider;

    public IronOCRService(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }
    public async Task<string> ExtractText(string key)
    {
        var fileStream = await _fileProvider.GetObjectFromS3Async(key);



        string TempPath = Path.GetTempPath();
        string fileName = Path.GetFileName(fileStream.FileName);
        string fullName = Path.Combine(TempPath, fileName);

        string responseBody;

        try
        {

            using (FileStream fsSource = new FileStream(fullName, FileMode.Create))
            {
                await fileStream.Stream.CopyToAsync(fsSource);
                fsSource.Close();
            }




            var Ocr = new IronOcr.IronTesseract();
            using (var Input = new IronOcr.OcrInput())
            {
                Input.LoadImage(fullName);
                var Result = Ocr.Read(Input);
                return Result.Text;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
        finally
        {
            fileStream.Stream.Dispose();
            File.Delete(fullName);
        }
    }

}
