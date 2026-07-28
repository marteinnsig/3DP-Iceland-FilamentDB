using FilamentDbApp.DemoDatasetTool;

try
{
    return args.Length > 0 &&
        string.Equals(args[0], "validate-transform", StringComparison.Ordinal)
        ? TransformationContractValidator.Run(args)
        : DemoDatasetInspector.Run(args);
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Demo dataset inspection failed: {SafeError.Code(exception)}");
    return 1;
}
