using System.Runtime.Serialization;
using Portfolio.Core.Types.Contracts;

namespace Portfolio.Core.Types.Contracts
{
    // ProjectsGradeTitleEnum || SkillsGradeTitleEnum
    [DataContract]
    public class ProjectsOrSkillsGrade<T> : BasicGradeAndMetaDataType
    {
        [DataMember(Name = "title")]
        public T Title { get; private set; }

        public ProjectsOrSkillsGrade(T title)
        {
            Title = title;
        }
    }
}