using iText.IO.Font;
using iText.Kernel.Font;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DigitalDoor.Reporting.Utilities
{
    internal class FontService
    {
        readonly HttpClient HttpClient;

        public FontService(HttpClient httpClient)
        {
          HttpClient=httpClient;
        }

        ConcurrentDictionary<string,PdfFont> DictionaryFonts = new ConcurrentDictionary<string,PdfFont>();  
        public async Task<PdfFont> GetFontByName(string name)
        {
            byte[] Result;
            PdfFont Font = null; 
            if (DictionaryFonts.ContainsKey(name))
            {
                Font = DictionaryFonts[name];   
            }
            else
            {
                Result =  await HttpClient.GetFromJsonAsync<byte[]>($"https://localhost:7167/Fonts/{name}");
                Font = PdfFontFactory.CreateFont(Result, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.FORCE_NOT_EMBEDDED);
                DictionaryFonts.TryAdd(name, Font);
            }
            return Font;
        }
    }
}
