namespace FilamentDbApp.Models;

public sealed record MaterialDetailGroup(string Name, IReadOnlyList<MaterialDetailField> Fields);
