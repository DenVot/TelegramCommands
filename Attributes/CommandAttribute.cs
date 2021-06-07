using System;

namespace Telegram.Bot.Attributes
{
    /// <summary>
    /// Attribute that indicates command-void.
    /// </summary>
    public class CommandAttribute : Attribute
    {
        /// <summary>
        /// Name of command.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of command.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initilize new <see cref="CommandAttribute"/>
        /// </summary>
        /// <param name="name">Name of command.</param>
        /// <param name="description">Description of command.</param>
        public CommandAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }        
    }
}
