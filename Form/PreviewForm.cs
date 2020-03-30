﻿using System;
using System.IO;
using TptMain.Models;

namespace TptMain.Form
{
    /// <summary>
    /// Displays a downloaded preview file using the WebBrowser control.
    ///
    /// Note: Requires a PDF reader to be installed on system and registered
    /// as browser addin (e.g., Adobe Reader).
    /// </summary>
    public partial class PreviewForm : System.Windows.Forms.Form
    {
        /// <summary>
        /// Preview temp file to display.
        /// </summary>
        private FileInfo _previewFile;

        /// <summary>
        /// Basic ctor.
        /// </summary>
        public PreviewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets preview file in member data, sets up form caption, and displays file.
        /// </summary>
        /// <param name="previewJob">Preview job (required).</param>
        /// <param name="previewFile">Preview temp file (required).</param>
        public virtual void SetPreviewFile(PreviewJob previewJob, FileInfo previewFile)
        {
            _ = previewJob ?? throw new ArgumentNullException(nameof(previewJob));
            _ = previewFile ?? throw new ArgumentNullException(nameof(previewFile));

            _previewFile = previewFile;

            Text = $"Preview - Project: \"{previewJob.ProjectName}\", Format: {previewJob.BookFormat}, Font: {previewJob.FontSizeInPts}pt, Leading: {previewJob.FontLeadingInPts}pt";
            webPreview.Navigate(new Uri(_previewFile.FullName).AbsoluteUri);
        }
    }
}