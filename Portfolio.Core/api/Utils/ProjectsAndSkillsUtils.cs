
using System.Reflection;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Interfaces.Context.Skills;
using Portfolio.Core.Interfaces.Contract;
using Portfolio.Core.Types.Contracts;
using Portfolio.Core.Types.DataTypes.Projects;
using Portfolio.Core.Types.DataTypes.Skills;
using Portfolio.Core.Types.DTOs.Projects;
using Portfolio.Core.Types.DTOs.Skills;
using Portfolio.Core.Types.Enums.Projects;
using Portfolio.Core.Types.Enums.Skills;
using Portfolio.Core.Types.Models.Projects;
using Portfolio.Core.Utils.Mapping;

namespace Portfolio.Core.Utils.ProjectsUtils
{
    public static class ProjectsAndSkillsUtils
    {
        #region Projects & Skills
        public static bool IsProject_OR_SkillValid(this IBasicProjectsAndSkillsPropertiesContract ps)
        {
            // Common Validations,
            if
            (
                ps == null ||
                string.IsNullOrEmpty(ps?.Title) ||
                string.IsNullOrEmpty(ps?.BriefDescription) ||
                string.IsNullOrEmpty(ps?.ExtensiveDescription) ||
                ps?.ResourceIntroId == null || ps?.ResourceIntroId == Guid.Empty ||
                ps?.ResourceMainId == null || ps?.ResourceMainId == Guid.Empty ||
                ps?.CreatedAt == null || ps?.CreatedAt == DateTime.MinValue || ps?.CreatedAt < new DateTime(2024, 5, 1) ||
                string.IsNullOrEmpty(ps?.Category) ||
                string.IsNullOrEmpty(ps?.Grades)
            )
                return false;

            // Projects' Validations,
            if (ps is IProjectsProperties p)
            {
                return p?.UpdatedAt != null && p?.UpdatedAt != DateTime.MinValue && p?.UpdatedAt >= new DateTime(2024, 5, 1);
            }

            // Skills' Validations,
            if (ps is ISkillsProperties s)
            {
                return true;
            }

            return false;
        }

        public static bool IsProject_OR_SkillDTOValid<G, C>(this IBasicProjectsAndSkillsDTOPropertiesContract<G, C> psDTO)
        {
            // Common Validations,
            if
            (
                psDTO == null ||
                string.IsNullOrEmpty(psDTO?.Title) ||
                string.IsNullOrEmpty(psDTO?.BriefDescription) ||
                psDTO?.ExtensiveDescriptionDTO == null || psDTO?.ExtensiveDescriptionDTO.Count() < 1 ||
                psDTO?.ResourceIntroId == null || psDTO?.ResourceIntroId == Guid.Empty ||
                psDTO?.ResourceMainId == null || psDTO?.ResourceMainId == Guid.Empty ||
                psDTO?.CreatedAt == null || psDTO?.CreatedAt == DateTime.MinValue || psDTO?.CreatedAt < new DateTime(2024, 5, 1) ||
                string.IsNullOrEmpty(psDTO?.Category)
            )
                return false;

            // Projects' Validations,
            if (psDTO != null && psDTO is IProjectsProperties p)
            {
                ProjectsAllGrades g = psDTO.GradesDTO as ProjectsAllGrades;
                return g?.ProjectsGradeValidation() == true;
            }

            // Skills' Validations,
            if (psDTO != null && psDTO is ISkillsProperties s)
            {
                return true;
            }

            return false;
        }

        public static bool IsProject_OR_SkillGradeValid(this BasicGradeAndMetaDataType psGrade)
        {
            if
            (
                psGrade == null ||
                string.IsNullOrEmpty(psGrade?.Email) ||
                string.IsNullOrEmpty(psGrade?.Details)
            )
                return false;

            if (psGrade is ProjectsOrSkillsGrade<ProjectsGradeTitleEnum> p)
            {
                return Enum.IsDefined(typeof(ProjectsGradeTitleEnum), p?.Title);
            }

            if (psGrade is ProjectsOrSkillsGrade<SkillsGradeTitleEnum> s)
            {
                return Enum.IsDefined(typeof(SkillsGradeTitleEnum), s?.Title);
            }

            return false;
        }

        #region Private Projects & Skills Service Methods,
        // Extract All The Emails From ProjectsDTO Object
        // @TAG: ProjectsGradeTitle && SkillsGradeTitle 
        // ProjectsDTO & SkillsDTO
        public static IEnumerable<string> ExtractAllEmailsFromProject_OR_Skills<C, G, R>(BasicSkillsAndProjectsDTOType<G, C> ps) where C : class, IBasicCommentTypeContract<G, R>
        {
            List<string> emails = new List<string>();
            if (ps == null) return emails;

            LoadingGradesEmails(emails, ps.GradesDTO);

            foreach (C c in ps.CommentsDTO)
            {
                LoadingGradesEmails(emails, c.Grade);
                if (!string.IsNullOrEmpty(c?.Review?.Email))
                    emails.Add(c?.Review?.Email);
            }
            return emails;
        }

        private static void LoadingGradesEmails<T>(List<string> emails, T grades)
        {
            foreach (string e in ProjectsAndSkillsUtils.ExtractEmailsFromProjectsGrades(grades))
            {
                if (!string.IsNullOrEmpty(e))
                    emails.Add(e);
            }
        }

