using NUnit.Framework;
using System.Diagnostics;
using System.IO;
using System.Threading;

[TestFixture]
public class Testing
{
    private string processName = "notepad"; // Process name for testing
    private int maxLifetimeMinutes = 1; // Maximum lifetime for testing
    private int monitoringFrequencyMinutes = 1; // Monitoring frequency for testing

    [Test]
    public void MonitorAndKillProcess()
    {
        // Arrange
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "notepad.exe" 
        };
        

        // Act
        Program.Main(new string[] { processName, maxLifetimeMinutes.ToString(), monitoringFrequencyMinutes.ToString() });
        Thread.Sleep(2 * 60 * 1000); 

        // Assert
        Assert.That(Process.GetProcessesByName(processName).Length, Is.Zero);
    }

    [Test]
    public void LogProcessKilled()
    {
        // Arrange
        string processName = "notepad"; 
        string logFilePath = "process_log.txt";

        // Act
        Program.Main(new string[] { processName, maxLifetimeMinutes.ToString(), monitoringFrequencyMinutes.ToString() });
        Thread.Sleep(2 * 60 * 1000); 

        // Assert
        string[] logLines = File.ReadAllLines(logFilePath);
        Assert.That(logLines.Length, Is.GreaterThan(0));
        Assert.That(logLines[logLines.Length - 1], Contains.Substring($"Process {processName} exceeded maximum lifetime and was killed"));
    }
}
