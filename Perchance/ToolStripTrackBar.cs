using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms.Design;

namespace Perchance
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripTrackBar : ToolStripControlHost
    {
        internal static readonly object EventRightToLeftLayoutChanged = new();

        public ToolStripTrackBar() : base(CreateControlInstance())
        {
            if (Control is ToolStripTrackBarControl trackbar)
                trackbar.Owner = this;
        }

        public ToolStripTrackBar(string? name) : this()
        {
            Name = name;
        }

        public ToolStripTrackBarControl TrackBar => (ToolStripTrackBarControl)Control;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image? BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        [DefaultValue(100)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Maximum
        {
            get => TrackBar.Maximum;
            set => TrackBar.Maximum = value;
        }

        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum
        {
            get => TrackBar.Minimum;
            set => TrackBar.Minimum = value;
        }

        [Localizable(true)]
        [DefaultValue(false)]
        public virtual bool RightToLeftLayout
        {
            get => TrackBar.RightToLeftLayout;
            set => TrackBar.RightToLeftLayout = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [AllowNull]
        public override string Text
        {
            get => Control.Text;
            set => Control.Text = value;
        }

        [DefaultValue(0)]
        [Bindable(true)]
        public int Value
        {
            get => TrackBar.Value;
            set => TrackBar.Value = Math.Max(TrackBar.Minimum, Math.Min(TrackBar.Maximum, value));
        }

        private static Control CreateControlInstance()
        {
            return new ToolStripTrackBarControl { Size = new Size(100, 20) };
        }

        private void HandleRightToLeftLayoutChanged(object? sender, EventArgs e)
        {
            OnRightToLeftLayoutChanged(e);
        }

        private protected void RaiseEvent(object key, EventArgs e) => ((EventHandler?)Events[key])?.Invoke(this, e);

        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            RaiseEvent(EventRightToLeftLayoutChanged, e);
        }

        protected override void OnSubscribeControlEvents(Control? control)
        {
            if (control is TrackBar bar)
                bar.RightToLeftLayoutChanged += new EventHandler(HandleRightToLeftLayoutChanged);

            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control? control)
        {
            if (control is TrackBar bar)
                bar.RightToLeftLayoutChanged -= new EventHandler(HandleRightToLeftLayoutChanged);

            base.OnUnsubscribeControlEvents(control);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler? KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler? KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler? KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? OwnerChanged
        {
            add => base.OwnerChanged += value;
            remove => base.OwnerChanged -= value;
        }

        public event EventHandler? RightToLeftLayoutChanged
        {
            add => Events.AddHandler(EventRightToLeftLayoutChanged, value);
            remove => Events.RemoveHandler(EventRightToLeftLayoutChanged, value);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler? Validated
        {
            add => base.Validated += value;
            remove => base.Validated -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event CancelEventHandler? Validating
        {
            add => base.Validating += value;
            remove => base.Validating -= value;
        }
    }
}
