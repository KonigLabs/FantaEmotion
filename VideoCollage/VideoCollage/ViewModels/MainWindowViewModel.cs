using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application = System.Windows.Application;

namespace VideoCollage.ViewModels
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            MediaElementViewModels = new ObservableCollection<MediaElementViewModel>();

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Created += new FileSystemEventHandler(Watcher_Changed);
            watcher.Path = Directory.GetCurrentDirectory() + @"\Videos";
            // Begin watching.
            watcher.EnableRaisingEvents = true;
            FirstInit();


        }

        private string _videoPath = "Videos";

        private void FirstInit()
        {
            var files = Directory.GetFiles(_videoPath).ToList();
            foreach (var file in files)
            {
                MediaElementViewModels.Add(new MediaElementViewModel(file));
                if (MediaElementViewModels.Count == 12)
                    return;
            }
        }

        public ObservableCollection<MediaElementViewModel> MediaElementViewModels { get; set; }


        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            new Thread(() => AddVideo(e)).Start();
        }

        private void AddVideo(FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                FileStream stream = null;
                while (stream == null)
                {
                    try
                    {
                        stream = File.Open(e.FullPath, FileMode.Open);
                    }
                    catch (IOException)
                    {


                    }
                    finally
                    {
                        if (stream != null)
                        {
                            stream.Close();
                        }
                    }
                }


                if (MediaElementViewModels.Count == 12)
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {//GC сожрет камим образо или нет?
                        MediaElementViewModels.RemoveAt(11);
                    }));
                var newElement = new MediaElementViewModel(e.FullPath);


                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    MediaElementViewModels.Insert(0, newElement);
                }));
            }
        }
    }
}
