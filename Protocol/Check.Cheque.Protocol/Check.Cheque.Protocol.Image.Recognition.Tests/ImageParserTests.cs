using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Check.Cheque.Protocol.Image.Recognition.Tests
{
  [TestClass]
  public class ImageParserTests
  {
    [TestMethod]
    public async Task ParseImageTest()
    {
      var parsedImage = await ImageParser.Parse("C:\\Projects\\Odyssey\\IMG_20190413_170510.jpg");

      var kvkNumber = string.Empty;
      var bankAccount = string.Empty;

      foreach (var imageRegion in parsedImage.regions)
      {
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
        }
      }
    }
  }
}
