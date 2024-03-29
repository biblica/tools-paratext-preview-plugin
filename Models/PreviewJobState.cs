﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TptMain.Models
{
    /// <summary>
    /// Represents the possible states of a Preview Job. These are ordered by the steps within the processing workflow.
    /// </summary>
    public enum JobStateEnum
    {
        Submitted = 1,
        Started = 2,
        GeneratingTemplate = 3,
        TemplateGenerated = 4,
        GeneratingTaggedText = 5,
        TaggedTextGenerated = 6,
        GeneratingPreview = 7,
        Cancelled = 8,
        Error = 9,
        PreviewGenerated = 10
    }

    /// <summary>
    /// The potential sources of records about the job state
    /// </summary>
    public enum JobStateSourceEnum
    {
        JobValidation,
        TemplateGeneration,
        TaggedTextGeneration,
        PreviewGeneration,
        GeneralManagement
    }

    /// <summary>
    /// For sorting based on oldest record
    /// </summary>
    public class PreviewJobStateOldestComparator : IComparer<PreviewJobState>
    {
        public int Compare(PreviewJobState x, PreviewJobState y)
        {
            return x.DateSubmitted.CompareTo(y.DateSubmitted);
        }
    }

    /// <summary>
    /// A preview job state. There may be more than one of these states associated with a preview job record
    /// </summary>
    public class PreviewJobState : IEquatable<PreviewJobState>, IComparable<PreviewJobState>
    {
        /// <summary>
        /// Default constructor. The only reason we want this is due to the ability to deepclone using json serialization.
        /// </summary>
        public PreviewJobState()
        {
            State = JobStateEnum.Submitted;
        }

        /// <summary>
        /// Constructor, specifying state
        /// </summary>
        /// <param name="state">The state to register</param>
        public PreviewJobState(JobStateEnum state)
        {
            State = state;
        }

        /// <summary>
        /// Constructor specifying state and source
        /// </summary>
        /// <param name="state">The state to register</param>
        /// <param name="source">The source of the state</param>
        public PreviewJobState(JobStateEnum state, JobStateSourceEnum source) : this(state)
        {
            Source = source;
        }

        /// <summary>
        /// Constructor specifying state, source, and time
        /// 
        /// This constructor is mainly used for testing purposes to expire jobs correctly
        /// </summary>
        /// <param name="state">The state to register</param>
        /// <param name="source">The source of the state</param>
        /// <param name="dateTime">The date and time of the entry if not using the default (now)</param>
        public PreviewJobState(JobStateEnum state, JobStateSourceEnum source, DateTime dateTime) : this(state)
        {
            Source = source;
            DateSubmitted = dateTime;
        }

        /// <summary>
        /// Database unique id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// When this state was added to the job. This is for reference purposes and too see what the most recent state is
        /// </summary>
        public DateTime DateSubmitted { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The state of this job.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStateEnum State { get; set; }

        /// <summary>
        /// The processing source reporting the job state
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStateSourceEnum Source { get; set; } = JobStateSourceEnum.GeneralManagement;

        /// <summary>
        /// Override for Sorting based on Date. If they are the same, compare based on status.
        /// </summary>
        /// <param name="other">The record to compare to</param>
        /// <returns>Standard CompareTo responses, see IComparer.
        /// In this case, return the comparison b/t states if the dates are equal, otherwise the date comparison.
        /// </returns>
        public int CompareTo(PreviewJobState other)
        {
            if (other == null)
            {
                return 1;
            }
            else
            {
                if (this.DateSubmitted.Equals(other.DateSubmitted))
                {
                    return ((int)this.State).CompareTo((int)other.State);
                }
                else
                {
                    return this.DateSubmitted.CompareTo(other.DateSubmitted);
                }
            }
        }

        /// <summary>
        /// Override the Equals to just the state so that we can find if the set of states contains the looked for one
        /// </summary>
        /// <param name="other">The record to compare this one to</param>
        /// <returns>Standard responses for Equals. In this case, comparing the State.</returns>
        public bool Equals(PreviewJobState other)
        {
            if (other == null) return false;
            return this.State.Equals(other.State);
        }
    }
}
