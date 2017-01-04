using System;
using System.Collections.Generic;
using System.Text;

namespace EricUtility
{
    /// <summary>
    /// Degré donné au message
    /// </summary>
    public enum LogLevel
    {
        MessageVeryLow = 0,
        MessageLow = 10,
        Message = 20,
        MessageHigh = 30,
        MessageVeryHigh = 40,
        WarningLow = 50,
        Warning = 60,
        WarningHigh = 70,
        ErrorLow = 80,
        Error = 90,
        ErrorHigh = 100
    }

    /// <summary>
    /// Delegate d'un log
    /// </summary>
    /// <param name="from">Source du message</param>
    /// <param name="message">Le message</param>
    /// <param name="level">Le degré du message (Voir "LogLevel" pour des valeurs préféfinies) (plus il est élevé, plus il est important)</param>
    public delegate void LogDelegate(string from, string message, int level);

    public static class LogManager
    {
        /// <summary>
        /// Évenement déclenché lorsque quelqu'un log quelquechose
        /// </summary>
        public static event LogDelegate MessageLogged;

        /// <summary>
        /// Log d'un message formatté
        /// </summary>
        /// <param name="from">Source du message</param>
        /// <param name="format">Message formatté</param>
        /// <param name="args">Argument du message formaté</param>
        public static void Log(string from, string format, params object[] args)
        {
            Log((int)LogLevel.Message, from, format, args);
        }

        /// <summary>
        /// Log d'un message formatté
        /// </summary>
        /// <param name="level">Élément LogLevel déterminant le degré du message</param>
        /// <param name="from">Source du message</param>
        /// <param name="format">Message formatté</param>
        /// <param name="args">Argument du message formaté</param>
        public static void Log(LogLevel level, string from, string format, params object[] args)
        {
            Log((int)level, from, format, args);
        }

        /// <summary>
        /// Log d'un message formatté
        /// </summary>
        /// <param name="level">Nombre déterminant le degré du message (plus il est élevé, plus il est important)</param>
        /// <param name="from">Source du message</param>
        /// <param name="format">Message formatté</param>
        /// <param name="args">Argument du message formaté</param>
        public static void Log(int level, string from, string format, params object[] args)
        {
            if (MessageLogged != null)
                MessageLogged(from, String.Format(format, args), level);
        }
    }
}

/*/////////////////////////
///////////////////////////
// EXEMPLE D'UTILISATION //
///////////////////////////
///////////////////////////
public partial class MainForm : Form
{
    StreamWriter lowlogger;
    StreamWriter logger;
    Tournament tournoi;

    public MainForm()
    {
        InitializeComponent();
        LogManager.MessageLogged += new LogDelegate(LogManager_MessageLogged);
        btnStartTournament_Click(this, new EventArgs()); // Automatic start !
    }

    void LogManager_MessageLogged(string from, string message, int level)
    {
        DateTime now = DateTime.Now;
        String strNow = String.Format("{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
        string msg;
        if (level >= (int)LogLevel.ErrorLow)
            msg = String.Format("[{0}]<{1}> !!!!!! ERROR !!!!!! {2}", strNow, from, message);
        else
            msg = String.Format("[{0}]<{1}> {2}", strNow, from, message);
        if (level >= (int)LogLevel.Message)
        {
            Console.WriteLine(msg);
            lock (logger)
            {
                logger.WriteLine(msg);
            }
        }
        lock (lowlogger)
        {
            lowlogger.WriteLine(msg);
        }
    }
    private void btnStartTournament_Click(object sender, EventArgs e)
    {
        logger = new StreamWriter("log.txt");
        lowlogger = new StreamWriter("logDetail.txt");
        btnStartTournament.Enabled = false;
        tournoi.StartTournament();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (logger != null)
            logger.Flush();
        if (lowlogger != null)
            lowlogger.Flush();
        logger.Close();
        lowlogger.Close();
    }
}
*/