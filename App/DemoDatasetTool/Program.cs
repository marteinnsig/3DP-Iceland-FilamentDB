using FilamentDbApp.DemoDatasetTool;

try
{
    return DemoDatasetInspector.Run(args);
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Demo dataset inspection failed: {SafeError.Code(exception)}");
    return 1;
}
