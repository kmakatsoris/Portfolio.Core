namespace Portfolio.Core.Interfaces.Contract
{
    // G: SkillsAllGrades || ProjectsAllGrades && C: SkillsComment ||  
    public interface IBasicProjectsAndSkillsDTOPropertiesContract<G, C> : IBasicProjectsAndSkillsPropertiesContract
    {
        public IEnumerable<string> ExtensiveDescriptionDTO { get; set; }
        public IEnumerable<C> CommentsDTO { get; set; }
        public G GradesDTO { get; set; }
    }
}