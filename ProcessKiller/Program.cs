using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: monitor.exe <process_name> <max_lifetime_minutes> <monitoring_frequency_minutes>");
            return;
        }

        string processName = args[0];
        int maxLifetimeMinutes = int.Parse(args[1]);
        int monitoringFrequencyMinutes = int.Parse(args[2]);

        
        Timer timer = new Timer(_ => TerminateProgram(), null, 5 * 60 * 1000, Timeout.Infinite);

        Console.CancelKeyPress += (sender, e) =>
        {
            TerminateProgram();
        };

        string logFilePath = "process_log.txt";

        try
        {
            using (StreamWriter logFile = File.AppendText(logFilePath))
            {
                while (true)
                {
                    Process[] processes = Process.GetProcessesByName(processName);
                    foreach (Process process in processes)
                    {
                        TimeSpan lifetime = DateTime.Now - process.StartTime;
                        if (lifetime.TotalMinutes > maxLifetimeMinutes)
                        {
                            process.Kill();
                            Log(logFile, $"Process {processName} (PID: {process.Id}) exceeded maximum lifetime and was killed");
                        }
                    }

                    Thread.Sleep(monitoringFrequencyMinutes * 60 * 1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static void TerminateProgram()
    {
        Console.WriteLine("\nProgram terminated.");
        Environment.Exit(0);
    }

    public static void Log(StreamWriter logFile, string message)
    {
        try
        {
            logFile.WriteLine($"{DateTime.Now} - {message}");
            logFile.Flush();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}
