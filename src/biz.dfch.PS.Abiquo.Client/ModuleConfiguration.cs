/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Management.Automation;
using System.Security;

namespace biz.dfch.PS.Abiquo.Client
{
    /// <summary>
    /// The factory / manager class for returning the module context
    /// </summary>
    public class ModuleConfiguration
    {
        /// <summary>
        /// The default name of the configuration file
        /// </summary>
        public const string CONFIGURATION_FILE_NAME = "biz.dfch.PS.Abiquo.Client.config";

        /// <summary>
        /// The name of the module configuration variable
        /// </summary>
        public const string MODULE_VARIABLE_NAME = "biz_dfch_PS_Abiquo_Client";

        private static readonly ModuleContext _moduleContext = new ModuleContext();

        /// <summary>
        /// Returns the current module context (singleton)
        /// </summary>
        /// <returns>ModuleContext containing module configuration information</returns>
        public static ModuleContext Current
        {
            get { return _moduleContext; }
        }

        /// <summary>
        /// Gets and validates a configuration file name or returns the default configuration file name
        /// </summary>
        /// <param name="fileInfo">FileInfo specifying a configuration file name or null</param>
        /// <returns>FileInfo specifying the configuration file name</returns>
        public static FileInfo ResolveConfigurationFileInfo(FileInfo fileInfo)
        {
            Contract.Ensures(null != Contract.Result<FileInfo>());

            if (null == fileInfo)
            {
                var location = typeof(ImportConfiguration).Assembly.Location;
                Contract.Assert(!string.IsNullOrWhiteSpace(location));

                var path = Path.GetDirectoryName(location);
                Contract.Assert(Directory.Exists(path), path);

                var configFile = Path.Combine(path, CONFIGURATION_FILE_NAME);

                fileInfo = new FileInfo(configFile);
            }

            Contract.Assert(!Directory.Exists(fileInfo.FullName), string.Format(Messages.ImportConfigurationDirectoryExists, fileInfo.FullName));
            Contract.Assert(File.Exists(fileInfo.FullName), string.Format(Messages.ImportConfigurationFileDoesNotExist, fileInfo.FullName));

            return fileInfo;
        }

        /// <summary>
        /// Gets the module context section from the default configuration file
        /// </summary>
        /// <returns>ModuleContextSection of current module</returns>
        public static ModuleContextSection GetModuleContextSection()
        {
            Contract.Ensures(null != Contract.Result<ModuleContextSection>());

            var fileInfo = ResolveConfigurationFileInfo(null);
            return GetModuleContextSection(fileInfo);
        }

        /// <summary>
        /// Gets the module context section from the specified configuration file
        /// </summary>
        /// <param name="fileInfo">Specifies the location of the configuration file to be loaded</param>
        /// <returns>ModuleContextSection of current module</returns>
        public static ModuleContextSection GetModuleContextSection(FileInfo fileInfo)
        {
            Contract.Requires(null != fileInfo);
            Contract.Ensures(null != Contract.Result<ModuleContextSection>());

            var configurationFileMap = new ConfigurationFileMap(fileInfo.FullName);
            Contract.Assert(null != configurationFileMap, string.Format(Messages.ImportConfigurationConfigurationFileMapCreateFailed, fileInfo.FullName));

            var configuration = ConfigurationManager.OpenMappedMachineConfiguration(configurationFileMap);
            Contract.Assert(null != configuration, string.Format(Messages.ImportConfigurationConfigurationOpenFailed, fileInfo.FullName));
            Contract.Assert(configuration.HasFile, string.Format(Messages.ImportConfigurationConfigurationHasFile, fileInfo.FullName));

            var moduleContextSection = configuration.GetSection(ModuleContextSection.SECTION_NAME) as ModuleContextSection;
            Contract.Assert(null != moduleContextSection, string.Format(Messages.ImportConfigurationSectionOpenFailed, fileInfo.FullName, ModuleContextSection.SECTION_NAME));

            return moduleContextSection;
        }


        /// <summary>
        /// Saves the specified configuration section to the configuration file
        /// </summary>
        /// <param name="fileInfo">Configuration file name to save to</param>
        /// <param name="section">Module context to save</param>
        public static void Save(FileInfo fileInfo, ConfigurationSection section)
        {
            Contract.Requires(null != fileInfo);
            Contract.Requires(null != section);

            var configurationFileMap = new ConfigurationFileMap(fileInfo.FullName);
            Contract.Assert(null != configurationFileMap, string.Format(Messages.ImportConfigurationConfigurationFileMapCreateFailed, fileInfo.FullName));

            var configuration = ConfigurationManager.OpenMappedMachineConfiguration(configurationFileMap);
            Contract.Assert(null != configuration, string.Format(Messages.ImportConfigurationConfigurationOpenFailed, fileInfo.FullName));
            Contract.Assert(configuration.HasFile, string.Format(Messages.ImportConfigurationConfigurationHasFile, fileInfo.FullName));

            configuration.Sections.Remove(ModuleContextSection.SECTION_NAME);
            configuration.Save(ConfigurationSaveMode.Minimal);

            configuration.Sections.Add(ModuleContextSection.SECTION_NAME, section);
            configuration.Save(ConfigurationSaveMode.Minimal);
        }

        /// <summary>
        /// Creates a new ModuleContextSection from the current module's ModuleContext
        /// </summary>
        /// <returns>Converted ModuleContextSection from ModuleContext</returns>
        public static ModuleContextSection ConvertToModuleContextSection()
        {
            Contract.Ensures(null != Contract.Result<ModuleContextSection>());
            
            var section = new ModuleContextSection();
            
            switch (Current.AuthenticationType)
            {
                case EnterServer.ParameterSets.PLAIN:
                case EnterServer.ParameterSets.CREDENTIAL:
                case EnterServer.ParameterSets.OAUTH2:
                    break;
                default:
                    const bool isValidAuthenticationType = false;
                    Contract.Assert(isValidAuthenticationType, section.AuthenticationType);
                    break;
            }

            if (null != Current.Credential)
            {
                section.Username = Current.Credential.UserName;
                section.Password = Current.Credential.GetNetworkCredential().Password;
            }
            section.OAuth2Token = Current.OAuth2Token;
            section.ApiVersion = Current.ApiVersion;
            section.AuthenticationType = Current.AuthenticationType;
            section.Uri = Current.Uri;

            return section;
        }
            
        /// <summary>
        /// Sets the module context from a configuration section
        /// </summary>
        public static void SetModuleContext(ModuleContextSection section)
        {
            Contract.Requires(null != section);

            switch (section.AuthenticationType)
            {
                case EnterServer.ParameterSets.PLAIN:
                case EnterServer.ParameterSets.CREDENTIAL:
                case EnterServer.ParameterSets.OAUTH2:
                    break;
                default:
                    const bool isValidAuthenticationType = false;
                    Contract.Assert(isValidAuthenticationType, section.AuthenticationType);
                    break;
            }
                
            if (!string.IsNullOrWhiteSpace(section.Username) && !string.IsNullOrWhiteSpace(section.Password))
            {
                var secureString = new SecureString();
                foreach (var c in section.Password)
                {
                    secureString.AppendChar(c);
                }
                Current.Credential = new PSCredential(section.Username, secureString);
            }

            Current.OAuth2Token = section.OAuth2Token;
            Current.ApiVersion = section.ApiVersion;
            Current.AuthenticationType = section.AuthenticationType;
            Current.Uri = section.Uri;
        }
    }
}
