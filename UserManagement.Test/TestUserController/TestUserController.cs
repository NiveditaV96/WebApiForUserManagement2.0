using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserManagement.Model;
using UserManagement.Repository;
using System.Collections.Generic;
using UserManagement.Controllers;
using System.Web.Http.Results;
using Moq;

namespace UserManagement.Test.TestUserController
{
    [TestClass]
    public class TestUserController
    {
        const string testRole1 = "Employee";

        [TestMethod]
        public void GetUsersByRole_WhenValidRoleIsGiven_ReturnsListOfRoles()
        {
            // Arrange
            var mockFindUserRepository = new Mock<IFindUsersRepository>();
            var mockUpdateUserRepository = new Mock<IUpdateUsersRepository>();
            var mockGenericRepository = new Mock<IRepository<User>>();
 
            var result = mockFindUserRepository.Setup(x => x.GetUsersByRole(It.IsAny<string>()))
                                               .Returns(GetRoles());

            var controller = new UserController(mockGenericRepository.Object, 
                                                mockUpdateUserRepository.Object, 
                                                mockFindUserRepository.Object);

            //Act
            var actionResult = controller.GetUsersByRole("Employee");
            var contentResult = actionResult as OkNegotiatedContentResult<IEnumerable<string>>;

            //Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(OkNegotiatedContentResult<IEnumerable<string>>));
         
           
        }

        [TestMethod]
        public void GetUsersByRole_WhenInvalidRoleIsGiven_ReturnsEmptyList()
        {
            // Arrange
            var mockUserRepository = new Mock<IFindUsersRepository>();
            var mockUpdateUserRepository = new Mock<IUpdateUsersRepository>();
            var mockGeneRepository = new Mock<IRepository<User>>();

            var result = mockUserRepository.Setup(x => x.GetUsersByRole(It.IsAny<string>()))
                                           .Returns(new List<string>() { });

           
            var controller = new UserController(mockGeneRepository.Object, mockUpdateUserRepository.Object, mockUserRepository.Object);

            //Act
            var actionResult = controller.GetUsersByRole("TL");
            var contentResult = actionResult as BadRequestErrorMessageResult;

            //Assert
            Assert.IsNotNull(actionResult);
            Assert.IsNotNull(contentResult);
            Assert.IsInstanceOfType(actionResult, typeof(BadRequestErrorMessageResult));
            //Assert.AreEqual({ },);
        }



        public static IEnumerable<string> GetRoles()
        {
            return new List<string>() { "Admin", "Cab", "Doctor" };
        }

        public static IEnumerable<User> GetTestUsers()
        {
            var testUsers = new List<User>()
            {
                new User { Username = "Demo1", Password= "pass1", RoleName="Employee"},
                new User { Username = "Demo2", Password= "pass2", RoleName="Manager"},
                new User { Username = "Demo3", Password= "pass3", RoleName="Employee"},
                new User { Username = "Demo4", Password= "pass4", RoleName="HR"},
                new User { Username = "Demo5", Password= "pass5", RoleName="Manager"}
            };

            return testUsers;
        }
    }
}
