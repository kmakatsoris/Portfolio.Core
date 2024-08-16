using System.Runtime.Serialization;

namespace Portfolio.Core.Types.Context
{
    [DataContract]
    public class AppSettings
    {
        [DataMember(Name = "AllowedHosts")]
        public string AllowedHosts { get; set; }

        [DataMember(Name = "JwtConfig")]
        public JwtConfig JwtConfig { get; set; }

        [DataMember(Name = "ConnectionStrings")]
        public ConnectionStrings ConnectionStrings { get; set; }

        [DataMember(Name = "CMS_Strapi_RenderPathsAndCollections")]
        public CMS_Strapi_RenderPathsAndCollections CMS_Strapi_RenderPathsAndCollections { get; set; }
    }

    [DataContract]
    public class JwtConfig
    {
        [DataMember(Name = "IssuerSigningKey")]
        public string IssuerSigningKey { get; set; }

        [DataMember(Name = "ValidIssuer")]
        public string ValidIssuer { get; set; }

        [DataMember(Name = "ValidAudience")]
        public string ValidAudience { get; set; }

        [DataMember(Name = "JwtTime")]
        public string JwtTime { get; set; } // Minutes
    }

    [DataContract]
    public class ConnectionStrings
    {
        [DataMember(Name = "MySqlConnection")]
        public string IdentityConnection { get; set; }

        [DataMember(Name = "StrapiConnection")]
        public string StrapiConnection { get; set; }
    }

    [DataContract]
    public class CMS_Strapi_RenderPathsAndCollections
    {
        [DataMember(Name = "HomePage")]
        public string HomePage { get; set; }

        [DataMember(Name = "BioPage")]
        public string BioPage { get; set; }

        [DataMember(Name = "SkillsPage")]
        public string SkillsPage { get; set; }

        [DataMember(Name = "ProjectsPage")]
        public string ProjectsPage { get; set; }

        [DataMember(Name = "APIsPage")]
        public string APIsPage { get; set; }

        [DataMember(Name = "ContactsPage")]
        public string ContactsPage { get; set; }
    }

}