        // ProjectsAllGrades && SkillsAllGrades
        private static bool ValidatingGradesEmails<T>(string email, T grades)
        {
            if (grades == null) return false;
            foreach (string e in ProjectsAndSkillsUtils.ExtractEmailsFromProjectsGrades(grades))
            {
                bool isValid = !string.IsNullOrEmpty(e) && e?.ToLower()?.Equals(email?.ToLower()) == true;
                if (isValid != true) return false;
            }
            return true;
        }

        // Fetch User's Comments From Specific Project
        // @TAG: ProjectsGradeTitle && SkillsGradeTitle  
        public static async Task<FetchUserCommentsBySkillOrProject<C>> FetchUserCommentsByProject_OR_Skills<T, G, C>(
            T projectsOrSkills,
            string requestEmail,
            IEnumerable<C> requestsComments
        ) where C : class
        {
            var result = new FetchUserCommentsBySkillOrProject<C>();

            if (typeof(C) == typeof(ProjectsComment) && projectsOrSkills is ProjectsDTO p)
            {
                ProcessComments<ProjectsComment, ProjectsAllGrades, ProjectsReviewsMetaData>(
                    p.CommentsDTO.Cast<ProjectsComment>(),
                    requestEmail,
                    requestsComments.Cast<ProjectsComment>(),
                    result as FetchUserCommentsBySkillOrProject<ProjectsComment>
                );
            }
            else if (typeof(C) == typeof(SkillsComment) && projectsOrSkills is SkillsDTO s)
            {
                ProcessComments<SkillsComment, SkillsAllGrades, SkillsReviewsMetaData>(
                    s.CommentsDTO.Cast<SkillsComment>(),
                    requestEmail,
                    requestsComments.Cast<SkillsComment>(),
                    result as FetchUserCommentsBySkillOrProject<SkillsComment>
                );
            }

            return await Task.FromResult(result);
        }

        private static void ProcessComments<C, G, R>(
            IEnumerable<C> comments,
            string requestEmail,
            IEnumerable<C> requestsComments,
            FetchUserCommentsBySkillOrProject<C> result)
            where C : class, IBasicCommentTypeContract<G, R>
        {
            if (comments == null || !comments.Any() || result == null)
                return;

            foreach (var comment in comments)
            {
                if (comment == null || comment?.Review == null) continue;
                if (comment?.Review?.Email?.ToLower()?.Equals(requestEmail?.ToLower()) != true)
                {
                    result.CommentsToStore.Add(comment);
                    continue;
                }

                if (comment?.Review?.Email?.ToLower()?.Equals(requestEmail?.ToLower()) == true && !ValidatingGradesEmails(comment?.Review?.Email, comment.Grade))
                {
                    throw new Exception("The email of the grade and review must be matched.");
                }

                if (result.IsExist)
                {
                    throw new Exception("There are more than one comments for the email address.");
                }

                result.IsExist = true;
                result.CommentsToStore.Add(requestsComments?.FirstOrDefault());
            }
        }
        #endregion

        #endregion

        #region Projects
        public static bool ProjectsGradeValidation(this ProjectsAllGrades ProjectsGrades)
        {
            List<string> emails = new List<string>();

            if (ProjectsGrades == null) return false;

            PropertyInfo[] properties = typeof(ProjectsAllGrades).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>))
                {
                    var ProjectsGrade = property.GetValue(ProjectsGrades) as ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>;

                    bool isValid = ProjectsGrade != null ? ProjectsGrade.IsProject_OR_SkillGradeValid() : false;
                    if (isValid != true) return false;
                }
            }

            return true;
        }

        public static List<string> ExtractEmailsFromProjectsGrades<T>(this T ProjectsGrades)
        {
            List<string> emails = new List<string>();

            if (ProjectsGrades == null) return emails;

            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>))
                {
                    var ProjectsGrade = property.GetValue(ProjectsGrades) as ProjectsOrSkillsGrade<ProjectsGradeTitleEnum>;

                    if (ProjectsGrade != null && !string.IsNullOrEmpty(ProjectsGrade.Email))
                    {
                        emails.Add(ProjectsGrade.Email);
                    }
                }
            }

            return emails;
        }
        #endregion

        #region Skills
        public static bool SkillsGradeValidation(this SkillsAllGrades skillsGrades)
        {
            List<string> emails = new List<string>();

            if (skillsGrades == null) return false;

            PropertyInfo[] properties = typeof(SkillsAllGrades).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(ProjectsOrSkillsGrade<SkillsGradeTitleEnum>))
                {
                    var skillsGrade = property.GetValue(skillsGrades) as ProjectsOrSkillsGrade<SkillsGradeTitleEnum>;

                    bool isValid = skillsGrade != null ? skillsGrade.IsProject_OR_SkillGradeValid() : false;
                    if (isValid != true) return false;
                }
            }

            return true;
        }

        public static List<string> ExtractEmailsFromSkillsGrades(this SkillsAllGrades skillsGrades)
        {
            List<string> emails = new List<string>();

            if (skillsGrades == null) return emails;

            PropertyInfo[] properties = typeof(SkillsAllGrades).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(ProjectsOrSkillsGrade<SkillsGradeTitleEnum>))
                {
                    var skillsGrade = property.GetValue(skillsGrades) as ProjectsOrSkillsGrade<SkillsGradeTitleEnum>;

                    if (skillsGrade != null && !string.IsNullOrEmpty(skillsGrade.Email))
                    {
                        emails.Add(skillsGrade.Email);
                    }
                }
            }

            return emails;
        }
        #endregion        

    }
}