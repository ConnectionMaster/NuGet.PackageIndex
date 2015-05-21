﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Text;

namespace Nuget.PackageIndex.Models
{
    /// <summary>
    /// Namespace metadata exposed publicly 
    /// </summary>
    public class NamespaceInfo
    { 
        public string Name { get; set; }
        public string AssemblyName { get; set; }
        public string PackageName { get; set; }
        public string PackageVersion { get; set; }
        public List<string> TargetFrameworks { get; internal set; }

        public NamespaceInfo()
        {
            TargetFrameworks = new List<string>();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Name)
                         .Append(",")
                         .Append(AssemblyName)
                         .Append(",")
                         .Append(PackageName)
                         .Append(" ")
                         .Append(PackageVersion)
                         .Append(", Target Frameworks: ")
                         .Append(GetTargetFrameworksString());

            return stringBuilder.ToString();
        }

        protected string GetTargetFrameworksString()
        {
            return string.Join(";", TargetFrameworks) ?? "";
        }
    }
}
