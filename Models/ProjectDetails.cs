﻿using System;

namespace TptMain.Models
{
    /// <summary>
    /// Project details model class.
    /// </summary>
    public class ProjectDetails
    {
        /// <summary>
        /// Paratext project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Last time Paratext project was updated on server.
        /// </summary>
        public DateTime ProjectUpdated { get; set; }
    }
}