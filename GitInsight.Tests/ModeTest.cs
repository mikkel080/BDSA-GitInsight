namespace GitInsight.Tests;

public class ModeTest
{
    StringWriter writer;
    string path;

    public ModeTest(){
        writer = new StringWriter();
        Console.SetOut(writer);

        path = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
    }

    [Fact]
    public void CommitFrequency()
    {
        Program.Main(new String[]{path+@"\testrepo.git", "F"});

        var output = writer.GetStringBuilder().ToString().TrimEnd();

        output.Should().Contain("1 2011-04-14");
    }

    [Fact]
    public void CommitAuthor()
    {
        Program.Main(new String[]{path+@"\testrepo.git", "A"});

        var output = writer.GetStringBuilder().ToString().TrimEnd();

        output.Should().Contain("Scott Chacon");
        output.Should().Contain("2 2010-05-25");

    }
}