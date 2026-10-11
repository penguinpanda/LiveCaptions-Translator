using System.Windows;
using System.Windows.Threading;

namespace LiveCaptionsTranslator
{
    /// <summary>
    /// Floating notification anchored to the bottom right corner of the screen. Used to tell the
    /// user that the application keeps running after the main window was hidden.
    /// </summary>
    public partial class ToastWindow : Window
    {
        private const double ScreenMargin = 16;
        private static ToastWindow? instance;

        private readonly DispatcherTimer closeTimer = new()
        {
            Interval = TimeSpan.FromSeconds(6)
        };

        public ToastWindow()
        {
            InitializeComponent();
            closeTimer.Tick += (s, e) => Hide();
            MouseLeftButtonDown += (s, e) => Hide();
        }

        public static void ShowToast(string title, string message)
        {
            instance ??= new ToastWindow();
            instance.ToastTitle.Text = title;
            instance.ToastMessage.Text = message;

            instance.closeTimer.Stop();
            instance.Show();
            instance.Reposition();
            instance.closeTimer.Start();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (IsVisible)
                Reposition();
        }

        private void Reposition()
        {
            Rect workArea = SystemParameters.WorkArea;
            Left = workArea.Right - ActualWidth - ScreenMargin;
            Top = workArea.Bottom - ActualHeight - ScreenMargin;
        }
    }
}
