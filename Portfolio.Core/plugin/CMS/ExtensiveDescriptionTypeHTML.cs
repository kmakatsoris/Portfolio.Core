using System.Net;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Portfolio.Core.Types.DTOs.Resources;
using Portfolio.Core.Types.Enums.Resources;
using Portfolio.Core.Utils.DefaultUtils;

namespace Portfolio.Core.CMS
{
    public static class ExtensiveDescriptionTypeHTML
    {
        public static IEnumerable<string> TransformPagesToJSXElements(this List<ExtensiveDescriptionType> extensiveDescriptionTypes)
        {
            var result = new List<string>();
            if (extensiveDescriptionTypes == null || extensiveDescriptionTypes.Count() <= 0 && extensiveDescriptionTypes?.Any(e => e == null) == true)
                return result;
            for (int i = 0; i < extensiveDescriptionTypes.Count(); i++)
            {
                if (extensiveDescriptionTypes[i]?.Location.Equals(ResourcesExtensiveDescriptionLocationEnum.Page) == true)
                {
                    IHtmlContent c = extensiveDescriptionTypes.TransformToJSXElement(i, ResourcesExtensiveDescriptionLocationEnum.Page.GetStringValue());
                    result.Add(JsonConvert.SerializeObject(c?.GetStringFromHtmlContent()));
                }
            }
            return result;
        }

        private static string GetStringFromHtmlContent(this IHtmlContent htmlContent)
        {
            if (htmlContent == null) return "";
            using StringWriter writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        private static IHtmlContent TransformToJSXElement(this List<ExtensiveDescriptionType> extensiveDescriptionTypes, int index, string location)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("container");

            int counter = 0;
            for (int i = 0; i < extensiveDescriptionTypes.Count; i++)
            {
                if (extensiveDescriptionTypes[i].Location.GetStringValue()?.Equals(location) == true)
                {
                    if (counter == index)
                    {
                        for (int j = i; j < extensiveDescriptionTypes.Count; j++)
                        {
                            if (counter == j || extensiveDescriptionTypes[j].Location.GetStringValue()?.Equals(location) != true)
                            {
                                var element = ToPerform(extensiveDescriptionTypes[j], j);
                                container.InnerHtml.AppendHtml(element);
                            }
                        }
                        break;
                    }
                    else
                    {
                        counter++;
                    }
                }
            }

            return container;
        }

        private static IHtmlContent ToPerform(this ExtensiveDescriptionType extensiveDescriptionType, int index)
        {
            var div = new TagBuilder("div");
            div.Attributes.Add("key", index.ToString());

            string className = FindClassname(extensiveDescriptionType.Location);
            div.AddCssClass(className);

            string style = $"color: {extensiveDescriptionType.Color ?? "#000000"}";
            div.Attributes.Add("style", style);

            div.InnerHtml.Append(extensiveDescriptionType.Text);

            return div;
        }

        private static string FindClassname(this ResourcesExtensiveDescriptionLocationEnum location)
        {
            switch (location)
            {
                case ResourcesExtensiveDescriptionLocationEnum.Space:
                    return "spaceContainer";
                case ResourcesExtensiveDescriptionLocationEnum.Line:
                    return "lineContainer";
                case ResourcesExtensiveDescriptionLocationEnum.Paragraph:
                    return "paragraphContainer";
                case ResourcesExtensiveDescriptionLocationEnum.Page:
                    return "pageContainer";
                default:
                    return "defaultContainer";
            }
        }
    }
}
