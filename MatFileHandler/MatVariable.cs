// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <inheritdoc />
    internal class MatVariable : IVariable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatVariable"/> class.
        /// </summary>
        /// <param name="value">The value of the variable.</param>
        /// <param name="name">Variable name.</param>
        /// <param name="isGlobal">Indicates if the variable is global.</param>
        public MatVariable(IArray value, string name, bool isGlobal)
        {
            Value = value;
            Name = name;
            IsGlobal = isGlobal;
        }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public IArray Value { get; }

        /// <inheritdoc />
        public bool IsGlobal { get; }
    }
}