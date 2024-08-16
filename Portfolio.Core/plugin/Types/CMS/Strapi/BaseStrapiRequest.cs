using System.Runtime.Serialization;

namespace Portfolio.Core.CMS.Strapi
{
    [DataContract]
    public class BaseStrapiRequest
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "renderPath")]
        public string RenderPath { get; set; }

        [DataMember(Name = "enSync")]
        public bool EnSync { get; set; }
    }
}