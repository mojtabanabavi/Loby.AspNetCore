using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Loby.AspNetCore.Tools
{
    /// <summary>
    /// Provide capability to convert enums to some other types.
    /// </summary>
    public class EnumParser
    {
        /// <summary>
        /// Creates a <see cref="SelectList"/> from an enum.
        /// </summary>
        /// <typeparam name="T">
        /// A type representing an enum.
        /// </typeparam>
        /// <returns>
        /// Returns a new instance of <see cref="SelectList"/> that 
        /// created based on <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum.
        /// </exception>
        public static SelectList ToSelectList<T>()
        {
            return ToSelectList<T>(null);
        }

        /// <summary>
        /// Creates a <see cref="SelectList"/> from an enum.
        /// </summary>
        /// <typeparam name="T">
        /// A type representing an enum.
        /// </typeparam>
        /// <param name="selectedValue">
        /// An object representing the selected value.
        /// </param>
        /// <returns>
        /// Returns a new instance of <see cref="SelectList"/> that 
        /// created based on <typeparamref name="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum.
        /// </exception>
        public static SelectList ToSelectList<T>(object selectedValue)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{nameof(T)} is not an enum.");
            }

            var selectListItems = Enum
                .GetValues(typeof(T))
                .Cast<Enum>()
                .Select(e => new SelectListItem
                {
                    Text = GetDisplayName(e),
                    Value = (Convert.ToInt32(e)).ToString(),
                });

            return new SelectList(selectListItems, "Value", "Text", selectedValue);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{int, string}"/> from an enum.
        /// </summary>
        /// <param name="value">
        /// An enum to convert.
        /// </param>
        /// <returns>
        /// Returns a new instance of <see cref="Dictionary{int, string}"/> that created 
        /// based on <typeparamref name="T"/> which the key is a number and value is a suitable 
        /// display name that are assigned to each field.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum.
        /// </exception>
        public static Dictionary<int, string> ToDictionary<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"{nameof(T)} is not an enum.");
            }

            return Enum
                .GetValues(typeof(T))
                .Cast<Enum>()
                .ToDictionary(e => Convert.ToInt32(e), e => GetDisplayName(e));
        }

        /// <summary>
        /// Returns a display name for the current enum field.
        /// </summary>
        /// <param name="value">
        /// An enum field.
        /// </param>
        /// <returns>
        /// Returns the value of <see cref="DisplayNameAttribute.DisplayName"/> 
        /// or <see cref="DescriptionAttribute.Description"/> if this attributes 
        /// are used for the current field, otherwise, an string representation of 
        /// <paramref name="value"/>.
        /// </returns>
        private static string GetDisplayName(Enum value)
        {
            var enumField = value
                .GetType()
                .GetField(value.ToString());

            var displayNameAttribute = enumField
                .GetCustomAttributes<DisplayNameAttribute>(false)
                .FirstOrDefault();

            var descriptionAttribute = enumField
                .GetCustomAttributes<DescriptionAttribute>(false)
                .FirstOrDefault();

            if (displayNameAttribute != null)
            {
                return displayNameAttribute.DisplayName;
            }
            else if (descriptionAttribute != null)
            {
                return descriptionAttribute.Description;
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
