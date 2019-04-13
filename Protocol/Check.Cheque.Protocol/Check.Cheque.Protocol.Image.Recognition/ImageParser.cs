using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Check.Cheque.Protocol.Core.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tangle.Net.Entity;

namespace Check.Cheque.Protocol.Image.Recognition
{
  public static class ImageParser
  {
    private const string SubscriptionKey = "8ee262d67631426ea2ce716529848bb9";
    private const string UriBase =
        "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr";

    public static async Task<Invoice> Parse(string imageFilePath)
    {
      try
      {
        var client = new HttpClient();

        // Request headers.
        client.DefaultRequestHeaders.Add(
            "Ocp-Apim-Subscription-Key", SubscriptionKey);

        var requestParameters = "language=unk&detectOrientation=true";
        var uri = UriBase + "?" + requestParameters;

        HttpResponseMessage response;
        var byteData = GetImageAsByteArray(imageFilePath);

        using (var content = new ByteArrayContent(byteData))
        {
          content.Headers.ContentType =
              new MediaTypeHeaderValue("application/octet-stream");

          response = await client.PostAsync(uri, content);
        }

        var contentString = await response.Content.ReadAsStringAsync();
        var parsedImage = JsonConvert.DeserializeObject<ParsedImage>(contentString);

        return ExtractInvoice(parsedImage);
      }
      catch (Exception e)
      {
        throw;
      }
    }

    private static Invoice ExtractInvoice(ParsedImage parsedImage)
    {
      var kvkNumber = string.Empty;
      var bankAccount = string.Empty;
      var hash = string.Empty;

      foreach (var imageRegion in parsedImage.regions)
      {
        var i = 0;
        foreach (var line in imageRegion.lines)
        {
          if (line.words.Any(l => l.text.ToLower().Contains("kvk")))
          {
            kvkNumber = line.words[1].text;
          }

          if (line.words.Any(l => l.text.ToLower().Contains("bank")))
          {
            bankAccount = line.words[1].text + line.words[2].text + line.words[3].text + line.words[4].text + line.words[5].text;
          }

          if (line.words.Any(l => l.text.ToLower().Contains("hash")))
          {
            hash = imageRegion.lines[i + 1].words.First().text + imageRegion.lines[i + 2].words.Last().text;
          }

          i++;
        }
      }

      hash = hash.Replace('1', 'I');
      hash = hash.Replace('l', 'I');

      return new Invoice {Hash = new Hash(hash), KvkNumber = kvkNumber, Payload = Encoding.UTF8.GetBytes(bankAccount)};
    }

    private static byte[] GetImageAsByteArray(string imageFilePath)
    {
      // Open a read-only file stream for the specified file.
      using (FileStream fileStream =
          new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
      {
        // Read the file's contents into a byte array.
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
      }
    }
  }
}
