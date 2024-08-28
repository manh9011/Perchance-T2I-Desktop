namespace Perchance
{
    public partial class Toast : Form
    {
        public int Duration { get; set; } = 3000;

        public string Caption
        {
            get
            {
                return lblCaption.Text;
            }
            set
            {
                lblCaption.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return lblDescription.Text;
            }
            set
            {
                lblDescription.Text = value;
            }
        }

        private int tick = 0;

        public Toast()
        {
            InitializeComponent();
            DoubleBuffered = true;
            typeof(Label)!.InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic, null,
                lblCaption, new object[] { true });
            typeof(Label)!.InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic, null,
                lblDescription, new object[] { true });
        }

        private void FrmToast_Shown(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            Location = new Point(Owner!.Location.X + (Owner.Width - Width) / 2, Owner.Location.Y - Height);

            tmrTimeline.Start();
            tmrAnimIn.Start();
        }

        // | 500ms in | ... | 500ms out |
        private void tmrTimeline_Tick(object sender, EventArgs e)
        {
            if (tick == 500)
                tmrAnimIn.Stop();

            if (tick == Duration - 500)
                tmrAnimOut.Start();

            if (tick > Duration)
            {
                tmrTimeline.Stop();
                tmrAnimOut.Stop();
                Close();
            }

            tick += tmrTimeline.Interval;
        }

        private double easeIn(double x)
        {
            return x * x * x * x * x;
        }

        private void tmrAnimIn_Tick(object sender, EventArgs e)
        {
            var time = tick / 500f;
            Opacity = easeIn(time);
            var delta_x = (Owner!.Location.X + (Owner.Width - Width) / 2 - Location.X) * easeIn(time);
            var delta_y = (Owner!.Location.Y + 10 - Location.Y) * easeIn(time);
            Location = new Point((int)Math.Round(Location.X + delta_x), (int)Math.Round(Location.Y + delta_y));
        }

        private double easeOut(double x)
        {
            return 1 - Math.Pow(1 - x, 5);
        }

        private void tmrAnimOut_Tick(object sender, EventArgs e)
        {
            var time = (tick - (Duration - 500)) / 500f;
            Opacity = easeOut(time);
            var delta_x = (Owner!.Location.X + (Owner.Width - Width) / 2 - Location.X) * easeOut(time);
            var delta_y = (Owner!.Location.Y - Height - Location.Y) * easeOut(time);
            Location = new Point((int)Math.Round(Location.X + delta_x), (int)Math.Round(Location.Y + delta_y));
        }

        public static void Show(string description, string caption = "Information", int duration = 3000)
        {
            var frm = new Toast()
            {
                Description = description,
                Caption = caption,
                Duration = duration
            };
            frm.Show(Application.OpenForms[0]!);
        }
    }
}
