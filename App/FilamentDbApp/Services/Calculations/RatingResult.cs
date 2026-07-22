namespace FilamentDbApp.Services.Calculations;

public sealed record RatingResult(
    int Stars,
    string Label,
    string Interpretation)
{
    public string Summary => Stars <= 0
        ? "—"
        : $"{new string('★', Stars)}{new string('☆', Math.Max(0, 5 - Stars))} {Label}";
}
