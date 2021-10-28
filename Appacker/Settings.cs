using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacker
{
    public class Settings
    {
        public string language;

        public bool isRepackable;
        public bool openUnpackDirectory;
        public MainForm.UnpackDirectory unpackDirectory;
        public string launchArguments;
        public string customFileDescription;

        public int positionTop;
        public int positionLeft;
        public int width;
        public int height;
    }
}
