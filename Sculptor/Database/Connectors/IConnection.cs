using Sculptor.Query.Grammars;

namespace Sculptor.Database.Connectors
{
    public interface IConnection : IConnectionSync, IConnectionAsync
    {
        /// <summary>
        /// The query grammar implementation.
        /// </summary>
        public Grammar Grammar { get; }
    }
}
