namespace UIShell.OSGi.Core.Service
{
    using OpenLicense;
    using OpenLicense.LicenseFile;
    using OpenLicense.LicenseFile.Constraints;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using OSGi;
    using Configuration.BundleManifest;
    using Core;
    using Utility;

    internal class LicenseService
    {
        internal static readonly LicenseType FrameworkLicenseType;
        internal static readonly bool ValidateCoreLicense = false;

        static LicenseService()
        {
            BundleLicenseFile = FrameworkConstants.DEFAULT_LICENSE_FILE;
            FrameworkLicenseType = ValidateFrameworkLicense();
        }

        internal static void EnsureHasAvailableBundleLicense()
        {
            if (ValidateCoreLicense)
            {
                bool flag;
                switch (FrameworkLicenseType)
                {
                    case LicenseType.const_0:
                    case LicenseType.UIOSP:
                    case LicenseType.iOpenWorksSDK:
                        return;

                    case LicenseType.AppStoreRuntime:
                    {
                        IBundleInstallerService firstOrDefaultService = BundleRuntime.Instance.GetFirstOrDefaultService<IBundleInstallerService>();
                        flag = false;
                        using (IEnumerator<KeyValuePair<string, BundleData>> enumerator = firstOrDefaultService.BundleDatas.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                KeyValuePair<string, BundleData> current = enumerator.Current;
                                LicenseStatus licenseStatus = GetLicenseStatus(current.Value);
                                if ((licenseStatus != LicenseStatus.None) && (licenseStatus != LicenseStatus.Expried))
                                {
                                    goto Label_007B;
                                }
                            }
                            break;
                        Label_007B:
                            flag = true;
                        }
                        break;
                    }
                    default:
                        LogAndThrow(new NotSupportedException("Not support framework license type."));
                        return;
                }
                if (!flag)
                {
                    LogAndThrow(new Exception("Framework starts fail, because no licensed bundle found."));
                }
            }
        }

        private static BundleLicenseItem GetBundleLicenseItem(BundleData bundle) => 
            new BundleLicenseItem(bundle.SymbolicName, (bundle.Version == null) ? FrameworkConstants.DEFAULT_VERSION : bundle.Version) { 
                LoadLicenseFolder = bundle.Path,
                UpdateLicenseFile = bundle.Path,
                LicenseFileName = <BundleLicenseFile>k__BackingField,
                SaveLicenseToIsolatedStorage = false
            };

        private static OpenLicenseFile GetFrameworkLicense(string licenseFolder)
        {
            OpenLicenseProvider provider = new OpenLicenseProvider();
            TypeLicenseItem type = new TypeLicenseItem(typeof(IFramework)) {
                SaveLicenseToIsolatedStorage = false,
                LoadLicenseFolder = licenseFolder,
                LicenseFileName = FrameworkConstants.DEFAULT_LICENSE_FILE
            };
            return (provider.GetLicense(type, true) as OpenLicenseFile);
        }

        public static LicenseStatus GetLicenseStatus(BundleData bundle)
        {
            LicenseStatus none;
            try
            {
                OpenLicenseFile licenseWithoutValidation = new OpenLicenseProvider().GetLicenseWithoutValidation(GetBundleLicenseItem(bundle), false);
                if (licenseWithoutValidation == null)
                {
                    return LicenseStatus.None;
                }
                bool flag = false;
                for (int i = 0; i < licenseWithoutValidation.Constraints.Count; i++)
                {
                    IConstraint constraint = licenseWithoutValidation.Constraints[i];
                    if (!constraint.Validate())
                    {
                        if (!(constraint is BetaConstraint))
                        {
                            goto Label_0074;
                        }
                        flag = true;
                    }
                }
                if (flag)
                {
                    return LicenseStatus.Expried;
                }
                return (licenseWithoutValidation.Product.IsLicensed ? LicenseStatus.Valid : LicenseStatus.Trial);
            Label_0074:
                none = LicenseStatus.None;
            }
            catch
            {
                none = LicenseStatus.None;
            }
            return none;
        }

        internal static OpenLicenseFile LoadBundleLicense(BundleData bundle, bool updateLicenseFile, bool allowExceptions)
        {
            OpenLicenseProvider provider = new OpenLicenseProvider();
            return (provider.GetLicense(GetBundleLicenseItem(bundle), allowExceptions, updateLicenseFile) as OpenLicenseFile);
        }

        private static void LogAndThrow(Exception ex)
        {
            FileLogUtility.Fatal(ex);
            throw ex;
        }

        public static byte[] TryGetLicense(string bundlePath)
        {
            string path = Path.Combine(bundlePath, <BundleLicenseFile>k__BackingField);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }
            return null;
        }

        public static bool UpdateLicense(string bundlePath, byte[] license)
        {
            try
            {
                File.WriteAllBytes(Path.Combine(bundlePath, <BundleLicenseFile>k__BackingField), license);
                return true;
            }
            catch (Exception exception)
            {
                FileLogUtility.Error(exception);
                return false;
            }
        }

        internal static void ValidateBundleLicense(BundleData bundle, bool updateLicenseFile)
        {
            if (ValidateCoreLicense)
            {
                LoadBundleLicense(bundle, updateLicenseFile, true);
            }
        }

        internal static LicenseType ValidateFrameworkLicense()
        {
            if (!ValidateCoreLicense)
            {
                return LicenseType.iOpenWorksSDK;
            }
            OpenLicenseFile frameworkLicense = null;
            Exception exception2 = null;
            try
            {
                frameworkLicense = GetFrameworkLicense(AppDomain.CurrentDomain.BaseDirectory);
            }
            catch (Exception exception)
            {
                exception2 = exception;
            }
            return frameworkLicense?.Product.LicenseType;
        }

        internal static string BundleLicenseFile
        {
            [CompilerGenerated]
            set
            {
                <BundleLicenseFile>k__BackingField = value;
            }
        }

        internal static bool RequireBundleLicenseValidation =>
            (FrameworkLicenseType == LicenseType.AppStoreRuntime);
    }
}

