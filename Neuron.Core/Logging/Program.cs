using System;
using System.Text;

namespace Neuron.Core.Logging.Neuron;

public class Program
{
    public static void Main()
    {
        var controller = new LoggerController();
        var logger = controller.GetLogger();
        Console.OutputEncoding = Encoding.UTF8;
        logger.Info($"Hello {controller} []", "Placeholder");
        logger.Info("Hello [Test]!", null);
        logger.Info("Hello [Test]!", 1);
        logger.Info("Hello [Test]!", "World");
        logger.Info("Hello [Test]!", LogBoxes.Successful);
        logger.Info("Hello [Test]");
        logger.Verbose("Example Message");
        logger.Info("Example Message");
        logger.Warn("Example Message");
        logger.Error("Example Message");
        logger.Fatal("Example Message");
        ConsoleWrapper.WidthOverride = 150;
        
        try
        {
            void test()
            {
                void test1()
                {
                    throw new Exception("Test Exception"); 
                }
                test1();
            }
            test();
        }
        catch (Exception e)
        {
            logger.Error(e);
        }

        logger.Info("[Diagnostic]", DiagnosticsError.FromParts(
            DiagnosticsError.Summary("Example Framework error has occured."),
            DiagnosticsError.Description("Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua.\n\nAt vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet."),
            DiagnosticsError.Hint("Maybe restart the server."),
            DiagnosticsError.Hint("Maybe update dependencies."),
            DiagnosticsError.Property("test", "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.")   ));
    }
}