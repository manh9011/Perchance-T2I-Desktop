namespace Perchance
{
    public partial class Toast : Form
    {
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
        public int Duration { get; set; } = 3000;

        private int tick = 0;

        public Toast(Form owner)
        {
            InitializeComponent();
            Owner = owner;
        }

        private void FrmToast_Shown(object sender, EventArgs e)
        {
            tmrTimeline.Start();
            Location = new Point(Owner!.Location.X + (Owner.Width - Width) / 2, Owner.Location.Y - Height);
            tmrAnimIn.Start();
        }

        private void tmrTimeline_Tick(object sender, EventArgs e)
        {
            tick += tmrTimeline.Interval;

            if (tick > Duration - 500 && tmrAnimIn.Enabled)
            {
                tmrAnimIn.Stop();
                tmrAnimOut.Start();
            }

            if (tick > Duration)
            {
                tmrTimeline.Stop();
                tmrAnimOut.Stop();
                Close();
            }
        }

        private void tmrAnimIn_Tick(object sender, EventArgs e)
        {
            var delta_x = (Owner!.Location.X + (Owner.Width - Width) / 2.0f - Location.X) / 2.0f;
            var delta_y = (Owner!.Location.Y + 10 - Location.Y) / 2.0f;
            Location = new Point((int)Math.Round(Location.X + delta_x), (int)Math.Round(Location.Y + delta_y));
        }

        private void tmrAnimOut_Tick(object sender, EventArgs e)
        {
            var delta_x = (Owner!.Location.X + (Owner.Width - Width) / 2.0f - Location.X) / 2.0f;
            var delta_y = (Owner!.Location.Y - Height - Location.Y) / 2.0f;
            Location = new Point((int)Math.Round(Location.X + delta_x), (int)Math.Round(Location.Y + delta_y));
        }

        public static void Show(string description, string caption = "Information", int duration = 3000)
        {
            var owner = Application.OpenForms[0]!;
            var frm = new Toast(owner)
            {
                Description = description,
                Caption = caption,
                Duration = duration
            };
            frm.Show(owner);
        }
    }
}
