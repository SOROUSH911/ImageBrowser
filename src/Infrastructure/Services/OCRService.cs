using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageBrowser.Application.Common.Interfaces;
using IronOcr;
using MassTransit.Caching.Internals;
using Microsoft.AspNetCore.Http;

namespace ImageBrowser.Infrastructure.Services;
public class OCRService : IOCRService
{
    public async Task<List<string>> ExtractText(FileStream file)
    {
        string TempPath = Path.GetTempPath();
        string fileName = Path.GetFileName(file.Name);
        string fullName = Path.Combine(TempPath, fileName);
        try
        {

            using (FileStream fsSource = new FileStream(fullName, FileMode.Create))
            {
                file.CopyTo(fsSource);
                fsSource.Close();
            }

          


                var ocrTesseract = new IronTesseract()
                {
                    Language = OcrLanguage.EnglishBest,
                    Configuration = new TesseractConfiguration()
                    {
                        ReadBarCodes = false,
                        RenderHocr = true,
                        BlackListCharacters = "`ë|^",
                        PageSegmentationMode = TesseractPageSegmentationMode.AutoOsd,
                    }
                };

                using var ocrInput = new OcrInput();
                ocrInput.LoadImage(fullName);
                var ocrResult = ocrTesseract.Read(ocrInput);
                Console.WriteLine(ocrResult.Text);
            return ocrResult.Words.Select(w => w.Text).ToList();
        }
        catch(Exception ex) {
            throw new Exception(ex.Message);
        }
        finally
        {
            
        }


        throw new NotImplementedException();
    }
}

