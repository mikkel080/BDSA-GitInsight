namespace GitInsight;

public class runConsole{
    public static void Main(string[] args){
        var program = new Program();
        var text = program.Run(args[0], args[1]);
        Console.WriteLine(text);
    }
}
