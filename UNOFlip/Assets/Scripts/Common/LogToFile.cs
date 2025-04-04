using UnityEngine;
using System.IO;
using System;

public class LogToFile : MonoBehaviour
{
    private StreamWriter _writer;
    private string _currentDate;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UpdateLogFile();
        Application.logMessageReceived += HandleLog;
    }

    void UpdateLogFile()
    {
        _currentDate = DateTime.Now.ToString("yyyy-MM-dd HHmmss");
        //{UnityEngine.Random.Range(1, 10000)}
        string path = Path.Combine(Application.persistentDataPath, $"log{_currentDate}-client.txt");
        print("log path: " + path);
        _writer = new StreamWriter(path, true, System.Text.Encoding.UTF8)
        {
            AutoFlush = true
        };
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        /*
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        if (today != _currentDate)
        {
            _writer.Close();
            UpdateLogFile();
        }
        */
        _writer.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{type}] {message}");
        //if (type == LogType.Error || type == LogType.Exception)
        //    _writer.WriteLine(stackTrace);
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        _writer?.Close();
    }
}