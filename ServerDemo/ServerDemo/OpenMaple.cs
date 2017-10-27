using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


    class OpenMaple
    {
        public static string myResult = "";
        public static string myStatus = "";

        MapleEngine.MapleCallbacks cb;
        IntPtr kv;
        String[] argv = new String[2];
        byte[] err = new byte[2048];


        #region Xử Lý CallBack

        /// <summary>
        /// Hàm cbText : Lấy kết quả tính toán
        /// </summary>
        /// <param name="data"></param>
        /// <param name="tag"></param>
        /// <param name="output"></param>
        public static void cbText(IntPtr data, int tag, IntPtr output)
        {
            myResult = Marshal.PtrToStringAnsi(output);

        }

        /// <summary>
        /// Hàm cbError : Hiển thị thông báo lỗi (nếu có)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="msg"></param>
        public static void cbError(IntPtr data, IntPtr offset, IntPtr msg)
        {
            //string s = Marshal.PtrToStringAnsi(msg);
            //MessageBox.Show(Marshal.PtrToStringAnsi(msg));
            Console.WriteLine(Marshal.PtrToStringAnsi(msg));
        }

        /// <summary>
        /// Hàm cbStatus : Lấy các thông số trạng thái của Maple
        /// </summary>
        /// <param name="data"></param>
        /// <param name="used"></param>
        /// <param name="alloc"></param>
        /// <param name="time"></param>
        public static void cbStatus(IntPtr data, IntPtr used, IntPtr alloc, double time)
        {
            myStatus = "cputime=" + time + "; memory used=" + used.ToInt64() + "kB ;" +
                       "alloc=" + alloc.ToInt64() + "kB";
        }

        #endregion

        public void Open()
        {
            argv[0] = "maple";
            argv[1] = "-A2";

            cb.textCallBack = cbText;
            cb.errorCallBack = cbError;
            cb.statusCallBack = cbStatus;
            cb.readlineCallBack = null;
            cb.redirectCallBack = null;
            cb.streamCallBack = null;
            cb.queryInterrupt = null;
            cb.callbackCallBack = null;

            #region Thử khởi động Maple và bẫy lỗi (nếu có)
            try
            {
                kv = MapleEngine.StartMaple(2, argv, ref cb, IntPtr.Zero, IntPtr.Zero, err);
            }
            catch (DllNotFoundException e)
            {
                //MessageBox.Show(e.ToString());
                Console.WriteLine(e.ToString());
                return;
            }
            catch (EntryPointNotFoundException e)
            {
                //MessageBox.Show(e.ToString());
                Console.WriteLine(e.ToString());
                return;
            }

            // make sure we got a good kernel vector handle back
            if (kv.ToInt64() == 0)
            {
                // Maple didn't start properly.  The "err" parameter will be filled
                // in with the reason why (usually a license error)
                // Note that since we passed in a byte[] array we need to trim
                // the characters past \0 during conversion to string
                //MessageBox.Show("Fatal Error, could not start Maple: "
                //        + System.Text.Encoding.ASCII.GetString(err, 0, Array.IndexOf(err, (byte)0)));
                Console.WriteLine("Fatal Error, could not start Maple: "
                        + System.Text.Encoding.ASCII.GetString(err, 0, Array.IndexOf(err, (byte)0)));
                return;
            }
            #endregion

        }
        public void Run(string query)
        {
            try
            {
                // set the default plot driver to something nicer than "char"
                MapleEngine.EvalMapleStatement(kv, Encoding.ASCII.GetBytes("plotsetup(maplet):"));
            }
            catch
            {
                //MessageBox.Show("Maple chưa được Open");
                Console.WriteLine("Maple chưa được Open");
                return;
            }

            try
            {
                // Kiểm tra user có sử dụng help hay không ?
                if (query.Substring(0, 1) == "?")
                    query = "help(" + query.Substring(1, 0) + ");";
            }
            catch
            {
                //MessageBox.Show("Lỗi do user input không hợp lệ");
                Console.WriteLine("Lỗi do user input không hợp lệ");
                return;
            }

            // Định giá trị đã được input, tính toán tức thời và gởi giá trị output
            // đến callback dưới dạng text (cbText). Điều này cũng đồng nghĩa với việc trả về 
            // một kết quả. Kết quả được hiển thị bởi dạng string thông qua biến myResult
            IntPtr val = MapleEngine.EvalMapleStatement(kv, Encoding.ASCII.GetBytes(query + ";"));

            // check if user typed quit/done/stop
            if (MapleEngine.IsMapleStop(kv, val).ToInt32() != 0)
                MapleEngine.StopMaple(kv);
        }
    }

