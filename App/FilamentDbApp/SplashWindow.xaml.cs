using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace FilamentDbApp;

public partial class SplashWindow : Window
{
    private readonly TaskCompletionSource<bool> _shownTcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TaskCompletionSource<bool> _extrusionTcs =
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

    }

    public Task StartExtrusionAnimationAsync()
    {
        var drawAnimation = new DoubleAnimation
        {
            From = 52,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(1900),
            BeginTime = TimeSpan.FromMilliseconds(80)
        };
        drawAnimation.Completed += (_, _) => _extrusionTcs.TrySetResult(true);
        ExtrusionTrace.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, drawAnimation);
        return _extrusionTcs.Task;
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
