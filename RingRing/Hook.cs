//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Reflection;
//using System.Runtime.InteropServices;
//using System.Windows.Forms;

//namespace RingRing
//{
//    public class Hook
//    {
//        public Hook()
//        {
//            _hookID = SetHook(KeyboardHookProc);
//        }

//        ~Hook()
//        {
//            UnhookWindowsHookEx(_hookID);
//        }
       
//        const int SW_HIDE = 0;

//        [DllImport("user32.dll")]
//        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

//        [DllImport("kernel32.dll")]
//        static extern IntPtr GetConsoleWindow();

//        private const int WH_KEYBOARD_LL = 13;
//        private const int WM_KEYDOWN = 0x0100;
//        private const int WM_KEYUP = 0x0101;
//        private const byte VK_SHIFT = 0x10;
//        private const byte VK_CAPITAL = 0x14;
//        private const byte VK_NUMLOCK = 0x90;
//        private const int WM_SYSKEYDOWN = 0x104;
//        private const int WM_SYSKEYUP = 0x105;

//        /// <summary>
//        /// Occurs when the user presses a key
//        /// </summary>
//        //public event KeyEventHandler KeyDown;
//        /// <summary>
//        /// Occurs when the user presses and releases 
//        /// </summary>
//        //public event KeyPressEventHandler KeyPress;
//        /// <summary>
//        /// Occurs when the user releases a key
//        /// </summary>
//        public event KeyEventHandler KeyUp;

//        [DllImport("user32")]
//        private static extern int ToAscii(
//            int uVirtKey,
//            int uScanCode,
//            byte[] lpbKeyState,
//            byte[] lpwTransKey,
//            int fuState);

//        [DllImport("user32")]
//        private static extern int GetKeyboardState(byte[] pbKeyState);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
//        private static extern short GetKeyState(int vKey);

//        //private static HookProc _proc;
//        private static IntPtr _hookID = IntPtr.Zero;
//        private static IntPtr SetHook(HookProc proc)
//        {
//            using (Process curProcess = Process.GetCurrentProcess())
//            using (ProcessModule curModule = curProcess.MainModule)
//            {
//                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
//                    GetModuleHandle(curModule.ModuleName), 0);
//            }
//        }


//        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

//        int value = 0;
//        private IntPtr KeyboardHookProc(int nCode, IntPtr wParam, IntPtr lParam)
//        {
//            bool handled = false;
//            int vkCode = Marshal.ReadInt32(lParam);
//            Keys keyData = (Keys)vkCode;
//            KeyEventArgs e = new KeyEventArgs(keyData);
//            bool isDownShift = false;
//            bool isDownCapslock = false;
//            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
//            {
//                //Console.WriteLine("WM_KEYDOWN : " + keyData);
//                if (keyData == Keys.LShiftKey || keyData == Keys.RShiftKey || keyData == Keys.CapsLock)
//                {
//                    value = 0;
//                    //Console.WriteLine("Shift : " + value);
//                }
//                else
//                {
//                    value++;
//                    //Console.WriteLine("value : " + value);
//                }
//                //Console.WriteLine("value : " + value);
//                //KeyDown(this, e);
//                //handled = handled || e.Handled;
//            }
//            // raise KeyPress
//            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
//            {
//                //Console.WriteLine("KeyPress : " + keyData);
//                //{
//                //    //Console.WriteLine("value : " + value++);
//                //    //value++;
//                isDownShift = ((GetKeyState(VK_SHIFT) & 0x80) == 0x80 ? true : false);
//                isDownCapslock = (GetKeyState(VK_CAPITAL) != 0 ? true : false);

//                //    //byte[] keyState = new byte[256];
//                //    //GetKeyboardState(keyState);
//                //    //byte[] inBuffer = new byte[2];
//                //if (ToAscii(vkCode, 0, keyState, inBuffer, 0) == 1)
//                //{
//                //    char key = (char)inBuffer[0];
//                //    if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key))
//                //        key = Char.ToUpper(key);
//                //    e = new KeyEventArgs((Keys)key);
//                //    KeyPressEventArgs keypress = new KeyPressEventArgs(key);
//                //    KeyPress(this, keypress);
//                //    handled = handled || e.Handled;
//                //}

//            }
//            // raise KeyUp

//            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
//            {
//                //Console.WriteLine("isDownShift : " + isDownShift + " isDownCapslock : " + isDownCapslock);
//                //Console.WriteLine("WM_KEYUP outer : " + e.KeyCode);
//                //Console.WriteLine("WM_KEYUP value : " + value);
//                if (value == 1)
//                {
//                    byte[] keyState = new byte[256];
//                    GetKeyboardState(keyState);
//                    byte[] inBuffer = new byte[2];
//                    if (ToAscii(vkCode, 0, keyState, inBuffer, 0) == 1)
//                    {
//                        char key = (char)inBuffer[0];
//                        //Console.WriteLine("key : " + (int)key);
//                        if ((isDownCapslock ^ isDownShift) && Char.IsLetter(key))
//                        {
//                            key = Char.ToUpper(key);
//                            //Console.WriteLine("key if condtion : " + (int)key);
//                        }
//                        e = new KeyEventArgs((Keys)key);
//                        //Console.WriteLine("WM_KEYUP inner: " + e.KeyValue);
//                        KeyUp(this, e);
//                        handled = handled || e.Handled;
//                    }
//                }
//                if (keyData != Keys.LShiftKey && keyData != Keys.RShiftKey && keyData != Keys.CapsLock)
//                {
//                    value = 0;
//                }
//                //int vkCode = Marshal.ReadInt32(lParam);
//                //Keys keyData = (Keys)vkCode;

//                //Console.WriteLine(e.KeyCode);
//                //KeyUp(this, e);
//                //handled = handled || e.Handled;
//            }
//            if (handled)
//                return (IntPtr)1;
//            else
//                return CallNextHookEx(_hookID, nCode, wParam, lParam);
//        }


//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr SetWindowsHookEx(int idHook,
//            HookProc lpfn, IntPtr hMod, uint dwThreadId);
//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
//            IntPtr wParam, IntPtr lParam);
//        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr GetModuleHandle(string lpModuleName);
//    }
//}
