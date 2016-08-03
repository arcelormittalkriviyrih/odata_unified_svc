using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODataRestierDynamic.Models
{
    /// <summary>	Class for dynamic info service object. </summary>
    public class DynamicServiceInfoObject
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public DynamicServiceInfoObject()
        {
            
        }

        /// <summary>	Gets or sets the IIS version. </summary>
        ///
        /// <value>	The IIS version. </value>
        public string IISVersion { get; set; }

        /// <summary>	Gets or sets the Target Framework. </summary>
        ///
        /// <value>	The Target Framework. </value>
        public string TargetFramework { get; set; }

        /// <summary>	Gets or sets the App Domain App Path. </summary>
        ///
        /// <value>	The App Domain App Path. </value>
        public string AppDomainAppPath { get; set; }

        /// <summary>	Gets or sets the Assembly Dictionary. </summary>
        ///
        /// <value>	The Assembly Dictionary. </value>
        public string AssemblyDictionary { get; set; }

        /// <summary>	Gets or sets the Application Settings. </summary>
        ///
        /// <value>	The Application Settings. </value>
        public string AppSettings { get; set; }

        /// <summary>	Gets or sets the SQL Product Version. </summary>
        ///
        /// <value>	The SQL Product Version. </value>
        public string SQLProductVersion { get; set; }

        /// <summary>	Gets or sets the SQL Patch Level. </summary>
        ///
        /// <value>	The SQL Patch Level. </value>
        public string SQLPatchLevel { get; set; }

        /// <summary>	Gets or sets the SQL Product Edition. </summary>
        ///
        /// <value>	The SQL Product Edition. </value>
        public string SQLProductEdition { get; set; }

        /// <summary>	Gets or sets the SQL CLR Version. </summary>
        ///
        /// <value>	The SQL CLR Version. </value>
        public string SQLCLRVersion { get; set; }

        /// <summary>	Gets or sets the SQL Default Collation. </summary>
        ///
        /// <value>	The SQL Default Collationn. </value>
        public string SQLDefaultCollation { get; set; }

        /// <summary>	Gets or sets the SQL Instance. </summary>
        ///
        /// <value>	The SQL Instance. </value>
        public string SQLInstance { get; set; }

        /// <summary>	Gets or sets the SQL Server Name. </summary>
        ///
        /// <value>	The SQL Server Name. </value>
        public string SQLServerName { get; set; }
    }
}