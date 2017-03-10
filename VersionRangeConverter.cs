namespace UIShell.OSGi
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;

    internal class VersionRangeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType != typeof(string))
            {
                return base.CanConvertFrom(context, sourceType);
            }
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType != typeof(InstanceDescriptor))
            {
                return base.CanConvertTo(context, destinationType);
            }
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string version = value as string;
            if (version == null)
            {
                return base.ConvertFrom(context, culture, value);
            }
            return new VersionRange(version);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            VersionRange range = value as VersionRange;
            if ((null != range) && (destinationType == typeof(string)))
            {
                return range.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            if (propertyValues == null)
            {
                throw new ArgumentNullException("propertyValues");
            }
            object obj3 = propertyValues["min"];
            object obj2 = propertyValues["max"];
            if (((obj3 == null) || (obj2 == null)) || (!(obj3 is string) || !(obj2 is string)))
            {
                throw new ArgumentException("PropertyValueInvalidEntry");
            }
            return new VersionRange((string) obj3, (string) obj2);
        }

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) => true;

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) => 
            TypeDescriptor.GetProperties(typeof(VersionRange), attributes).Sort(new string[] { "min", "max" });

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => true;
    }
}

