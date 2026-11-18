// Sageata Dreapta - apasa automat tasta "Sageata dreapta" la un interval ales.
// Compilat cu csc.exe din .NET Framework 4 (vine cu Windows) -> C# 5, fara sintaxa noua.
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

[assembly: System.Reflection.AssemblyTitle("Sageata Dreapta")]
[assembly: System.Reflection.AssemblyProduct("Sageata Dreapta")]
[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]

namespace SageataDreapta
{
    static class Native
    {
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);

        public const byte VK_RIGHT = 0x27;
        public const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        public const uint KEYEVENTF_KEYUP = 0x2;
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_SYSTEM_REQUIRED = 0x1;
        public const uint ES_DISPLAY_REQUIRED = 0x2;
        public const int WM_HOTKEY = 0x0312;

        public static void PressRightArrow()
        {
            byte scan = (byte)MapVirtualKey(VK_RIGHT, 0);
            keybd_event(VK_RIGHT, scan, KEYEVENTF_EXTENDEDKEY, UIntPtr.Zero);
            System.Threading.Thread.Sleep(30);
            keybd_event(VK_RIGHT, scan, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
    }

    class MainForm : Form
    {
        const int HOTKEY_ID = 0xB00B;

        NumericUpDown numInterval;
        ComboBox cmbUnit;
        NumericUpDown numDelay;
        NumericUpDown numMax;
        CheckBox chkMinimize;
        CheckBox chkTopMost;
        Button btnStart;
        Label lblStatus;
        Label lblNext;
        Label lblCount;

        Timer timer;
        Stopwatch clock = new Stopwatch();
        bool running;
        bool inDelay;
        double nextAtMs;
        int count;
        bool hotkeyOk;

        static readonly Color Accent = Color.FromArgb(22, 163, 74);
        static readonly Color StopRed = Color.FromArgb(220, 38, 38);

        string SettingsPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SageataDreapta.ini"); }
        }

        public MainForm()
        {
            Text = "Sageata Dreapta  →";
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10f);
            ClientSize = new Size(380, 420);
            BackColor = Color.White;
            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

            var title = new Label();
            title.Text = "→  Apasă automat Săgeata Dreapta";
            title.Font = new Font("Segoe UI Semibold", 13f);
            title.AutoSize = true;
            title.Location = new Point(18, 14);
            Controls.Add(title);

            int y = 62;
            AddLabel("La fiecare:", 18, y + 3);
            numInterval = new NumericUpDown();
            numInterval.DecimalPlaces = 1;
            numInterval.Minimum = 0.2m;
            numInterval.Maximum = 100000m;
            numInterval.Increment = 1m;
            numInterval.Value = 5m;
            numInterval.Location = new Point(150, y);
            numInterval.Width = 90;
            Controls.Add(numInterval);
            cmbUnit = new ComboBox();
            cmbUnit.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUnit.Items.AddRange(new object[] { "secunde", "minute" });
            cmbUnit.SelectedIndex = 0;
            cmbUnit.Location = new Point(248, y);
            cmbUnit.Width = 112;
            Controls.Add(cmbUnit);

            y += 38;
            AddLabel("Rapid:", 18, y + 4);
            int bx = 150;
            foreach (int s in new int[] { 3, 5, 10, 30, 60 })
            {
                var b = new Button();
                b.Text = s < 60 ? s + "s" : "1m";
                b.Tag = s;
                b.Size = new Size(40, 28);
                b.Location = new Point(bx, y);
                b.FlatStyle = FlatStyle.System;
                b.Click += delegate(object o, EventArgs e)
                {
                    int sec = (int)((Button)o).Tag;
                    if (sec >= 60) { cmbUnit.SelectedIndex = 1; numInterval.Value = sec / 60; }
                    else { cmbUnit.SelectedIndex = 0; numInterval.Value = sec; }
                };
                Controls.Add(b);
                bx += 43;
            }

            y += 44;
            AddLabel("Pornește după (sec):", 18, y + 3);
            numDelay = new NumericUpDown();
            numDelay.Minimum = 0; numDelay.Maximum = 60; numDelay.Value = 3;
            numDelay.Location = new Point(210, y); numDelay.Width = 70;
            Controls.Add(numDelay);

            y += 36;
            AddLabel("Oprește după (0 = niciodată):", 18, y + 3);
            numMax = new NumericUpDown();
            numMax.Minimum = 0; numMax.Maximum = 1000000; numMax.Value = 0;
            numMax.Location = new Point(210, y); numMax.Width = 70;
            Controls.Add(numMax);

            y += 38;
            chkMinimize = new CheckBox();
            chkMinimize.Text = "Ascunde fereastra la Start (focus pe poze)";
            chkMinimize.Checked = true;
            chkMinimize.AutoSize = true;
            chkMinimize.Location = new Point(18, y);
            Controls.Add(chkMinimize);

            y += 26;
            chkTopMost = new CheckBox();
            chkTopMost.Text = "Mereu deasupra";
            chkTopMost.AutoSize = true;
            chkTopMost.Location = new Point(18, y);
            chkTopMost.CheckedChanged += delegate { TopMost = chkTopMost.Checked; };
            Controls.Add(chkTopMost);

            y += 36;
            btnStart = new Button();
            btnStart.Text = "START";
            btnStart.Font = new Font("Segoe UI Semibold", 14f);
            btnStart.Size = new Size(344, 52);
            btnStart.Location = new Point(18, y);
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.BackColor = Accent;
            btnStart.ForeColor = Color.White;
            btnStart.Cursor = Cursors.Hand;
            btnStart.Click += delegate { Toggle(); };
            Controls.Add(btnStart);

            y += 60;
            lblStatus = new Label();
            lblStatus.AutoSize = false;
            lblStatus.Size = new Size(344, 22);
            lblStatus.Location = new Point(18, y);
            lblStatus.Font = new Font("Segoe UI Semibold", 10f);
            Controls.Add(lblStatus);

            y += 22;
            lblNext = new Label();
            lblNext.AutoSize = false;
            lblNext.Size = new Size(200, 22);
            lblNext.Location = new Point(18, y);
            lblNext.ForeColor = Color.DimGray;
            Controls.Add(lblNext);
            lblCount = new Label();
            lblCount.AutoSize = false;
            lblCount.TextAlign = ContentAlignment.TopRight;
            lblCount.Size = new Size(144, 22);
            lblCount.Location = new Point(218, y);
            lblCount.ForeColor = Color.DimGray;
            Controls.Add(lblCount);

            timer = new Timer();
            timer.Interval = 50;
            timer.Tick += OnTick;

            LoadSettings();
            UpdateUi();
        }

