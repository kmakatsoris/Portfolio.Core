using System.Text.RegularExpressions;
using Portfolio.Core.Types.DTOs.Resources;

namespace Portfolio.Core.Utils.DefaultUtils
{
    public static class DefaultUtils
    {
        public static string GetStringValue(this Enum enumValue)
        {
            var type = enumValue.GetType();
            var memberInfo = type.GetMember(enumValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Value;
                }
            }
            return "";
        }

        public static string ToPerformTagsRequest(this string tags)
        {
            if (tags == null) return "";
            string[] tmp_tags = tags.Split(',');
            string pattern = @"^#[A-Za-z0-9._]+$";
            string result = "";
            foreach (string tag in tmp_tags)
            {
                if (Regex.IsMatch(tag, pattern))
                {
                    if (!result.Equals(""))
                        result += ",";
                    result += tag;
                }
            }
            return result;
            // return tmp_tags.Where(tag => tag != null && ).ToArray();
        }

        // @TODO: Choice to trhow exception
        public static T GetValue<T>(this IEnumerable<T> arg, int idx)
        {
            if (arg == null) return default(T);
            if (arg?.Count() > idx) return arg.ElementAt(idx);
            return default(T);
        }

        // @TODO: Check -> string[] array4 = { "one", "two", "two", "three" }; string[] array5 = { "one", "two", "three", "two" };
        public static bool ArraysContainSameElements<T>(IEnumerable<T> array1, IEnumerable<T> array2)
        {
            if (array1 == null || array2 == null)
            {
                return false;
            }

            if (array1.Count() != array2.Count())
            {
                return false;
            }

            var dict1 = array1.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
            var dict2 = array2.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

            return dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }

        public static bool ArraysContainSameElements(IEnumerable<ExtensiveDescriptionType> array1, IEnumerable<ExtensiveDescriptionType> array2)
        {
            if (array1 == null || array2 == null)
            {
                return false;
            }

            var list1 = array1.ToList();
            var list2 = array2.ToList();

            if (list1.Count != list2.Count)
            {
                return false;
            }

            var dict1 = list1.GroupBy(x => new { x.Text, x.Color, x.Location })
                             .ToDictionary(g => g.Key, g => g.Count());

            var dict2 = list2.GroupBy(x => new { x.Text, x.Color, x.Location })
                             .ToDictionary(g => g.Key, g => g.Count());

            return dict1.Count == dict2.Count && !dict1.Except(dict2).Any();
        }

        // before MyEqual dont add ?
        public static bool MyEqual(this string s1, string s2)
        {
            s1 = s1?.ToLower() ?? "";
            s2 = s2?.ToLower() ?? "";
            return s1?.Equals(s2) == true;
        }

        public static string GenerateTagsClauseCondition(this List<string> tags)
        {
            if (tags == null || tags?.Count() <= 0 || !tags.Any()) return "";
            string conds = tags.FirstOrDefault();
            for (int i = 1; i < tags?.Count(); i++)
                conds += " OR " + tags[i];
            return ValidateTagsConds(conds) ? conds : "";
        }

        public static bool ValidateTagsConds(this string conds)
        {
            string pattern = @"^(\w+\sOR\s)+\w+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(conds);
        }
    }
}