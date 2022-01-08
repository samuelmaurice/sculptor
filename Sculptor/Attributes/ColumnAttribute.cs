using System;

namespace Sculptor.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// The custom column name for the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new instance of the attribute.
        /// </summary>
        /// <param name="name">The custom column name for the property.</param>
        public ColumnAttribute(string name)
        {
            Name = name;
        }
    }
}
