using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;

namespace amorphie.shield.test.architecture;

public class DependencyCheck
{
    [Fact]
    public void CoreDependencyCheck()
    {
        var result = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("amorphie.shield.core")
            .ShouldNot()
            .HaveDependencyOn("amorphie.shield.data")
            .GetResult()
            .IsSuccessful;
    }
}
