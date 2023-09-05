using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace NightCity.Core.Controls
{
    public static class MouseLeftButtonDownBehavior
    {
        public static ICommand GetCommand(ListView listView) =>
            (ICommand)listView.GetValue(CommandProperty);

        public static void SetCommand(ListView listView, ICommand value) =>
            listView.SetValue(CommandProperty, value);

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(MouseLeftButtonDownBehavior),
            new UIPropertyMetadata(null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ListView listView = (ListView)d;

            ICommand oldCommand = e.OldValue as ICommand;
            if (oldCommand != null)
                listView.RemoveHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown);

            ICommand newCommand = e.NewValue as ICommand;
            if (newCommand != null)
                listView.AddHandler(UIElement.MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnMouseLeftButtonDown, true);
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListView listView = (ListView)sender;
            ICommand command = GetCommand(listView);
            if (command != null)
                command.Execute(listView.SelectedItem);
        }
    }
}
