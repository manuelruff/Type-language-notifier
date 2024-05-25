using System;
using System.Diagnostics;
using System.Windows.Forms;

public class ControlForm : Form
{
    private Button startButton;
    private Button stopButton;
    private Process notifierProcess;

    public ControlForm()
    {
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
            AutoSize = true
        };
        stopButton.Click += StopButton_Click;

        Controls.Add(startButton);
        Controls.Add(stopButton);
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        if (notifierProcess == null || notifierProcess.HasExited)
        {
            notifierProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "TypeLanguageNotifier.exe",
                    WorkingDirectory = @"C:\path\to\your\publish\directory", // Update this path
                    UseShellExecute = false
                }
            };
            notifierProcess.Start();
        }
        else
        {
            MessageBox.Show("TypeLanguageNotifier is already running.");
        }
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        if (notifierProcess != null && !notifierProcess.HasExited)
        {
            notifierProcess.Kill();
            notifierProcess = null;
        }
        else
        {
            MessageBox.Show("TypeLanguageNotifier is not running.");
        }
    }
}
