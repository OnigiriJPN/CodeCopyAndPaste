using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YourProjectName[ここ変えて］
{
    public partial class Form1 : Form
    {
        // Win32 API
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect,
                                                       int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private int titleBarHeight = 30;

        // Win32 API: マウスでウィンドウをドラッグする
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public Form1()
        {
            InitializeComponent();
            this.Padding = new Padding(1);
            this.BackColor = Color.Black; // 枠色
            this.DoubleBuffered = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 丸角ウィンドウにする例
            IntPtr hRgn = CreateRoundRectRgn(0, 0, this.Width, this.Height, 15, 15);
            SetWindowRgn(this.Handle, hRgn, true);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTCLIENT = 1;
            const int HTCAPTION = 2;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);
                Point cursor = this.PointToClient(Cursor.Position);

                ReleaseCapture(); // マウスキャプチャ解除
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0); // ネイティブドラッグ開始

                return;
            }

            base.WndProc(ref m);
        }

            protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        // タイトルバー描画
        Rectangle titleBarRect = new Rectangle(0, 0, this.Width, titleBarHeight);
        e.Graphics.FillRectangle(Brushes.DarkSlateBlue, titleBarRect);

        // タイトル文字
        TextRenderer.DrawText(e.Graphics, "Custom TitleBar", this.Font,
                              new Point(10, 5), Color.White);

        // 閉じるボタン例
        Rectangle closeBtn = new Rectangle(this.Width - 30, 0, 30, titleBarHeight);
        e.Graphics.FillRectangle(Brushes.Red, closeBtn);
        TextRenderer.DrawText(e.Graphics, "X", this.Font, closeBtn, Color.White,
                              TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        // 自前閉じるボタン
        Rectangle closeBtn = new Rectangle(this.Width - 30, 0, 30, titleBarHeight);
        if (closeBtn.Contains(e.Location))
            this.Close();
    }
    }
}
