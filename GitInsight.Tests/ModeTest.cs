namespace GitInsight.Tests;

public class ModeTest
{
    string path;
    Program program;

    public ModeTest()
    {
        path = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        program = new Program();

        if(path.Contains(@"\"))
        {
            path = path+@"\testrepo.git";
        }
        else
        {
            path = path+@"/testrepo.git";
        }
    }

    [Fact]
    public void CommitFrequency()
    {
        var output = program.Run(path, "F");

        output.Should().Contain("1 2011-04-14");
    }

    [Fact]
    public void CommitAuthor()
    {
        var output =  program.Run(path, "A");

        output.Should().Contain("Scott Chacon");
        output.Should().Contain("2 2010-05-25");

    }
}