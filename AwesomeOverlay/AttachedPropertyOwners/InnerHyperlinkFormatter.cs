
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AwesomeOverlay.AttachedPropertyOwners
{
    class InnerHyperlinkFormatter
    {

        public static bool GetTextToFormat(DependencyObject d) =>
            (bool)d.GetValue(TextToFormatProperty);

        public static void SetTextToFormat(DependencyObject d, bool value) =>
            d.SetValue(TextToFormatProperty, value);

        public static readonly DependencyProperty TextToFormatProperty =
            DependencyProperty.RegisterAttached(
                "TextToFormat",
                typeof(string),
                typeof(InnerHyperlinkFormatter),
                new PropertyMetadata(
                    "",
                    OnTextToFormatUpdated
                ));

        private static void OnTextToFormatUpdated(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBlock element) || string.IsNullOrEmpty((string)e.NewValue))
                return;

            element.Inlines.Clear();
            string text = (string)e.NewValue;

            Regex regex = new Regex(@"(https?:\/\/)?([\w-]{1,32}\.[\w-]{1,32})[^\s@]*");

            MatchCollection matchCollection = regex.Matches(text);
            ;
            if (matchCollection.Count == 0) {
                element.Inlines.Add(new Run(text));
                return;
            } else {
                element.Inlines.Add(new Run(text.Substring(0, matchCollection[0].Index)));
            }

            for (int i = 0; i < matchCollection.Count; i++) {
                Hyperlink hp = new Hyperlink(new Run(matchCollection[i].Value));
                try {
                    hp.NavigateUri = new Uri(matchCollection[i].Value);
                    hp.RequestNavigate += (o, args) => {
                        Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
                        args.Handled = true;
                    };
                    element.Inlines.Add(hp);
                } catch {
                    element.Inlines.Add(new Run(matchCollection[i].Value));
                }

                if (text.Length > matchCollection[i].Index + matchCollection[i].Length) {
                    if (i == matchCollection.Count - 1)
                        element.Inlines.Add(new Run(text.Substring(matchCollection[i].Index + matchCollection[i].Length, text.Length - (matchCollection[i].Index + matchCollection[i].Length))));
                    else
                        element.Inlines.Add(new Run(text.Substring(matchCollection[i].Index + matchCollection[i].Length, matchCollection[i + 1].Index - (matchCollection[i].Index + matchCollection[i].Length))));
                }
            }
        }

    }
}
