namespace UIShell.OSGi.Utility
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using OSGi;

    internal class TypeConverterUtility
    {
        private static IDictionary _converters = new Hashtable();

        static TypeConverterUtility()
        {
            RegisterConverter(typeof(Version), new VersionConverter());
            RegisterConverter(typeof(VersionRange), new VersionRangeConverter());
        }

        public static bool CanConvert(object Obj, Type TargetType) => 
            CanConvert(Obj.GetType(), TargetType);

        public static bool CanConvert(Type ObjType, Type TargetType)
        {
            if ((ObjType == TargetType) || ObjType.IsAssignableFrom(TargetType))
            {
                return true;
            }
            TypeConverter converter = GetConverter(ObjType);
            if ((converter != null) && converter.CanConvertTo(TargetType))
            {
                return true;
            }
            converter = GetConverter(TargetType);
            return ((converter != null) && converter.CanConvertFrom(ObjType));
        }

        public static object ConvertTo(object Obj, Type TargetType)
        {
            if (Obj.GetType() == TargetType)
            {
                return Obj;
            }
            if ((Obj == null) || !CanConvert(Obj, TargetType))
            {
                return null;
            }
            Type objType = Obj.GetType();
            try
            {
                TypeConverter converter = ToConverter(objType, TargetType);
                if (converter != null)
                {
                    return converter.ConvertTo(Obj, TargetType);
                }
                converter = FromConverter(objType, TargetType);
                if (converter != null)
                {
                    return converter.ConvertFrom(Obj);
                }
                return Convert.ChangeType(Obj, TargetType);
            }
            catch
            {
                return null;
            }
        }

        public static TypeConverter FromConverter(Type ObjType, Type TargetType)
        {
            TypeConverter converter = null;
            converter = GetConverter(TargetType);
            if ((converter != null) && converter.CanConvertFrom(ObjType))
            {
                return converter;
            }
            return null;
        }

        public static TypeConverter GetConverter(Type type)
        {
            AssertUtility.ArgumentNotNull(type, "type");
            TypeConverter converter = (TypeConverter) _converters[type];
            if (converter != null)
            {
                return converter;
            }
            if (type.IsEnum)
            {
                return new EnumConverter(type);
            }
            return TypeDescriptor.GetConverter(type);
        }

        public static TypeConverter GetConvertor(object Obj) => 
            GetConverter(Obj.GetType());

        public static TypeConverter GetConvertor(Type ObjType) => 
            GetConverter(ObjType);

        public static void RegisterConverter(Type type, TypeConverter converter)
        {
            AssertUtility.ArgumentNotNull(type, "type");
            AssertUtility.ArgumentNotNull(converter, "converter");
            lock (_converters.SyncRoot)
            {
                _converters[type] = converter;
            }
        }

        public static TypeConverter SuitConverter(Type ObjType, Type TargetType)
        {
            TypeConverter converter = ToConverter(ObjType, TargetType);
            if (converter != null)
            {
                return converter;
            }
            return FromConverter(ObjType, TargetType);
        }

        public static TypeConverter ToConverter(Type ObjType, Type TargetType)
        {
            TypeConverter converter = null;
            converter = GetConverter(ObjType);
            if ((converter != null) && converter.CanConvertTo(TargetType))
            {
                return converter;
            }
            return null;
        }
    }
}

