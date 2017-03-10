namespace UIShell.OSGi.Utility
{
    using System;
    using System.Globalization;

    public sealed class AssertUtility
    {
        private AssertUtility()
        {
        }

        public static void ArgumentAssignableFrom(object argument, Type BaseType, string name)
        {
            if (!argument.GetType().IsAssignableFrom(BaseType))
            {
                throw new ArgumentNullException(name, string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot assignable from {1}.", new object[] { name, BaseType.FullName }));
            }
        }

        public static void ArgumentAssignableFrom(object argument, Type BaseType, string name, string message)
        {
            if (!argument.GetType().IsAssignableFrom(BaseType))
            {
                throw new ArgumentNullException(name, message);
            }
        }

        public static void ArgumentHasText(string argument, string name)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(name, string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot be null or resolve to an empty string : '{1}'.", new object[] { name, argument }));
            }
        }

        public static void ArgumentHasText(string argument, string name, string message)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentNullException(name, message);
            }
        }

        public static void ArgumentNotNull(object argument, string name)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, string.Format(CultureInfo.InvariantCulture, "Argument '{0}' cannot be null.", new object[] { name }));
            }
        }

        public static void ArgumentNotNull(object argument, string name, string message)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(name, message);
            }
        }

        public static void ArgumentSameType(object argument, Type argumentType, string name)
        {
            if (!argument.GetType().Equals(argumentType))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Type of argument '{0}' must be {1}.", new object[] { name, argumentType.FullName }), name);
            }
        }

        public static void ArgumentSameType(object argument, Type argumentType, string name, string message)
        {
            if (!argument.GetType().Equals(argumentType))
            {
                throw new ArgumentException(message, name);
            }
        }

        public static void EnumDefined(Type enumType, object enumValue, string name)
        {
            if (!Enum.IsDefined(enumType, enumValue))
            {
                throw new ArgumentException(name, string.Format(CultureInfo.InvariantCulture, "Argument '{0}' is not defined in the enumeration type '{1}'.", new object[] { name, enumType.FullName }));
            }
        }

        public static void IsTrue(bool value)
        {
            IsTrue(value, string.Empty);
        }

        public static void IsTrue(bool value, string msg)
        {
            if (!value)
            {
                throw new Exception(msg);
            }
        }

        public static void NotNull(object member)
        {
            if (member == null)
            {
                throw new Exception("Invalid value.");
            }
        }

        public static void NotNull(object member, string name)
        {
            if (member == null)
            {
                throw new ArgumentNullException(name, string.Format(CultureInfo.InvariantCulture, "Member '{0}' cannot be null.", new object[] { name }));
            }
        }
    }
}

