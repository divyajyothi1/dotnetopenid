using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetOpenId.Session
{
    public interface ISessionState
    {
        /// <summary>
        /// Gets or sets a session value by numerical index.
        /// </summary>
        /// <param name="index">The numerical index of the session value.</param>
        /// <returns>The session-state value stored at the specified index.</returns>
        object this[int index] { get; set; }
        /// <summary>
        /// Gets or sets a session value by name.
        /// </summary>
        /// <param name="name">The key name of the session value.</param>
        /// <returns>The session-state value with the specified name.</returns>
        object this[string name] { get; set; }
        /// <summary>
        /// Adds a new item to the session-state collection.
        /// </summary>
        /// <param name="name">The name of the item to add to the session-state collection.</param>
        /// <param name="value">The value of the item to add to the session-state collection.</param>
        void Add(string name, object value);
        /// <summary>
        /// Removes all keys and values from the session-state collection.
        /// </summary>
        void Clear();
        /// <summary>
        /// Deletes an item from the session-state collection.
        /// </summary>
        /// <param name="name">The name of the item to delete from the session-state collection.</param>
        void Remove(string name);
        /// <summary>
        /// Gets the number of items in the session-state collection.
        /// </summary>
        /// <value>The number of items in the collection.</value>
        int Count { get; }
    }
}
