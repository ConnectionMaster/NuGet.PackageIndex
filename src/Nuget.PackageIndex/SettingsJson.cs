﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nuget.PackageIndex.Abstractions;

namespace Nuget.PackageIndex
{
    /// <summary>
    /// Represents settings.json file located in the index directory. If file does not
    /// exist, creates a new settings.json with default settings. When user modifies, settings.json
    /// changes are in effect after VS is re-opened.
    /// </summary>
    public class SettingsJson : ISettingsJson
    {
        private const string SettingJsonFileName = "settings.json";
        private const string DefaultJsonContent = @"{
    'includePackagePatterns' : []
}
";
        /// <summary>
        /// These are prefixes for package names that are shipped under,
        /// %ProgramFiles(x86)%\Microsoft Web Tools\DNU. We do look for packages 
        /// at multiple local folders (sources), but index only currated list of 
        /// packages. If users want to add other packages to the index they could 
        /// update file %UserProfile%\AppData\Local\Microsoft\PackageIndex\settings.json
        /// and add a regex for their packages to "include" list.
        /// </summary>
        private readonly string[] _defaultIncludeRules = new []
        {
            @"dnx\..+",
            @"entityframework\..+",
            @"entityframework7\..+",
            @"microsoft\..+",
            @"newtonsoft\..+",
            @"system\..+",
            @"remotion\..+"
        };

        private readonly IFileSystem _fileSystem;
        private string _settingsJsonPath;

        public SettingsJson(string indexDirectory)
            : this(indexDirectory, new FileSystem())
        {

        }

        internal SettingsJson(string indexDirectory, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _settingsJsonPath = Path.Combine(indexDirectory, SettingJsonFileName);
        }

        private JObject InitializeSettingsJson()
        {
            JObject jObject = null;
            if (_fileSystem.FileExists(_settingsJsonPath))
            {
                jObject = ReadJsonFromFile(_settingsJsonPath);
            } 
            else
            {
                jObject = GenerateDefaultSettingsJson();
                WriteJsonToFile(jObject);
            }

            return jObject;
        }

        private JObject GenerateDefaultSettingsJson()
        {
            var jObject = JObject.Parse(DefaultJsonContent);
            JArray includePackagePaterns = (JArray)jObject["includePackagePatterns"];
            foreach(var pattern in _defaultIncludeRules)
            {
                includePackagePaterns.Add(pattern);
            }

            return jObject;
        }

        private JObject ReadJsonFromFile(string filePath)
        {
            JObject result = null;

            try
            {
                var fileContent = _fileSystem.FileReadAllText(filePath);
                result = JObject.Parse(fileContent);
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }

            return result;
        }

        private void WriteJsonToFile(JObject json)
        {
            try
            {
                if (_fileSystem.FileExists(_settingsJsonPath))
                {
                    _fileSystem.FileDelete(_settingsJsonPath);
                }

                using (FileStream fs = File.Open(_settingsJsonPath, FileMode.OpenOrCreate))
                using (StreamWriter sw = new StreamWriter(fs))
                using (JsonWriter jw = new JsonTextWriter(sw))
                {
                    jw.Formatting = Formatting.Indented;

                    json.WriteTo(jw);
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }
        }

        private IEnumerable<string> ReadJsonArray(string nodeName)
        {
            try
            {
                var settingsJson = InitializeSettingsJson();
                if (settingsJson != null)
                {
                    var array = settingsJson[nodeName] as JArray;
                    if (array != null)
                    {
                        return array.Select(x => x.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Write(e.ToString());
            }

            return null;
        }

        private List<string> GetIncludePackagePatterns()
        {
            List<string> result = null;

            var includeRulesFromJsonFile = ReadJsonArray("includePackagePatterns");
            if (includeRulesFromJsonFile != null)
            {
                result = new List<string>(includeRulesFromJsonFile);
            }

            return result ?? new List<string>(_defaultIncludeRules);
        }

        #region ISettingsJson 

        private List<string> _includePackagePatterns;
        public List<string> IncludePackagePatterns
        {
            get
            {
                if (_includePackagePatterns == null)
                {
                    _includePackagePatterns = GetIncludePackagePatterns();
                }

                return _includePackagePatterns;
            }
        }

        #endregion
    }
}
