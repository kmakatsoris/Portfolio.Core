namespace Portfolio.Core.Interfaces.Contract
{
    public interface IBasicProjectsAndSkillsPropertiesContract
    {
        public string Title { get; set; }
        public string BriefDescription { get; set; }
        public string ExtensiveDescription { get; } // IEnumarable<string>
        public Guid ResourceIntroId { get; set; }
        public string Category { get; set; }
        public Guid ResourceMainId { get; set; }
        public string Tags { get; } // IEnumarable<string>
        public string Grades { get; } // JSON<ProjectsGrade> Mine Grade
        public string Comments { get; } // JSON<ProjectsComment[]> Others             
        public DateTime CreatedAt { get; set; }
    }
}