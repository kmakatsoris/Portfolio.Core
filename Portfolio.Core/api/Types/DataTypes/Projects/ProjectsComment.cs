using System.Runtime.Serialization;
using Portfolio.Core.Interfaces.Contract;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.DTOs.Reviews;

namespace Portfolio.Core.Types.DataTypes.Projects
{
    [DataContract]
    public class ProjectsComment : BasicCommentType<ProjectsAllGrades, ProjectsReviewsMetaData>, IBasicCommentTypeContract<ProjectsAllGrades, ProjectsReviewsMetaData>
    {
        [DataMember(Name = "grade")]
        public override ProjectsAllGrades Grade { get; set; }
    }
}