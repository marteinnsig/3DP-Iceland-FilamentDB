using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace FilamentDbApp;

public partial class SplashWindow : Window
{
    private readonly TaskCompletionSource<bool> _shownTcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public SplashWindow()
    {
        InitializeComponent();
        Loaded += SplashWindow_Loaded;
    }

    public Task WaitUntilShownAsync() => _shownTcs.Task;

    private void SplashWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Opacity = 0;

        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(260));
        fadeIn.Completed += (_, _) => _shownTcs.TrySetResult(true);
        BeginAnimation(OpacityProperty, fadeIn);

        var drawAnimation = new DoubleAnimation
        {
            From = 220,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1400),
            BeginTime = TimeSpan.FromMilliseconds(180),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        ExtrusionTrace.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, drawAnimation);
    }

    public void SetStatus(string status)
    {
        LoadingText.Text = status;
    }

    public void FadeOut(Action completed)
    {
        var animation = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(260));
        animation.Completed += (_, _) => completed();
        BeginAnimation(OpacityProperty, animation);
    }
}
