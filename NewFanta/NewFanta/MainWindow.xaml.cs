using EDSDKLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NewFanta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:IDisposable
    {

        public static string SavePathName = "savepath.txt";
        public MainWindow()
        {
            InitializeComponent();
            TakeVideoControl.VideoSave += TakeVideoSave;
            VideoResultControl.Repeat += VideoRepeat;
            VideoResultControl.Continue += Continue;
            TakeVideoControl.Visibility = Visibility.Visible;
            VideoResultControl.Visibility = Visibility.Collapsed;
        }

        public static DirectoryInfo GetLocalVideoDirectory()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return !Directory.Exists(System.IO.Path.Combine(baseDir, "Videos"))
                ? Directory.CreateDirectory(System.IO.Path.Combine(baseDir, "Videos"))
                : new DirectoryInfo(System.IO.Path.Combine(baseDir, "Videos"));
        }
        public static DirectoryInfo GetRemoteVideoDirectory()
        {
            if (File.Exists(SavePathName))
            {
                using (var stream = File.OpenText(SavePathName))
                {
                    var path = stream.ReadLine();
                    if (Directory.Exists(path))
                    {
                        return new DirectoryInfo(path);
                    }
                }
            }
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            return !Directory.Exists(System.IO.Path.Combine(baseDir, "Videos"))
                ? Directory.CreateDirectory(System.IO.Path.Combine(baseDir, "Videos"))
                : new DirectoryInfo(System.IO.Path.Combine(baseDir, "Videos"));
        }
        /// <summary>
        /// Пользователь нажал продолжить
        /// </summary>
        private void Continue(object sender, EventArgs e)
        {
            var info = GetLocalVideoDirectory();
            var remoteInfo = GetRemoteVideoDirectory();
            //Получаем спискок видео
            var lastVideo = info.EnumerateFiles("MVI*.mov").OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();
            if (lastVideo != null)
            {
                //Запускаем конвертацию в фоне
                Process process = new Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.FileName = "ffmpeg.exe";
                process.StartInfo.Arguments = $"-i \"{info.FullName}\\{lastVideo.Name}\" -vf scale=400x226 \"{remoteInfo.FullName}\\Min{lastVideo.Name}\"";
                process.Start();
                process.WaitForExit();
                //Грохаем орегинал
                lastVideo.Delete();
            }
            //заново запускам LV и показываем LV
            TakeVideoControl.StartLiveView();
            TakeVideoControl.Visibility = Visibility.Visible;
            VideoResultControl.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// Пользователь нажал еще раз
        /// </summary>
        private void VideoRepeat(object sender, EventArgs e)
        {
            //Грохаем последний файл
            var info = GetLocalVideoDirectory();
            var lastVideo = info.EnumerateFiles("MVI*.mov").OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();
            if (lastVideo != null)
                lastVideo.Delete();
            //Запускаем LV и показываем LV
            TakeVideoControl.StartLiveView();
            TakeVideoControl.Visibility = Visibility.Visible;
            VideoResultControl.Visibility = Visibility.Collapsed;
            //Начинаем отсчет
            TakeVideoControl.InitRecord();
        }
        /// <summary>
        /// Видео сохрнилось
        /// </summary>
        private void TakeVideoSave(object sender, EventArgs e)
        {
            var info = GetLocalVideoDirectory();
            var lastVideo = info.EnumerateFiles("MVI*.mov").OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();
            if (lastVideo != null)
            {
                VideoResultControl.PreviewVideo.Source = new Uri(lastVideo.FullName);
            }
            TakeVideoControl.Visibility = Visibility.Collapsed;
            VideoResultControl.Visibility = Visibility.Visible;
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            Dispose();
        }
        
        public void Dispose()
        {
            TakeVideoControl.VideoSave -= TakeVideoSave;
            VideoResultControl.Repeat -= VideoRepeat;
            VideoResultControl.Continue -= Continue;
            TakeVideoControl.Dispose();
        }
    }
}
