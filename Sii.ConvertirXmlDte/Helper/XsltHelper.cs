using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Xsl;
using Humanizer;
using Pdf417EncoderLibrary;

namespace Sii.ConvertirXmlDte.Helper;

public partial class XsltHelper
{
    private readonly XslCompiledTransform xlstFile = new();
    private readonly XsltArgumentList xlstArgs = new();

    public XsltHelper(string xsltContent)
    {
        using StringReader sr = new(xsltContent);
        using XmlReader xr = XmlReader.Create(sr, new XmlReaderSettings());
        xlstFile.Load(xr);
    }

    private string AddTimbreXml(
        string xml,
        string timbre417,
        string? res_nr,
        string res_fecha,
        CancellationToken token = default
    )
    {
        token.ThrowIfCancellationRequested();
        string totalEnPalabras = string.Empty;
        Match match = MyRegex().Match(xml);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int total))
            totalEnPalabras = total.ToWords(new CultureInfo("es-CL"));

        xlstArgs.Clear();
        xlstArgs.AddParam("TedTimbre", "", $"data:image/png;base64,{timbre417}");
        xlstArgs.AddParam("ResNum", "", res_nr!);
        xlstArgs.AddParam("ResYear", "", res_fecha);
        xlstArgs.AddParam("TotalPalabras", "", $"{totalEnPalabras} Pesos.");
        token.ThrowIfCancellationRequested();

        using StringReader sr = new(xml);
        using XmlReader xr = XmlReader.Create(sr, new XmlReaderSettings());
        XmlWriterSettings settings = new()
        {
            Encoding = Encoding.GetEncoding("ISO-8859-1"),
            Indent = true,
        };

        using MemoryStream ms = new();
        using (XmlWriter writer = XmlWriter.Create(ms, settings))
            xlstFile.Transform(xr, xlstArgs, writer);

        token.ThrowIfCancellationRequested();
        return Encoding.GetEncoding("ISO-8859-1").GetString(ms.ToArray());
    }
    public static string ExtractTED(string xml)
    {
        XmlDocument xmlDoc = new();
        xmlDoc.LoadXml(xml);

        // Busca el nodo TED en cualquier parte del documento
        XmlNamespaceManager nsMgr = new(xmlDoc.NameTable);
        nsMgr.AddNamespace("d", xmlDoc.DocumentElement?.NamespaceURI ?? "");

        XmlNode? tedNode = xmlDoc.SelectSingleNode("//d:TED", nsMgr);


        if (tedNode == null)
        {
            Console.WriteLine("Failed to find TED element");
            return null!;
        }

        return tedNode.OuterXml.Trim();
    }
    private static string Pdf417EncoderLibrary(string tedNode)
    {
        Pdf417Encoder encoder = new()
        {
            ErrorCorrection = ErrorCorrectionLevel.Level_5,
            EncodingControl = EncodingControl.ByteOnly,
            GlobalLabelIDCharacterSet = "iso-8859-1",
        };

        encoder.Encode(tedNode);
        encoder.WidthToHeightRatio(3.1d);

        using MemoryStream ms = new();
        encoder.SaveBarcodeToPngFile(ms);
        return Convert.ToBase64String(ms.ToArray());
    }

    private static string FormatTedNode(string tedNode)
    {
        tedNode = MyRegex1().Replace(tedNode, "><");
        return tedNode;
    }

    [GeneratedRegex(@"<MntTotal>(\d+)</MntTotal>")]
    private static partial Regex MyRegex();

    [GeneratedRegex(@">\s+<")]
    private static partial Regex MyRegex1();

    public string GenerateHtml(
     string xml,
     DateOnly? date_res = default!,
     int nr_res = 80,
     CancellationToken token = default
 )
    {
        try
        {
            DateOnly fechaResolucion = date_res ?? new DateOnly(2014, 8, 22);

            string? nodeTed = ExtractTED(xml);
            if (nodeTed is null)
                throw new Exception("Error al extraer el nodo TED.");

            string timbre417 = Pdf417EncoderLibrary(FormatTedNode(nodeTed))
                ?? throw new Exception("Error al generar el timbre.");

            token.ThrowIfCancellationRequested();

            return AddTimbreXml(
                xml,
                timbre417,
                nr_res.ToString(),
                fechaResolucion.ToString("dd-MM-yyyy"),
                token
            );
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return null!;
        }
    }


}
