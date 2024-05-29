using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;
using amorphie.shield.Module;
using Xunit.Priority;

namespace amorphie.shield.architecture;

public class ModuleCheck
{
    static bool PreviousFactResult = true; // Initialize with whatever the actual result of the previous test was.
    static ManualResetEventSlim manualEvent = new ManualResetEventSlim(false); // Used to control the execution of the next
    public dynamic GetModules()
    {
        var types = Types.InAssembly(typeof(CertificateModule).Assembly);
        return types.That().ResideInNamespace("amorphie.shield.Module");
    }

    [Fact, Priority(1)]
    public void CheckModuleName()
    {
        // Wait for the manual event to be set before proceeding
        manualEvent.Wait();

        var modules = GetModules();

        var result = modules.Should().HaveNameEndingWith("Module")
                        .GetResult()
                        .IsSuccessful;

        Assert.True(result);
    }

    [Fact, Priority(0)]
    public void IsModuleSealed()
    {
        var modules = GetModules();

        var result = modules.Should().BeSealed()
                        .GetResult()
                        .IsSuccessful;

        Assert.True(result);
        // Set the manual event to allow the next test to proceed
        manualEvent.Set();
    }

    //[Fact]
    //public void IsInheritedFromCore()
    //{
    //    var modules = GetModules();

    //    var result = modules.Should().Inherit(typeof(BaseBBTRoute<,,>)).Or().Inherit(typeof(BaseRoute))
    //                    .GetResult()
    //                    .IsSuccessful;

    //    Assert.True(result);
    //}
}
