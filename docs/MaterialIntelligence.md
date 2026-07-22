# Material Intelligence

## v26.3 - Material Intelligence

Material Intelligence extends the AI Assistant with filter-aware recommendation actions.

## Visible App Features

- Recommended Next Video
- Recommended Comparisons
- Hidden Gems

## How It Works

The feature uses the currently visible material rows in the WPF application.
This means global search and material filters can be used before generating recommendations.

## Recommendation Signals

The local recommendation logic considers:

- Manufacturer
- Product line
- Base material
- Material category
- Reinforcement
- Available score columns where present
- Engineering and practical buyer-interest signals

## Workflow

1. Filter or search the materials list.
2. Open AI Assistant.
3. Click Recommended Next Video, Recommended Comparisons or Hidden Gems.
4. Save the result as a research session if useful.

## v26.4 Scope Support

Material Intelligence can now analyze either current visible rows or a selected Material Collection.
