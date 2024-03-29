using Umbraco.Cms.Core.Security;

namespace UmbracoProject.Common
{
    public class SvgXssSecurityAnalyzer : IFileStreamSecurityAnalyzer
    {
        public bool IsConsideredSafe(Stream fileStream)
        {
            var streamReader = new StreamReader(fileStream);
            var fileContent = streamReader.ReadToEnd();
            return !(fileContent.Contains("<script") && fileContent.Contains("/script>"));
        }

        public bool ShouldHandle(Stream fileStream)
        {
            //reduce memory footprint by partially reading the file
            var startBuffer = new byte[256];
            var endBuffer = new byte[256];
            fileStream.Read(startBuffer);
            if(endBuffer.Length > fileStream.Length)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }
            else
            {
                fileStream.Seek(fileStream.Length - endBuffer.Length, SeekOrigin.Begin);
            }
            fileStream.Read(endBuffer);
            var startString = System.Text.Encoding.UTF8.GetString(startBuffer);
            var endString = System.Text.Encoding.UTF8.GetString(endBuffer);
            return startString.Contains("<svg")
                   && startString.Contains("xmlns=\"http://www.w3.org/2000/svg\"")
                   && endString.Contains("/svg>");
        }
    }
}