        void AddLabel(string text, int x, int y)
        {
            var l = new Label();
            l.Text = text;
            l.AutoSize = true;
            l.Location = new Point(x, y);
            Controls.Add(l);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            hotkeyOk = Native.RegisterHotKey(Handle, HOTKEY_ID, 0, (uint)Keys.F9);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Native.WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                // Pornit din tastatura: fereastra cu pozele are deja focus, deci fara asteptare.
                Toggle(true);
                return;
            }
            base.WndProc(ref m);
        }

        double IntervalMs
        {
            get
            {
                double v = (double)numInterval.Value;
                return cmbUnit.SelectedIndex == 1 ? v * 60000.0 : v * 1000.0;
            }
        }

        void Toggle() { Toggle(false); }

        void Toggle(bool fromHotkey)
        {
            if (running) StopRun("Oprit."); else StartRun(fromHotkey);
        }

        void StartRun(bool fromHotkey)
        {
            SaveSettings();
            running = true;
            count = 0;
            clock.Restart();
            double delayMs = fromHotkey ? 0 : (double)numDelay.Value * 1000.0;
            if (delayMs > 0) { inDelay = true; nextAtMs = delayMs; }
            else { inDelay = false; nextAtMs = IntervalMs; }
            Native.SetThreadExecutionState(Native.ES_CONTINUOUS | Native.ES_SYSTEM_REQUIRED | Native.ES_DISPLAY_REQUIRED);
            SetInputsEnabled(false);
            timer.Start();
            UpdateUi();
            if (!fromHotkey && chkMinimize.Checked) WindowState = FormWindowState.Minimized;
        }

        void StopRun(string why)
        {
            running = false;
            inDelay = false;
            timer.Stop();
            clock.Stop();
            Native.SetThreadExecutionState(Native.ES_CONTINUOUS);
            SetInputsEnabled(true);
            UpdateUi();
            lblStatus.Text = why;
        }

        void OnTick(object sender, EventArgs e)
        {
            double now = clock.Elapsed.TotalMilliseconds;
            if (now >= nextAtMs)
            {
                if (inDelay)
                {
                    inDelay = false;
                }
                Native.PressRightArrow();
                count++;
                if (numMax.Value > 0 && count >= numMax.Value)
                {
                    StopRun("Gata: " + count + " apăsări.");
                    if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
                    return;
                }
                // Programam fata de momentul planificat, ca sa nu se adune intarzieri.
                nextAtMs += IntervalMs;
                if (nextAtMs < now) nextAtMs = now + IntervalMs;
            }
            UpdateUi();
        }

        void SetInputsEnabled(bool on)
        {
            foreach (Control c in Controls)
            {
                if (c == btnStart || c is Label || c == chkTopMost) continue;
                c.Enabled = on;
            }
        }

        void UpdateUi()
        {
            string hk = hotkeyOk ? "  (sau F9)" : "";
            if (running)
            {
                btnStart.Text = "STOP" + hk;
                btnStart.BackColor = StopRed;
                double left = Math.Max(0, nextAtMs - clock.Elapsed.TotalMilliseconds) / 1000.0;
                lblStatus.ForeColor = StopRed;
                lblStatus.Text = inDelay ? "Pornește... dă click pe fereastra cu pozele!" : "Rulează — apasă →";
                lblNext.Text = (inDelay ? "Prima apăsare în " : "Următoarea în ") + FormatSec(left);
                lblCount.Text = "Apăsări: " + count;
                Text = "▶ " + count + " · " + FormatSec(left) + " — Sageata Dreapta";
            }
            else
            {
                btnStart.Text = "START" + hk;
                btnStart.BackColor = Accent;
                lblStatus.ForeColor = Color.DimGray;
                lblStatus.Text = hotkeyOk
                    ? "Sfat: dă click pe poze, apoi apasă F9 (pornește/oprește)."
                    : "Apasă START, apoi dă click pe fereastra cu pozele.";
                lblNext.Text = "";
                lblCount.Text = count > 0 ? "Apăsări: " + count : "";
                Text = "Sageata Dreapta  →";
            }
        }

        static string FormatSec(double s)
        {
            if (s >= 60)
            {
                int total = (int)Math.Ceiling(s);
                return (total / 60) + ":" + (total % 60).ToString("00") + " min";
            }
            return s.ToString(s < 10 ? "0.0" : "0", CultureInfo.InvariantCulture) + " s";
        }

        void LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return;
                foreach (string line in File.ReadAllLines(SettingsPath))
                {
                    int i = line.IndexOf('=');
                    if (i < 0) continue;
                    string k = line.Substring(0, i).Trim();
                    string v = line.Substring(i + 1).Trim();
                    decimal d;
                    bool okNum = decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out d);
                    if (k == "interval" && okNum) numInterval.Value = Clamp(d, numInterval);
                    else if (k == "unit" && v == "minute") cmbUnit.SelectedIndex = 1;
                    else if (k == "delay" && okNum) numDelay.Value = Clamp(d, numDelay);
                    else if (k == "max" && okNum) numMax.Value = Clamp(d, numMax);
                    else if (k == "minimize") chkMinimize.Checked = v == "1";
                    else if (k == "topmost") chkTopMost.Checked = v == "1";
                }
            }
            catch { }
        }

        static decimal Clamp(decimal d, NumericUpDown n)
        {
            return Math.Min(n.Maximum, Math.Max(n.Minimum, d));
        }

        void SaveSettings()
        {
            try
            {
                File.WriteAllLines(SettingsPath, new string[] {
                    "interval=" + numInterval.Value.ToString(CultureInfo.InvariantCulture),
                    "unit=" + (cmbUnit.SelectedIndex == 1 ? "minute" : "secunde"),
                    "delay=" + numDelay.Value.ToString(CultureInfo.InvariantCulture),
                    "max=" + numMax.Value.ToString(CultureInfo.InvariantCulture),
                    "minimize=" + (chkMinimize.Checked ? "1" : "0"),
                    "topmost=" + (chkTopMost.Checked ? "1" : "0")
                });
            }
            catch { } // ex. ruleaza de pe un stick protejat la scriere - nu e grav
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (running) StopRun("");
            SaveSettings();
            if (hotkeyOk) Native.UnregisterHotKey(Handle, HOTKEY_ID);
            base.OnFormClosing(e);
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
