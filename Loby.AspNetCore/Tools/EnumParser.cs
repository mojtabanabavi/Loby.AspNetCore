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
        /// Returns a new instance of <see cref="SelectList"/> that is created 
        /// based on <typeparamref name="T"/> which the value is the number assigned 
        /// to each field and text is <see cref="DescriptionAttribute.Description"/>, if 
        /// it's defined; otherwise the name of field.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// T is not an enum.
        /// </exception>
        public static SelectList ToSelectList<T>()
        {
            return ToSelectList<T>(null);
        }

        /// <summary>
        /// Creates a select list from an enum.
        /// </summary>
        /// <typeparam name="T">
        /// A type representing an enum.
        /// </typeparam>
        /// <param name="selectedValue">
        /// An object representing the selected value.
        /// </param>
        /// <returns>
        /// Returns a new instance of <see cref="SelectList"/> that is created 
        /// based on <typeparamref name="T"/> which the value is the number assigned 
        /// to each field and text is <see cref="DescriptionAttribute.Description"/>, if 
        /// it's defined; otherwise the name of field.
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
                    Text = GetDescription(e),
                    Value = (Convert.ToInt32(e)).ToString(),
                });

            return new SelectList(selectListItems, "Value", "Text", selectedValue);
        }

        /// <summary>
        /// Creates a dictionary from an enum.
        /// </summary>
        /// <typeparam name="T">
        /// A type representing an enum.
        /// </typeparam>
        /// <returns>
        /// Returns a new instance of <see cref="Dictionary{int, string}"/> that is 
        /// created based on <typeparamref name="T"/> which the key is the number assigned 
        /// to each field and value is <see cref="DescriptionAttribute.Description"/>, if 
        /// it's defined; otherwise the name of field.
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
                .ToDictionary(e => Convert.ToInt32(e), e => GetDescription(e));
        }

        /// <summary>
        /// Returns the description value of <see cref="DescriptionAttribute"/> 
        /// that is used on current object.
        /// </summary>
        /// <param name="value">
        /// An enum field.
        /// </param>
        /// <returns>
        /// The value of <see cref="DescriptionAttribute.Description"/> that is 
        /// used on current field if it's defined; otherwise the name of field.
        /// </returns>
        private static string GetDescription(Enum value)
        {
            var attribute = value
                .GetType()
                .GetField(value.ToString())
                .GetCustomAttributes<DescriptionAttribute>(false)
                .FirstOrDefault();

            return attribute != null ? attribute.Description : value.ToString();
        }
    }
}
