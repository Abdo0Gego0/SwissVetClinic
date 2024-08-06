namespace ServicesLibrary.PrintServices
{
    using DinkToPdf;
    using DinkToPdf.Contracts;

    public class PdfConversionService
    {
        private readonly IConverter _converter;

        public PdfConversionService(IConverter converter)
        {
            _converter = converter;
        }

        public byte[] ConvertHtmlToPdf(string html)
        {
            var document = new HtmlToPdfDocument
            {
                
                Objects =
                    {
                        new ObjectSettings
                        {
                            HtmlContent = html,
                            WebSettings =new WebSettings
                            {
                                LoadImages= true,
                                DefaultEncoding="utf-8",
                                EnableJavascript=true,
                                
                            },
                            PagesCount=true,
                            UseExternalLinks=true,
                            UseLocalLinks=true,
                            
                            
                        },
                    }
                
            };
            document.GlobalSettings.Margins.Bottom = 15;
            document.GlobalSettings.Margins.Top = 15;

            return _converter.Convert(document);
        }
    }

}
