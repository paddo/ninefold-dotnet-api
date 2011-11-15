﻿using System;
using System.Xml.Linq;
using Ninefold.Core;

namespace Ninefold.Compute.Messages
{
    public class Snapshot
    {
        public int Id { get; set; }
        public string Account { get; set; }
        public DateTime Created { get; set; }
        public string Domain { get; set; }
        public int DomainId { get; set; }
        public string IntervalType { get; set; }
        public int JobId { get; set; }
        public string JobStatus { get; set; }
        public string Name { get; set; }
        public string SnapshotType { get; set; }
        public int VolumeId { get; set; }
        public string VolumeName { get; set; }
        public string VolumeType { get; set; }

        public static Snapshot From(XElement snapshotElement)
        {
            return new Snapshot
            {
                Id = int.Parse(snapshotElement.ExtractValue("id")),
                Account = snapshotElement.ExtractValue("account"),
                Created = DateTime.Parse(snapshotElement.ExtractValue("created")),
                Domain = snapshotElement.ExtractValue("domain"),
                DomainId = int.Parse(snapshotElement.ExtractValue("domainId")),
                IntervalType = snapshotElement.ExtractValue("intervalType"),
                JobId = int.Parse(snapshotElement.ExtractValue("jobId")),
                JobStatus = snapshotElement.ExtractValue("jobStatus"),
                Name = snapshotElement.ExtractValue("name"),
                SnapshotType = snapshotElement.ExtractValue("snapshotType"),
                VolumeId = int.Parse(snapshotElement.ExtractValue("volumeId")),
                VolumeName = snapshotElement.ExtractValue("volumeName"),
                VolumeType = snapshotElement.ExtractValue("volumeType")
            };
        }
    }
}
