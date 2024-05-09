using amorphie.core.Module.minimal_api;
using NetArchTest.Rules;
using amorphie.template.Module;

namespace amorphie.template.test.architecture;

public class TypeCheck
{
    [Fact]
    public void CheckIfAnyStatic()
    {
        var types = Types.InAssembly(typeof(StudentModule).Assembly);

        var result = types.Should().BeStatic()
                        .GetResult()
                        .IsSuccessful;

        Assert.False(result);
    }

}
