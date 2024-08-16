using System.Runtime.Serialization;

namespace Portfolio.Core.Types.DataTypes.Resources
{
    [DataContract]
    public class PreviewData
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "caption")]
        public string Caption { get; set; }

        [DataMember(Name = "width")]
        public string Width { get; set; }

        [DataMember(Name = "height")]
        public string Height { get; set; }
    }
}