using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RavSoft
{
    /// <summary>
    /// Provides textual cues to a text box.
    /// </summary>
    /// <summary>
    /// An object that provides basic logging capabilities.
    /// Copyright (c) 2008 Ravi Bhavnani, ravib@ravib.com
    ///
    /// This software may be freely used in any product or work, provided this
    /// copyright notice is maintained.  To help ensure a single point of release,
    /// please email and bug reports, flames and suggestions to ravib@ravib.com.
    /// </summary>
    public static class CueProvider
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage
          (IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        /// <summary>
        /// Sets a text box's cue text.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="cue">The cue text.</param>
        public static void SetCue
          (TextBox textBox,
           string cue)
        {
            SendMessage (textBox.Handle, EM_SETCUEBANNER, 0, cue);
        }

        /// <summary>
        /// Clears a text box's cue text.
        /// </summary>
        /// <param name="textBox">The text box</param>
        public static void ClearCue
          (TextBox textBox)
        {
            SendMessage (textBox.Handle, EM_SETCUEBANNER, 0, string.Empty);
        }
    }
}
