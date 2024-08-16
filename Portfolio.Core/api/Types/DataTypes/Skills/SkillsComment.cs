using System.Runtime.Serialization;
using Portfolio.Core.Types.Contracts;

namespace Portfolio.Core.Types.DataTypes.Skills
{
    [DataContract]
    public class SkillsComment : BasicCommentType<SkillsAllGrades, SkillsReviewsMetaData>
    {
        [DataMember(Name = "grade")]
        public override SkillsAllGrades Grade { get; set; }
    }
}