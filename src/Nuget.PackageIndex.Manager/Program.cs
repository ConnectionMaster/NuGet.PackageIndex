﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
namespace Nuget.PackageIndex.Manager
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var indexManager = new PackageIndexManager();
            indexManager.Run(args);
        }
    }
}
