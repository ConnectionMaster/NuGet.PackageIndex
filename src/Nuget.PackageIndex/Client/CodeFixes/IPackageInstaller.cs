﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System.Threading;
using Microsoft.CodeAnalysis;
using TypeInfo = Nuget.PackageIndex.Models.TypeInfo;
using System.Collections.Generic;

namespace Nuget.PackageIndex.Client.CodeFixes
{
    /// <summary>
    /// Abstraction that knows how to install a package to a given workspace and document.
    /// For example there could be several different IDEs that would provide their own 
    /// implementation of IPackageIndexInstaller
    /// </summary>
    public interface IPackageInstaller
    {
        void InstallPackage(Workspace workspace, Document document, TypeInfo typeInfo, 
                            IEnumerable<ProjectMetadata> projects, CancellationToken cancellationToken);
    }
}
