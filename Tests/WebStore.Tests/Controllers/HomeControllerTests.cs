using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using WebStore.Controllers;
using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers;

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public void Contacts_returns_with_View()
    {
        // AAA = A-A-A Arrange - Act - Assert
        // Подготовка данных - их сравнение и проверяем результат

        #region Arrange
        var controller = new HomeController(null!);
        #endregion

        #region Act
        var result = controller.Contacts();
        #endregion

        #region Assert
        var view_result = Assert.IsType<ViewResult>(result);

        // Assert.Equal(nameof(HomeController.Contacts), view_result.ViewName); // неправильное утверждениие
        Assert.Null(view_result.ViewName);
        #endregion
    }

}
