using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace org.theGecko.Utilities
{
	public static class EnumExtensions
	{
		public static T EnumParse<T>(this string value)
		{
			T result = default(T);

			if (!string.IsNullOrEmpty(value))
			{
				if (Enum.IsDefined(typeof(T), value))
				{
					result = (T)Enum.Parse(typeof(T), value, true);
				}
				else
				{
					foreach (string s in Enum.GetNames(typeof(T)))
					{
						if (s.Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							result = (T)Enum.Parse(typeof(T), value, true);
						}
					}
				}
			}
			return result;
		}

        /// <summary>
        /// Extension method.
        /// Call to get value of a DescriptionAttribute on an enum item.
        /// </summary>
        /// <param name="enumValue">
        /// The enum value to retrieve the description for.
        /// </param>
        /// <returns>
        /// The description string associated with the enum value.
        /// </returns>
        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());
            var da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (da == null)
            {
                return enumValue.ToString();
            }
            else
            {
                return da.Description;
            }
        }
	}
}
