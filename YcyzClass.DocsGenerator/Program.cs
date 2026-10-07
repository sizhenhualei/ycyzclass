// See https://aka.ms/new-console-template for more information

using XmlDocMarkdown.Core;

namespace YcyzClass.DocsGenerator;

static class Program
{
    public static int Main(string[] args)
    {
        Console.WriteLine("YcyzClass Document Generator");
        return XmlDocMarkdownApp.Run(args);
    }
}