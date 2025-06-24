using System;
using System.Drawing;
using System.Windows.Forms;

namespace MC.LaundryShop.App.Helper.UserControls
{
    public partial class CustomTextbox : UserControl
    {
        private string _placeholderText = "Enter value";
        private char _actualPasswordChar = '\0';
        public new event KeyEventHandler KeyUp;
        public new event KeyPressEventHandler KeyPress;

        public string LabelText
        {
            get => label.Text;
            set => label.Text = value;
        }

        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                if (string.IsNullOrEmpty(textbox.Text) || (textbox.Tag is bool isPlaceholder && isPlaceholder))
                {
                    SetPlaceholder();
                }
            }
        }

        public char PasswordChar
        {
            get => _actualPasswordChar;
            set => _actualPasswordChar = value;
        }

        public override string Text
        {
            get
            {
                if (textbox.Tag is bool isPlaceholder && isPlaceholder)
                {
                    return string.Empty;
                }
                return textbox.Text;
            }
            set
            {
                textbox.Text = value;
                textbox.ForeColor = SystemColors.ControlText;
                textbox.Tag = false;
                textbox.PasswordChar = _actualPasswordChar;
                if (string.IsNullOrWhiteSpace(value))
                {
                    SetPlaceholder();
                }
            }
        }


        public CustomTextbox()
        {
            InitializeComponent();
            InitializeCustomTextbox();
        }

        private void InitializeCustomTextbox()
        {
            label.Text = LabelText;
            textbox.Enter += TextBox_Enter;
            textbox.Leave += TextBox_Leave;
            SetPlaceholder();
        }

        private void SetPlaceholder()
        {
            if (!string.IsNullOrWhiteSpace(textbox.Text) &&
                (!(textbox.Tag is bool currentIsPlaceholder) || !currentIsPlaceholder)) return;
            textbox.Text = _placeholderText;
            textbox.ForeColor = SystemColors.ControlDarkDark;
            textbox.Tag = true;
            textbox.PasswordChar = '\0';
        }


        private void RemovePlaceholder()
        {
            if (!(textbox.Tag is bool isPlaceholder) || !isPlaceholder) return;
            textbox.Text = "";
            textbox.ForeColor = SystemColors.ControlText;
            textbox.Tag = false;
            textbox.PasswordChar = _actualPasswordChar;
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            RemovePlaceholder();
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textbox.Text))
            {
                SetPlaceholder();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (textbox != null)
            {
                textbox.Size = ClientSize;
            }
        }

        private void textbox_KeyUp(object sender, KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        private void textbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }
    }
}