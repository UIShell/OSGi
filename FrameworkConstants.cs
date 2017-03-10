namespace UIShell.OSGi
{
    using System;
    using System.Globalization;
    using System.IO;

    public class FrameworkConstants
    {
        public const string ASSEMBLY_SEARCH_FOLDERS = "bin";
        public static readonly string ASSEMBLY_SEARCH_FOLDERS_FOR_DEBUG = string.Format("bin{0}debug;bin{0}Debug", Path.DirectorySeparatorChar);
        public static readonly string ASSEMBLY_SEARCH_FOLDERS_FOR_NONDEBUG = string.Format("bin{0}release;bin{0}Release;bin{0}publish;bin{0}Publish", Path.DirectorySeparatorChar);
        public const string BUNDLE_UPDATE_FOLDER = "Update";
        public const string CONSTANT_NAME_BASEDIRECTORY = "{BaseDirectory}";
        public const string CONSTANT_NAME_BUNDLEABSOLUTEPATH = "{BundleAbsolutePath}";
        public const string CONSTANT_NAME_BUNDLELOCATION = "{BundleLocation}";
        public const string CONSTANT_NAME_BUNDLERELATIVEPATH = "{BundleRelativePath}";
        public static CultureInfo DEFAULT_CULTURE = new CultureInfo(string.Empty);
        public static string DEFAULT_LICENSE_FILE = "license.lic";
        public const int DEFAULT_MILLISECONDS_TIMEOUT_ONLOCK = 0x2710;
        public const string DEFAULT_PLUGINS_FOLDER = "Plugins";
        public static Version DEFAULT_VERSION = new Version(1, 0, 0, 0);
        public const string EVENT_THREAD_SLOT = "EventSlotName";
        public const string LOG_FILE = "log.txt";
        public const string LOG_NAME = "UIShell.OSGi.Logger";
        public const string MANIFEST_EXTENSION_XPATH = "/Bundle/Extension";
        public const string MANIFEST_EXTENSION_XPATH_WITHNS = "/ebm2:Bundle/ebm2:Extension";
        public const string MANIFEST_EXTENSIONPOINT_XPATH = "/Bundle/ExtensionPoint";
        public const string MANIFEST_EXTENSIONPOINT_XPATH_WITHNS = "/ebm2:Bundle/ebm2:ExtensionPoint";
        public const string MANIFEST_EXTENSIONS_XPATH = "//Extensions";
        public const string MANIFEST_EXTENSIONS_XPATH_WITHNS = "//ebm2:Extensions";
        public const string MANIFEST_FILE_NAME = "Manifest.xml";
        public const string MANIFEST_NAMESPACE = "urn:uiosp-bundle-manifest-2.0";
        public const string MANIFEST_NAMESPACE_ABBREVIATION = "ebm2";
        public const bool MULTIPLE_VERSIONS_SUPPORT = true;
        public const string PERSISTENT_FILE_NAME = "persistent.xml";
        public const string SYSTEM_BUNDLE_NAME = "SystemBundle";
        public const string SYSTEM_BUNDLE_SYMBOLICNAME = "UIShell.OSGi.SystemBundle";
    }
}

