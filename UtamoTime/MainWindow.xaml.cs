using System;
using System.Collections.Generic;
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
using System.Runtime.InteropServices;
using System.Timers;
using System.Diagnostics;

namespace UtamoTime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        HookProc hp;

        delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc llkp, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        readonly Stopwatch sw = new Stopwatch();

        double x = 0;

        private static Timer myTimer = new Timer();

        IntPtr MyHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            var keyCode = Marshal.ReadInt32(lParam);

            if (keyCode == 0x79 && wParam == new IntPtr(0x100))
            {
                resetTimer();
            }

            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        void resetTimer()
        {
            x = 0;
            sw.Reset();
            sw.Start();
            myTimer.Stop();
            myTimer = new Timer(100);
            myTimer.Elapsed += updateLabel;
            myTimer.Start();
        }

        void updateLabel(object sender, ElapsedEventArgs e)
        {
            TimeLabel.Dispatcher.Invoke(() => { TimeLabel.Content = ((float)(sw.Elapsed.TotalMilliseconds) / 1000.0f).ToString("F1"); });//e.SignalTime.Second.ToString(); });
            if (x < Math.PI / 2)
            {
                x += 0.0104;
            }
            this.Dispatcher.Invoke(() =>
            {
                this.Background = new SolidColorBrush(Color.FromArgb(255, (byte)(127.0 + 127.0 * Math.Sin(x)), (byte)((255) - (byte)(127.0 + 127.0 * Math.Sin(x))), 0));
            });
        }

        public MainWindow()
        {
            InitializeComponent();

            sw.Start();

            hp = new HookProc(MyHookProc);

            myTimer.Interval = 100;
            myTimer.Elapsed += updateLabel;

            SetWindowsHookEx(13, hp, GetModuleHandle(""), 0);
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.ToolWindow)
            {
                this.WindowStyle = WindowStyle.None;
            }
            else
            {
                this.WindowStyle = WindowStyle.ToolWindow;
            }
        }
    }
}
