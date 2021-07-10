// Copyright 2017-2018 Alexander Luzgarev

using System.Collections.Generic;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// .mat file.
    /// </summary>
    internal class MatFile : IMatFile
    {
        private readonly Dictionary<string, IVariable> _variables;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFile"/> class.
        /// </summary>
        /// <param name="variables">List of variables.</param>
        public MatFile(IEnumerable<IVariable> variables)
        {
            _variables = new Dictionary<string, IVariable>();
            foreach (var variable in variables)
            {
                _variables[variable.Name] = variable;
            }
        }

        /// <inheritdoc />
        public IVariable[] Variables => _variables.Values.ToArray();

        /// <inheritdoc />
        public IVariable this[string variableName]
        {
            get => _variables[variableName];
            set => _variables[variableName] = value;
        }

        /// <inheritdoc />
        public bool TryGetVariable(string name, out IVariable value)
        {
            return _variables.TryGetValue(name, out value);
        }
    }
}