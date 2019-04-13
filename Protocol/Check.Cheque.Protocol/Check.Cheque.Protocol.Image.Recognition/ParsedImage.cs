using System;
using System.Collections.Generic;
using System.Text;

namespace Check.Cheque.Protocol.Image.Recognition
{
  internal class Word
  {
    public string boundingBox { get; set; }
    public string text { get; set; }
  }

  internal class Line
  {
    public string boundingBox { get; set; }
    public List<Word> words { get; set; }
  }

  internal class Region
  {
    public string boundingBox { get; set; }
    public List<Line> lines { get; set; }
  }

  internal class ParsedImage
  {
    public string language { get; set; }
    public double textAngle { get; set; }
    public string orientation { get; set; }
    public List<Region> regions { get; set; }
  }
}
