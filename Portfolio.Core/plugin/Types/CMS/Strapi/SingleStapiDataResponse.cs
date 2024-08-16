using System;
using System.Collections.Generic;

namespace Portfolio.Core.CMS.Strapi
{
    public class MultipleStrapiDataResponse
    {
        public IEnumerable<Data> Data { get; set; }
        public Meta Meta { get; set; }
    }
    public class SingleStrapiDataResponse
    {
        public Data Data { get; set; }
        public Meta Meta { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public Attributes Attributes { get; set; }
    }

    public class Attributes
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public string Tags { get; set; }
        public string FullPath { get; set; }
        public StrapiAttributesMetaData MetaData { get; set; }
        public PreviewData PreviewData { get; set; }
    }

    public class StrapiAttributesMetaData
    {
        public string BriefDescription { get; set; }
        public string ExtensiveDescription { get; set; }
        public string TAG { get; set; }
        public Profile Profile { get; set; }
        public List<WorkExperience> WORK_EXPERIENCE { get; set; }
        public List<Education> EDUCATION { get; set; }
        public List<ProgrammingSkill> PROGRAMMING_SKILLS { get; set; }
        public List<Volunteering> VOLUNTEERING { get; set; }
        public List<Contact> CONTACT { get; set; }
        public List<Language> LANGUAGES { get; set; }
    }

    public class Profile
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class WorkExperience
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class Education
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class ProgrammingSkill
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }

    public class Volunteering
    {
        public string Title { get; set; }
    }

    public class Contact
    {
        public string Title { get; set; }
    }

    public class Language
    {
        public string Title { get; set; }
    }


    public class PreviewData
    {
        public PreviewDataDetails Data { get; set; }
    }

    public class PreviewDataDetails
    {
        public int Id { get; set; }
        public PreviewAttributes Attributes { get; set; }
    }

    public class PreviewAttributes
    {
        public string Name { get; set; }
        public string AlternativeText { get; set; }
        public string Caption { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Formats Formats { get; set; }
        public string Hash { get; set; }
        public string Ext { get; set; }
        public string Mime { get; set; }
        public double Size { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public string Provider { get; set; }
        public object ProviderMetadata { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Formats
    {
        public Format Thumbnail { get; set; }
        public Format Small { get; set; }
    }

    public class Format
    {
        public string Name { get; set; }
        public string Hash { get; set; }
        public string Ext { get; set; }
        public string Mime { get; set; }
        public object Path { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Size { get; set; }
        public int SizeInBytes { get; set; }
        public string Url { get; set; }
    }

    public class Meta
    {
        public Pagination Pagination { get; set; }
    }

    public class Pagination
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int Total { get; set; }
    }

}