using System.Windows.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace ChatTailorAI.WinUI.Common.Behaviors
{
    public static class FlyoutCommandBehavior
    {
        public static ICommand GetFlyoutItemCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(FlyoutItemCommandProperty);
        }

        public static void SetFlyoutItemCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(FlyoutItemCommandProperty, value);
        }

        // Workaround for accessing the view model command from a menu item in a flyout
        // that is in a data template
        public static readonly DependencyProperty FlyoutItemCommandProperty =
            DependencyProperty.RegisterAttached(
            "FlyoutItemCommand",
            typeof(ICommand),
            typeof(FlyoutCommandBehavior),
            new PropertyMetadata(null, OnFlyoutItemCommandChanged));

        private static void OnFlyoutItemCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MenuFlyoutItem menuItem)
            {
                menuItem.Click -= OnMenuItemClick;
                if (e.NewValue is ICommand)
                {
                    menuItem.Click += OnMenuItemClick;
                }
            }
        }

        private static void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuFlyoutItem;
            var command = GetFlyoutItemCommand(menuItem);
            if (command != null && command.CanExecute(menuItem.DataContext))
            {
                command.Execute(menuItem.DataContext);
            }
        }
    }
}
