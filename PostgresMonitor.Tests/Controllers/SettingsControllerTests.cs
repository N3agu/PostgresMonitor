using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using PostgresMonitor.Web.Controllers;
using PostgresMonitor.Web.Models;
using PostgresMonitor.Web.Services;

namespace PostgresMonitor.Tests.Controllers
{
    public class SettingsControllerTests
    {
        [Fact]
        public async Task Edit_Post_NewDatabase_WithoutPassword_ReturnsModelError()
        {
            var mockService = new Mock<SettingsService>();
            mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((DbSettings)null);

            var controller = new SettingsController(mockService.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };

            var newSettings = new DbSettings
            {
                Id = Guid.NewGuid(),
                ConnectionName = "Test DB",
                Password = ""
            };

            var result = await controller.Edit(newSettings) as ViewResult;

            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid, "Model state should be invalid");
            Assert.True(controller.ModelState.ContainsKey("Password"), "Should contain a validation error for Password");
        }

        [Fact]
        public async Task Edit_Post_ExistingDatabase_WithoutPassword_RedirectsToIndex()
        {
            var dbId = Guid.NewGuid();
            var mockService = new Mock<SettingsService>();

            mockService.Setup(s => s.GetByIdAsync(dbId))
                       .ReturnsAsync(new DbSettings { Id = dbId, Password = "existing_password" });

            mockService.Setup(s => s.SaveAsync(It.IsAny<DbSettings>())).Returns(Task.CompletedTask);

            var controller = new SettingsController(mockService.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object
            };

            var existingSettings = new DbSettings
            {
                Id = dbId,
                ConnectionName = "Test DB",
                Password = ""
            };

            var result = await controller.Edit(existingSettings) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.True(controller.ModelState.IsValid, "Model state should be valid because the database exists");
        }
    }
}