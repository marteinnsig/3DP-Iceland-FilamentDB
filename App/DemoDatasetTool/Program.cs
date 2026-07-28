using FilamentDbApp.DemoDatasetTool;

try
{
    return (args.Length > 0 ? args[0] : string.Empty) switch
    {
        "validate-transform" => TransformationContractValidator.Run(args),
        "build" => DemoDatasetBuilder.Run(args),
        _ => DemoDatasetInspector.Run(args)
    };
}
catch (Exception exception)
{
    var operation = args.Length > 0 ? args[0] : "inspection";
    Console.Error.WriteLine(
        $"Demo dataset {operation} failed: {SafeError.Code(exception)}");
    return 1;
}
