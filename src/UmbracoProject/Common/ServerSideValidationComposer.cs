using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Security;

namespace UmbracoProject.Common
{
    public class ServerSideValidationComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddSingleton<IFileStreamSecurityAnalyzer, SvgXssSecurityAnalyzer>();
        }
    }
}
