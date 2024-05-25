using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

public class ControlForm : Form
{
    private Button startButton;
    private Button stopButton;
    private TextBox delayTextBox;
    private Label delayLabel;
    private KeyPressListener keyPressListener;

    public ControlForm()
    {
        // Set form properties
        this.Text = "Type Language Notifier";
        this.Size = new Size(200, 200);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.Sizable;  // Allow resizing
        this.MaximizeBox = true;

        keyPressListener = new KeyPressListener();

        // Create and configure TableLayoutPanel
        var layoutPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 3,
            AutoSize = true
        };
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33F));
        layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34F));

        delayLabel = new Label
        {
            Text = "Delay (seconds):",
            Anchor = AnchorStyles.Right,
            AutoSize = true,
            Margin = new Padding(10)
        };

        delayTextBox = new TextBox
        {
            Anchor = AnchorStyles.Left,
            Size = new Size(75, 20),
            Text = "2"
        };

        startButton = new Button
        {
            Text = "Start",
            Dock = DockStyle.Fill,
            Font = new Font("Arial", 10, FontStyle.Bold),
            Margin = new Padding(10)
        };
        startButton.Click += StartButton_Click;

        stopButton = new Button
        {
            Text = "Stop",
            Dock = DockStyle.Fill,
            Font = new Font("Arial", 10, FontStyle.Bold),
            Margin = new Padding(10),
            Enabled = false
        };
        stopButton.Click += StopButton_Click;

        layoutPanel.Controls.Add(delayLabel, 0, 0);
        layoutPanel.Controls.Add(delayTextBox, 1, 0);
        layoutPanel.Controls.Add(startButton, 0, 1);
        layoutPanel.Controls.Add(stopButton, 1, 1);

        this.Controls.Add(layoutPanel);
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        if (!keyPressListener.IsRunning)
        {
            if (int.TryParse(delayTextBox.Text, out int delay))
            {
                keyPressListener.SetDelay(TimeSpan.FromSeconds(delay).TotalMilliseconds);
            }
            else
            {
                MessageBox.Show("Invalid delay value. Please enter a valid number.");
                return;
            }

            keyPressListener.Start();
            startButton.Enabled = false;
            stopButton.Enabled = true;
            delayTextBox.Enabled = false;
        }
        else
        {
            MessageBox.Show("KeyPressListener is already running.");
        }
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        if (keyPressListener.IsRunning)
        {
            keyPressListener.Stop();
            startButton.Enabled = true;
            stopButton.Enabled = false;
            delayTextBox.Enabled = true;
        }
        else
        {
            MessageBox.Show("KeyPressListener is not running.");
        }
    }
}
