using System;

namespace Sculptor.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// The table associated with the model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new instance of the attribute.
        /// </summary>
        /// <param name="name">The table associated with the model.</param>
        public TableAttribute(string name)
        {
            Name = name;
        }
    }
}
