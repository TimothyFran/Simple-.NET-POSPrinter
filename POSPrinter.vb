'*********************************************************************
' LIBRARY NAME: Simple .NET POSPrinter
' DESCRIPTION:  This is a VB.NET library that can be used to easily manage a thermal printer
'               without using ESC/POS commands.
'               This library uses System.Drawing.Printer and it is not needed to use virtual
'               COM port.
' AUTHOR: Timothy Franceschi
' COPYRIGHT: © 2023 Timothy Franceschi
' LICENSE: MIT License
' VERSION: 2.0.0
'*********************************************************************


Imports System.Drawing.Printing
Imports System.Net
Imports System.Runtime.CompilerServices
Imports Newtonsoft.Json.Linq
Imports ZXing
Imports ZXing.Common
Imports ZXing.Windows.Compatibility

Public Class POSPrinter

    Dim myFont As Font
    Dim myBrush As SolidBrush
    Dim lineHeight As Single
    Dim myPoint As New PointF

    Dim e As PrintPageEventArgs

    Public Sub New(setE As PrintPageEventArgs)
        myFont = New Font("Arial", 15, FontStyle.Bold)
        myBrush = New SolidBrush(Color.Black)
        myPoint = New PointF(0, 0)
        lineHeight = myFont.Height
        e = setE
    End Sub

    Public Property Font() As Font
        Set(ByVal newFont As Font)
            myFont = newFont
            lineHeight = myFont.GetHeight * 1.1
        End Set
        Get
            Return myFont
        End Get
    End Property

    Public Property Brush() As SolidBrush
        Set(ByVal newBrush As SolidBrush)
            myBrush = newBrush
        End Set
        Get
            Return myBrush
        End Get
    End Property

    Public Property Point() As PointF
        Get
            Return myPoint
        End Get
        Set(newPoint As PointF)
            myPoint = newPoint
        End Set
    End Property

    Public Sub EmptyLine()
        myPoint.Y += lineHeight / 1.1
        myPoint.X = 0
    End Sub

    Public Sub NewLine()
        myPoint.Y += lineHeight
        myPoint.X = 0
    End Sub

    Public Sub PrintHLine()
        Dim myPen As New Pen(Color.Black)
        Dim startPoint As New PointF(0, myPoint.Y)
        Dim endPoint As New PointF(e.MarginBounds.Right, myPoint.Y)
        e.Graphics.DrawLine(myPen, startPoint, endPoint)
        myPoint = New PointF(0, myPoint.Y + (lineHeight * 0.8))
    End Sub

    Public Sub PrintBarcode(ByVal testo As String, ByVal options As EncodingOptions, ByVal Optional format As BarcodeFormat = BarcodeFormat.CODE_128)

        Dim writer As New BarcodeWriter() With {
            .Format = format,
            .Options = options
        }

        ' Genera immagine codice a barre
        Dim barcodeImg As Bitmap = writer.Write(testo)

        ' Stampa immagine
        e.Graphics.DrawImage(barcodeImg, myPoint)

    End Sub

    Public Sub PrintText(ByVal testo As String, Optional align As String = "left")
        Dim textPos As PointF
        Select Case align
            Case "center"
                textPos = CenterPosition(testo)
            Case "right"
                textPos = RightPosition(testo)
            Case Else
                textPos = myPoint
        End Select
        e.Graphics.DrawString(testo, myFont, myBrush, textPos)

        Dim textSize As SizeF = e.Graphics.MeasureString(testo, myFont)
        myPoint.X += textSize.Width

    End Sub

    Public Sub PrintTextLn(testo As String, Optional align As String = "left")

        Dim textPos As PointF
        Select Case align
            Case "center"
                textPos = CenterPosition(testo)
            Case "right"
                textPos = RightPosition(testo)
            Case Else
                textPos = myPoint
        End Select
        e.Graphics.DrawString(testo, myFont, myBrush, textPos)

        myPoint.Y += lineHeight / 1.1
        myPoint.X = 0

    End Sub

    Private Function CenterPosition(testo As String) As PointF

        'Calcola dimensioni testo
        Dim textSize As SizeF = e.Graphics.MeasureString(testo, myFont)

        Dim freeSpace As Single = e.PageSettings.PaperSize.Width - textSize.Width

        'Calcola coordinata X del testo
        Dim centerX As Single = ((e.PageSettings.PaperSize.Width - textSize.Width) / 2) - (e.PageSettings.Margins.Left / 2)

        Return New PointF(centerX, myPoint.Y)

    End Function

    Private Function RightPosition(testo As String) As PointF

        'Calcola dimensioni testo
        Dim textSize As SizeF = e.Graphics.MeasureString(testo, myFont)

        Dim freeSpace As Single = e.PageSettings.PaperSize.Width - textSize.Width

        'Calcola coordinata X del testo
        Dim centerX As Single = ((e.PageSettings.PaperSize.Width - textSize.Width) / 2)

        Return New PointF(centerX, myPoint.Y)

    End Function

End Class
