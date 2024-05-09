using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.PeerToPeer.Collaboration;
using System.Text;
using System.Threading.Tasks;
using BarcodeStandard;
using SkiaSharp;

namespace YourNamespace;

public class POSPrinter
{

    Font myFont = new("Arial", 15, FontStyle.Bold);
    SolidBrush myBrush = new(Color.Black);
    PointF myPoint = new(0, 0);

    Single lineHeight { get; set; }


    PrintPageEventArgs ppea;

    public POSPrinter(PrintPageEventArgs setE)
    {
        lineHeight = myFont.Height;
        ppea = setE;
    }

    public void PrintHLine()
    {
        Pen pen = new(Color.Black);
        PointF startPoint = new(0, myPoint.Y);
        PointF endPoint = new(ppea.PageBounds.Width, myPoint.Y);
        ppea.Graphics!.DrawLine(pen, startPoint, endPoint);
        myPoint.X = 0;
        myPoint.Y += (float)(lineHeight * 0.8);
    }

    public void EmptyLine()
    {
        myPoint.Y += (float)(lineHeight / 1.1);
        myPoint.X = 0;
    }

    public void NewLine()
    {
        myPoint.Y += (float)(lineHeight);
        myPoint.X = 0;
    }


    public void PrintText(string text, TextAlign align = TextAlign.LEFT)
    {

        PointF textPos;
        switch(align)
        {
            case TextAlign.LEFT:
                textPos = myPoint;
                break;
            case TextAlign.RIGHT:
                textPos = RightPosition(text);
                break;
            case TextAlign.CENTER:
                textPos = CenterPosition(text);
                break;
            default:
                return;
        }

        ppea.Graphics!.DrawString(text, myFont, myBrush, textPos);

        SizeF textSize = ppea.Graphics!.MeasureString(text, myFont);
        myPoint.X += textSize.Width;

    }

    public void PrintTextLn(string text, TextAlign align = TextAlign.LEFT)
    {
        PrintText(text, align);
        myPoint.X = 0;
        myPoint.Y += (float)(lineHeight / 1.1);
    }


    public void PrintBarcode(string value)
    {

        SKFont labelFont = new()
        {
            Size = 10
        };
        Barcode b = new()
        {
            IncludeLabel = true,
            LabelFont = labelFont
        };

        SKImage img = b.Encode(BarcodeStandard.Type.Code128, value, SKColors.Black, SKColors.White, 150, 50);
        SKData data = img.Encode(SKEncodedImageFormat.Png, 50);
        Stream barcodeImageStream = data.AsStream();

        Image image = Image.FromStream(barcodeImageStream);

        myPoint.X = 0;
        ppea.Graphics!.DrawImage(image, myPoint);

        myPoint.X = 0;
        myPoint.Y += 75;
    }
    

    private PointF CenterPosition(string text)
    {
        SizeF textSize = ppea.Graphics!.MeasureString(text, myFont);
        Single freeSpace = ppea.PageSettings.PaperSize.Width - textSize.Width;
        Single centerX = (freeSpace / 2) - (ppea.PageSettings.Margins.Left / 6);
        return new PointF(centerX, myPoint.Y);
    }

    private PointF RightPosition(string text)
    {
        SizeF textSize = ppea.Graphics!.MeasureString(text, myFont);
        Single freeSpace = ppea.PageSettings.PaperSize.Width - textSize.Width;
        Single positionX = (freeSpace / 2) - (ppea.PageSettings.Margins.Left / 2);
        return new PointF(positionX, myPoint.Y);
    }

    

    public Font Font
    {
        get { return myFont; }
        set { myFont = value; lineHeight = myFont.Height; }
    }

    public SolidBrush Brush
    {
        get { return myBrush; }
        set { myBrush = value; }
    }

}

public enum TextAlign
{
    RIGHT = 0,
    LEFT = 1,
    CENTER = 2
}
