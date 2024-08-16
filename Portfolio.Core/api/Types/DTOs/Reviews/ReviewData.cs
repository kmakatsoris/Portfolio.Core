using System.Runtime.Serialization;
using Portfolio.Core.Types.Enums.Reviews;

namespace Portfolio.Core.Types.DTOs.Reviews
{
    [DataContract]
    public class ReviewData
    {
        [DataMember(Name = "characteristics")]
        public string Characteristics { get; set; }

        [DataMember(Name = "highlights")]
        public string Highlights { get; set; }

        [DataMember(Name = "grade")]
        public float Grade { get; set; }

        [DataMember(Name = "title")]
        public ReviewCategoryTitleEnum Title { get; private set; }

        public ReviewData(ReviewCategoryTitleEnum title)
        {
            Title = title;
        }
    }

}