namespace RE_Editor.Generator;

public static class Program {
    public static void Main(string[] args) {
        new GenerateFiles().Go(args);

        if (System.Diagnostics.Debugger.IsAttached && Environment.UserInteractive && !Console.IsInputRedirected) {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}