using System;
using System.Windows;
using System.Windows.Interactivity;
using KonigLabs.FantaEmotion.CommonView.Helpers;
using KonigLabs.FantaEmotion.CommonViewModels.Behaviors;
using WindowState = KonigLabs.FantaEmotion.CommonViewModels.Behaviors.WindowState;

namespace KonigLabs.FantaEmotion.CommonView.Behaviors
{
    public class WindowClosingBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += (sender, args) =>
            {
                var closeable = AssociatedObject.DataContext as ICloseable;
                if (closeable != null)
                {
                    closeable.StateChanged +=
                        (obj, state) =>
                            Application.Current.Dispatcher.BeginInvoke(
                                new Action(() => AssociatedObject.SetWindowCloseStatus(state)));
                    AssociatedObject.Closing += (o, eventArgs) => closeable.OnClose();

                    closeable.RequestWindowVisibilityChanged += state =>
                    {
                        Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            switch (state)
                            {
                                case WindowState.Closed:
                                    AssociatedObject.Close();
                                    break;
                                case WindowState.Hidden:
                                    AssociatedObject.Hide();
                                    break;
                                case WindowState.Visible:
                                    AssociatedObject.Show();
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(state));
                            }
                        });
                    };
                }
            };
        }
    }
}
