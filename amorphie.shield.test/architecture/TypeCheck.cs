using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;
using amorphie.shield.Module;

namespace amorphie.shield.architecture;

public class TypeCheck
{
    [Fact]
    public void CheckIfAnyStatic()
    {
        var types = Types.InAssembly(typeof(CertificateModule).Assembly);

        var result = types.Should().BeStatic()
                        .GetResult()
                        .IsSuccessful;

        Assert.False(result);
    }

}
