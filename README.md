[![](https://img.shields.io/badge/License-GPLv3-blue.svg?style=for-the-badge)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/w/sergiokml/Sii.ConvertirXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ConvertirXmlDte)
[![GitHub contributors](https://img.shields.io/github/contributors/sergiokml/Sii.ConvertirXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ConvertirXmlDte/graphs/contributors/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/sergiokml/Sii.ConvertirXmlDte?style=for-the-badge)](https://github.com/sergiokml/Sii.ConvertirXmlDte)

# Convertir XML DTE a HTML

This solution transforms a DTE (Electronic Tax Document) XML into a HTML format using XSLT and embeds a PDF417 barcode extracted from the TED node. It is compliant with Chilean SII ([Servicio de Impuestos Internos](https://www.sii.cl/)) document structure.

It includes:

- Minimal API for converting DTE XML to HTML
- XSLT template loading from Azure Blob Storage
- HTML output encoded in UTF-8 for full compatibility

> This repository has no relationship with the government entity [SII](https://www.sii.cl/), only for educational purposes.

---

### 📦 Details

| Package Reference            | Version |
|-----------------------------|:-------:|
| Azure.Storage.Blobs         | 12.24.0 |
| Microsoft.Extensions.Azure  | 1.7.6   |
| Pdf417EncoderLibrary        | latest  |

---

### 🚀 Usage

Once the app is running, you can convert XML DTEs via:

```bash
curl -X POST http://localhost:5230/api/dte/genera-html/ \
  -H "Content-Type: application/xml" \
  -H "Accept: text/html" \
  
<?xml version="1.0" encoding="iso-8859-1"?>
<DTE version="1.0" xmlns="http://www.sii.cl/SiiDte">
  ...

</DTE>
```

The response will be HTML output with the DTE information rendered, including a PDF417 barcode and optional resolution metadata.

<p align="center">
  <img src="https://img001.prntscr.com/file/img001/6V6VhlGVS-6RTvHbIHXOaw.png" width="60%" />
</p>


### ⚙️ Configuration

Use `appsettings.json` or environment variables to configure the XSLT template source:

```json
{
  "StorageConnection": "UseDevelopmentStorage=true",
  "StorageConnection:ContainerName": "templates",
  "StorageConnection:BlobName": "Documento.xlst"
}
```

You may also define these as [Azure App Settings](https://learn.microsoft.com/en-us/azure/app-service/configure-common) if you're deploying the API to the cloud.

---

### 📣 Have a question? Found a Bug?

Feel free to **file a new issue** with a respective title and description on the [Sii.ConvertirXmlDte/issues](https://github.com/sergiokml/Sii.ConvertirXmlDte/issues) repository.

---

### 💖 Community and Contributions

If this tool was useful, consider contributing with ideas or improving it further.

<p align="center">
    <a href="https://www.paypal.com/donate/?hosted_button_id=PTKX9BNY96SNJ" target="_blank">
        <img width="12%" src="https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white" alt="Donate">
    </a>
</p>

---

### 📘 License

This repository is released under the [GNU General Public License v3.0](LICENSE.txt).

