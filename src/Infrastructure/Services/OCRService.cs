using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using MassTransit.Caching.Internals;
using Microsoft.AspNetCore.Http;
using Tesseract;

namespace ImageBrowser.Infrastructure.Services;
public class OCRService : IOCRService
{
    private readonly IFileProvider _fileProvider;

    public OCRService(IFileProvider fileProvider)
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




            using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default))
            {
                // Load the image
                using (var image = Pix.LoadFromFile(fullName))
                {
                    // Perform OCR
                    var result = engine.Process(image);

                    // Print extracted text
                    Console.WriteLine("Extracted Text:");
                    var extractedText = result.GetText();
                    Console.WriteLine(extractedText);
                    return extractedText;
                }
            }
        }
        catch(Exception ex) {
            throw new Exception(ex.Message);
        }
        finally
        {
            fileStream.Stream.Dispose();
            File.Delete(fullName);
        }
    }
}

