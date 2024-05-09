using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;

namespace amorphie.template.test.architecture;

public class DependencyCheck
{
    [Fact]
    public void CoreDependencyCheck()
    {
        var result = Types.InCurrentDomain()
            .That()
            .ResideInNamespace("amorphie.template.core")
            .ShouldNot()
            .HaveDependencyOn("amorphie.template.data")
            .GetResult()
            .IsSuccessful;
    }
}
