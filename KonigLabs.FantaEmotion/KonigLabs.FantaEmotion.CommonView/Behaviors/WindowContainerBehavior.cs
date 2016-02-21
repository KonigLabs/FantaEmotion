using System;
using System.Windows;
using System.Windows.Interactivity;
using KonigLabs.FantaEmotion.CommonView.Helpers;
using KonigLabs.FantaEmotion.CommonView.Windows;
using KonigLabs.FantaEmotion.CommonViewModels.Behaviors;

namespace KonigLabs.FantaEmotion.CommonView.Behaviors
{
    public class WindowContainerBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += delegate
            {
                var container = AssociatedObject.DataContext as IWindowContainer;
                if (container != null)
                    container.ShowWindow += (sender, args) =>
                    {
                        var window = new DialogChildWindow
                        {
                            DataContext = args.Context,
                            Owner = AssociatedObject
                        };

                        window.Loaded += (o, eventArgs) => Application.Current.Dispatcher.BeginInvoke(new Action(() => window.SetWindowCloseStatus(false)));

                        if (args.IsDialog)
                        {
                            window.ShowDialog();
                        }
                        else
                        {
                            window.Show();
                        }
                    };
            };
        }
    }
}
