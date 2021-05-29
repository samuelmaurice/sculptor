using System;

namespace Sculptor.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConnectionAttribute : Attribute
    {
        /// <summary>
        /// The connection name for the model.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Create a new instance of the attribute.
        /// </summary>
        /// <param name="name">The connection name for the model.</param>
        public ConnectionAttribute(string name)
        {
            Name = name;
        }
    }
}
