using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoCollage.ViewModels
{
    public class MediaElementViewModel
    {
        public MediaElementViewModel(string videoPath)
        {
            VideoPath = new Uri(videoPath, UriKind.Relative);
        }
        public Uri VideoPath { get; set; }
    }
}
