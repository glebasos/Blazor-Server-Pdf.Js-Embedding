# Blazor Server + PDF.js Example

This repository demonstrates how to **embed PDF.js into a Blazor Server application** and display PDFs in the browser.

---

## 📦 Setup Instructions

### 1. Download PDF.js
Download the latest [PDF.js release](https://github.com/mozilla/pdf.js/releases).

Copy the following folders into your project under `wwwroot/pdfjs/`:
- `build/`
- `web/`

Your structure should look like:

```
wwwroot/
 └── pdfjs/
     ├── build/
     └── web/
```

---

### 2. Configure MIME Types
Blazor/IIS may not serve `.properties` and `.ftl` files required by PDF.js by default.  
Add custom MIME type mappings in `Program.cs`:

```csharp

// Configure MIME types for PDF.js
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".properties"] = "application/octet-stream";
provider.Mappings[".ftl"] = "application/octet-stream";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});

```

---

### 3. Embed PDF.js Viewer in Blazor
Use an `<iframe>` to point to the PDF.js `viewer.html` and load your PDF file:

```razor
<div class="pdf-viewer-container">
    <iframe id="pdfViewerIframe"
            src="@GetPdfViewerUrl()"
            width="100%"
            height="800px">
    </iframe>
</div>
```

```csharp

private string pdfUrl { get; set; } = string.Empty;

protected override async Task OnInitializedAsync()
{
    // Example: load from API endpoint
    pdfUrl = "/api/pdf/invoice-batch/";
    StateHasChanged();
}

private string GetPdfViewerUrl()
{
    var baseUrl = $"/pdfjs/web/viewer.html?file={Uri.EscapeDataString(pdfUrl)}";
    return baseUrl;
}

```