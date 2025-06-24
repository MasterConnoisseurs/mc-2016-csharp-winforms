using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MC.LaundryShop.App.Forms
{
    public partial class SplashScreen : Form
    {
        private Timer _loadingTimer;
        private List<string> _loadingMessages;
        private int _currentMessageIndex;

        public SplashScreen()
        {
            InitializeComponent();
            InitializeLoadingComponents();
        }

        private void InitializeLoadingComponents()
        {
            _loadingMessages = new List<string>
            {
                "Initializing application modules...",
                "Loading user configurations...",
                "Connecting to database servers...",
                "Fetching latest updates...",
                "Preparing UI components...",
                "Verifying data integrity...",
                "Optimizing resource allocation...",
                "Synchronizing system files...",
                "Caching essential assets...",
                "Setting up network connections...",
                "Compiling runtime scripts...",
                "Decompressing game data...",
                "Activating security protocols...",
                "Loading custom themes...",
                "Establishing secure channels...",
                "Validating license information...",
                "Performing initial setup checks...",
                "Analyzing system performance...",
                "Reconciling data structures...",
                "Finalizing startup routines..."
            };

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            label1.Text = _loadingMessages.Count > 0 ? _loadingMessages[_currentMessageIndex] : @"Starting application...";

            _loadingTimer = new Timer()
            {
                Interval = 100
            };
            _loadingTimer.Tick += LoadingTimer_Tick;
            _loadingTimer.Start();
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < progressBar1.Maximum)
            {
                progressBar1.Value += 1;
                var expectedMessageIndex = (int)Math.Floor((double)progressBar1.Value / progressBar1.Maximum * _loadingMessages.Count);

                if (expectedMessageIndex >= _loadingMessages.Count)
                {
                    expectedMessageIndex = _loadingMessages.Count - 1;
                }

                if (expectedMessageIndex == _currentMessageIndex) return;
                _currentMessageIndex = expectedMessageIndex;
                if (_loadingMessages.Count > 0)
                {
                    label1.Text = _loadingMessages[_currentMessageIndex];
                }
            }
            else
            {
                _loadingTimer.Stop();
                label1.Text = @"Loading complete!";
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void SplashScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_loadingTimer == null || !_loadingTimer.Enabled) return;
            _loadingTimer.Stop();
            _loadingTimer.Dispose();
        }
    }
}
