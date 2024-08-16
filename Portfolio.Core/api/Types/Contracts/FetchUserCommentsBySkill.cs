using System.Runtime.Serialization;

namespace Portfolio.Core.Types.Contracts
{
    // SkillsComment || ProjectsComment
    [DataContract]
    public class FetchUserCommentsBySkillOrProject<T>
    {
        [DataMember(Name = "isExist")]
        public bool IsExist { get; set; }

        [DataMember(Name = "commentsToStore")]
        public List<T> CommentsToStore { get; set; } = new List<T>();
    }
}