using Moq;
using Portfolio.Core.Implementation.Projects;
using Portfolio.Core.Interfaces.Context.Projects;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.DTOs.Projects;
using Portfolio.Core.Types.Models.Projects;
using Xunit;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.UnitTesting.Implementation.Projects
{
    public class ProjectsService__UnitTest 
    {
        private readonly Mock<IProjectsRepository> _projectsRepositoryMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IOAuthAuthorizationService> _oauthServiceMock;
        private readonly ProjectsService _projectsService;
        private readonly List<ProjectModel> projectsModelList_Valid;

        public ProjectsService__UnitTest()
        {
            _projectsRepositoryMock = new Mock<IProjectsRepository>();
            _loggerMock = new Mock<ILogger>();
            _oauthServiceMock = new Mock<IOAuthAuthorizationService>();
            _projectsService = new ProjectsService(
                _loggerMock.Object,
                _projectsRepositoryMock.Object,
                _oauthServiceMock.Object
            );

            projectsModelList_Valid = new List<ProjectModel>
            {
                new ProjectModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Entity 1",
                    BriefDescription = "Brief description for Entity 1",
                    ExtensiveDescription = "Extensive description for Entity 1",
                    ResourceIntroId = Guid.NewGuid(),
                    Category = "Category 1",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now, // Initial value
                    ResourceMainId = Guid.NewGuid(),
                    Tags = "Tag1, Tag2, Tag3"
                },
                new ProjectModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Entity 2",
                    BriefDescription = "Brief description for Entity 2",
                    ExtensiveDescription = "Extensive description for Entity 2",
                    ResourceIntroId = Guid.NewGuid(),
                    Category = "Category 2",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now, // Initial value
                    ResourceMainId = Guid.NewGuid(),
                    Tags = "Tag4, Tag5"
                },
                new ProjectModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Entity 3",
                    BriefDescription = "Brief description for Entity 3",
                    ExtensiveDescription = "Extensive description for Entity 3",
                    ResourceIntroId = Guid.NewGuid(),
                    Category = "Category 3",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now, // Initial value
                    ResourceMainId = Guid.NewGuid(),
                    Tags = "Tag6"
                }
            };
        }

        [Fact]
        [Trait("Category", "projectsService")]
        [Trait("Subcategory", "GetAllprojects")]
        [Trait("Attribute", "[Standard-Usage]: Returns mapped projects from the repository.")]
        public async Task GetAllprojects_ReturnsMappedprojects()
        {
            // Arrange
            _projectsRepositoryMock.Setup(repo => repo.GetAllProjects()).ReturnsAsync(projectsModelList_Valid);

            // Act
            var result = await _projectsService.GetAllProjects();            

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<ProjectsDTO>>(result);
            Assert.Equal(projectsModelList_Valid.Count(), result.Count());
        }
    }
}