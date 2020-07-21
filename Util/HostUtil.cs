﻿using AddInSideViews;
using System;
using System.IO;
using System.Windows.Forms;

namespace TptMain.Util
{
    /// <summary>
    /// Process-wide error utilities.
    /// </summary>
    public class HostUtil
    {
        /// <summary>
        /// Private singleton instance.
        /// </summary>
        private static readonly HostUtil _instance = new HostUtil();

        /// <summary>
        /// Thread-safe singleton instance.
        /// </summary>
        public static HostUtil Instance => _instance;

        /// <summary>
        /// Global reference to plugin, to route logging.
        /// </summary>
        private TypesettingPreviewPlugin _typesettingPreviewPlugin;

        /// <summary>
        /// Global reference to host interface, providing Paratext services including logging.
        /// </summary>
        private IHost _host;

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public TypesettingPreviewPlugin TypesettingPreviewPlugin { set => _typesettingPreviewPlugin = value; }

        /// <summary>
        /// Property for assignment from plugin entry method.
        /// </summary>
        public IHost Host { set => _host = value; }

        /// <summary>
        /// Reports exception to log and message box w/prefix text.
        ///
        /// Either prefixText (or) ex must be non-null.
        /// </summary>
        /// <param name="prefixText">Prefix text (optional, may be null; default used when null).</param>
        /// <param name="ex">Exception (optional, may be null).</param>
        public void ReportError(string prefixText, Exception ex)
        {
            if (prefixText == null && ex == null)
            {
                throw new ArgumentNullException("prefixText (or) ex must be non-null");
            }

            var messageText = (prefixText ?? "Error: Please contact support.")
                + (ex == null ? string.Empty
                    : Environment.NewLine + Environment.NewLine
                    + "Details: " + ex + Environment.NewLine);

            MessageBox.Show(messageText, "Notice...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogLine($"Error: {messageText}", true);
        }

        /// <summary>
        /// Log text to Paratext's app log and the console.
        /// </summary>
        /// <param name="inputText">Input text (required).</param>
        /// <param name="isError">Error flag.</param>
        public void LogLine(string inputText, bool isError)
        {
            (isError ? Console.Error : Console.Out).WriteLine(inputText);
            _host?.WriteLineToLog(_typesettingPreviewPlugin, inputText);
        }

        /// <summary>
        /// Finds the path for the project to pass to GetFootnoteCallerSequence.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public DirectoryInfo GetParatextProjectDirectory(string projectName)
        {
            // validate inputs
            _ = projectName ?? throw new ArgumentNullException(nameof(projectName));

            // We're using the host's figure path to determine project directory, 
            // as the Paratext AddinViews doesn't appear to provide a function to do this
            var figurePath = _host.GetFigurePath(projectName, false);

            // If the non-local figure path is unavailable, get the local figure path
            if (figurePath == null)
            {
                figurePath = _host.GetFigurePath(projectName, true);

                if (figurePath == null)
                {
                    throw new Exception("We couldn't find the project path for " + projectName);
                }

                // return the project directory from the local figure directory. EG: usNIV11/local/figure/ -> usNIV11
                return Directory.GetParent(Directory.GetParent(figurePath).FullName);
            }

            // return the project directory from the non-local figure directory. EG: usNIV11/figure/ -> usNIV11
            return Directory.GetParent(figurePath);
        }
    }
}