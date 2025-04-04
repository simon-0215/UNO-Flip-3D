using System;
using System.IO;
using System.Text;

/// <summary>
/// 所有日志同时输出到控制台和文件
/// </summary>
public class DualLogWriter : TextWriter
{
    private readonly TextWriter _consoleWriter;
    private readonly StreamWriter _fileWriter;
    private readonly object _lock = new object();

    public DualLogWriter(string filePath)
    {
        _consoleWriter = Console.Out;
        _fileWriter = new StreamWriter(filePath, true, Encoding.UTF8)
        {
            AutoFlush = true
        };
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void WriteLine(string value)
    {
        lock (_lock)
        {
            _consoleWriter.WriteLine(value);
            _fileWriter.WriteLine(value);
        }
    }

    public override void Write(string value)
    {
        lock (_lock)
        {
            _consoleWriter.Write(value);
            _fileWriter.Write(value);
        }
    }

    protected override void Dispose(bool disposing)
    {
        lock (_lock)
        {
            _fileWriter.Dispose();
            Console.SetOut(_consoleWriter);
        }
        base.Dispose(disposing);
    }
}
/*
class Program
{
    static void Main()
    {
        string logPath = $"log{DateTime.Now:yyyy-MM-dd}-server.txt";
        var dualWriter = new DualLogWriter(logPath);
        Console.SetOut(dualWriter);

        Console.WriteLine("测试日志"); // 同时输出到控制台和文件

        // 程序结束前手动释放或注册退出事件
        AppDomain.CurrentDomain.ProcessExit += (s, e) => dualWriter.Dispose();
    }
}*/

public class LogWriterUtil
{
    public static void writeTimeLine(string log = "")
    {
        string timeStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"------------------ {timeStr}[{log}] ------------------");
    }

    /// <summary>
    /// 所有日志同时输出到控制台和文件
    /// </summary>
    /// <param name="logPath"></param>
    public static void Init(string logPath)
    {
        #region 日志同时输出到控制台和文件中
        var dualWriter = new DualLogWriter(logPath);
        Console.SetOut(dualWriter);

        // 程序结束前手动释放或注册退出事件
        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            writeTimeLine("App Exit");
            dualWriter.Dispose();
        };

        writeTimeLine("App Start");
        Console.WriteLine($"测试日志:所有日志同时输出到控制台和文件{logPath}");
        #endregion
    }
    /// <summary>
    /// 所有日志同时输出到控制台和文件
    /// </summary>
    public static void Init()
    {
        string logPath = $"log{DateTime.Now:yyyy-MM-dd}-server.txt";
        Init(logPath);
    }
}

