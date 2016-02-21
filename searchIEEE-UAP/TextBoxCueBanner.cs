using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace searchIEEE
{
    public sealed class TextBoxCueBanner : TextBox
    {
        public Boolean CueBannerState;
        public Brush CueBannerActiveBrush;
        public Brush CueBannerInactiveBrush;
        public String CueBannerText
        {
            get { return (String)GetValue(CueBannerTextProperty); }
            set { SetValue(CueBannerTextProperty, value); }
        }
        public static readonly DependencyProperty CueBannerTextProperty = DependencyProperty.Register("CueBannerText", typeof(String), typeof(TextBoxCueBanner), new PropertyMetadata("Enter Search..."));

        public TextBoxCueBanner()
        {
            this.DefaultStyleKey = typeof(TextBox);

            CueBannerState = true;
            CueBannerActiveBrush = this.BorderBrush;
            CueBannerInactiveBrush = this.Foreground;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (CueBannerState)
            {
                this.CueBannerState = false;
                this.Text = String.Empty;
                this.Foreground = CueBannerInactiveBrush;
            }
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (!CueBannerState && string.IsNullOrEmpty(this.Text))
            {
                this.CueBannerState = true;
                this.Text = CueBannerText;
                this.Foreground = CueBannerActiveBrush;
            }
        }
    }
}
