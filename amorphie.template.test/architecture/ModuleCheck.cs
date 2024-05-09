using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;
using amorphie.template.Module;

namespace amorphie.template.test.architecture;

public class ModuleCheck
{
    public dynamic GetModules()
    {
        var types = Types.InAssembly(typeof(StudentModule).Assembly);
        return types.That().ResideInNamespace("amorphie.template.Module");
    }

    [Fact]
    public void CheckModuleName()
    {
        var modules = GetModules();

        var result = modules.Should().HaveNameEndingWith("Module")
                        .GetResult()
                        .IsSuccessful;

        Assert.True(result);
    }

    [Fact]
    public void IsModuleSealed()
    {
        var modules = GetModules();

        var result = modules.Should().BeSealed()
                        .GetResult()
                        .IsSuccessful;

        Assert.True(result);
    }

    [Fact]
    public void IsInheritedFromCore()
    {
        var modules = GetModules();

        var result = modules.Should().Inherit(typeof(BaseBBTRoute<,,>)).Or().Inherit(typeof(BaseRoute))
                        .GetResult()
                        .IsSuccessful;

        Assert.True(result);
    }
}
