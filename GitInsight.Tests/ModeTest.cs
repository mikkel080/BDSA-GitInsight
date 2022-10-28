namespace GitInsight.Tests;

public class ModeTest
{
    StringWriter writer;
    string path;

    public ModeTest(){
        writer = new StringWriter();
        Console.SetOut(writer);

        path = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
        
        if(path.Contains(@"\")){
            path = path+@"\testrepo.git";
        }else{
            path = path+@"/testrepo.git";
        }
    }

    [Fact]
    public void CommitFrequency()
    {
        Program.Main(new String[]{path, "F"});

        var output = writer.GetStringBuilder().ToString().TrimEnd();

        output.Should().Contain("1 2011-04-14");
    }

    [Fact]
    public void CommitAuthor()
    {
        Program.Main(new String[]{path, "A"});

        var output = writer.GetStringBuilder().ToString().TrimEnd();

        output.Should().Contain("Scott Chacon");
        output.Should().Contain("2 2010-05-25");

    }
}