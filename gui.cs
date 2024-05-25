using System;
using System.Diagnostics;
using System.Windows.Forms;

public class ControlForm : Form
{
    private Button startButton;
    private Button stopButton;
    private KeyPressListener keyPressListener;

    public ControlForm()
    {
        keyPressListener = new KeyPressListener();

        startButton = new Button
        {
            Text = "Start",
            Location = new System.Drawing.Point(10, 10),
            AutoSize = true
        };
        startButton.Click += StartButton_Click;

        stopButton = new Button
        {
            Text = "Stop",
            Location = new System.Drawing.Point(100, 10),
            AutoSize = true,
            Enabled = false
        };
        stopButton.Click += StopButton_Click;

        Controls.Add(startButton);
        Controls.Add(stopButton);
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        if (!keyPressListener.IsRunning)
        {
            keyPressListener.Start();
            startButton.Enabled = false;
            stopButton.Enabled = true;
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
        }
        else
        {
            MessageBox.Show("KeyPressListener is not running.");
        }
    }
}
