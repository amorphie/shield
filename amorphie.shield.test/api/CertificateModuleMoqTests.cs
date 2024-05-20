using amorphie.core.Base;
using amorphie.shield.Certificates;
using amorphie.shield.Module;
using Microsoft.AspNetCore.Http;
using Moq;

namespace amorphie.shield.api;

public class CertificateModuleMoqTests
{
    [Theory]
    [InlineData("123")]
    public async Task GetByDeviceId_Returns_Certificate(string deviceId)
    {
        // Arrange
        var mock = new Mock<ICertificateAppService>();

        mock.Setup(m => m.GetByDeviceIdAsync(deviceId))
        .ReturnsAsync(
            Response<CertificateQueryOutputDto>.Success("")
        );

        // Act
        var result = await CertificateModule.GetByDeviceIdAsync(deviceId, mock.Object);

        //Assert
        Assert.IsAssignableFrom<IResult>(result);
    }
    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public async Task GetByDeviceId_Returns_BusinessException_IfNoExist(string deviceId)
    {
        // Arrange
        var mock = new Mock<ICertificateAppService>();

        mock.Setup(m => m.GetByDeviceIdAsync(deviceId))
        .Throws(new BusinessException(code: 404, severity: "warning", message: "Not exist", details: "User certificate not found"))
        ;

        // Act
        BusinessException ex = await Assert.ThrowsAsync<BusinessException>(async () => await CertificateModule.GetByDeviceIdAsync(deviceId, mock.Object));
        // Assert
        Assert.NotNull(ex);
    }
}

