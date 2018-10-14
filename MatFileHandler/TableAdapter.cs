// Copyright 2017-2018 Alexander Luzgarev

using System;
using System.Linq;

namespace MatFileHandler
{
    /// <summary>
    /// A better interface for using table objects.
    /// </summary>
    public class TableAdapter
    {
        private readonly IMatObject matObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableAdapter"/> class.
        /// </summary>
        /// <param name="array">Source table object.</param>
        public TableAdapter(IArray array)
        {
            matObject = array as IMatObject;
            if (matObject?.ClassName != "table")
            {
                throw new ArgumentException("The object provided is not a table.");
            }

            var cellArray = matObject["varnames"] as ICellArray;
            VariableNames = Enumerable
                .Range(0, cellArray.Count)
                .Select(i => (cellArray[i] as ICharArray).String)
                .ToArray();
            NumberOfVariables = VariableNames.Length;
            var props = matObject["props"] as IStructureArray;
            Description = (props["Description"] as ICharArray).String;
            NumberOfRows = (int)matObject["nrows"].ConvertToDoubleArray()[0];
            var rowNamesArrays = matObject["rownames"] as ICellArray;
            RowNames = Enumerable
                .Range(0, rowNamesArrays.Count)
                .Select(i => (cellArray[i] as ICharArray).String)
                .ToArray();
        }

        /// <summary>
        /// Gets table description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the number of rows in the table.
        /// </summary>
        public int NumberOfRows { get; }

        /// <summary>
        /// Gets the number of variables in the table.
        /// </summary>
        public int NumberOfVariables { get; }

        /// <summary>
        /// Gets row names.
        /// </summary>
        public string[] RowNames { get; }

        /// <summary>
        /// Gets variable names.
        /// </summary>
        public string[] VariableNames { get; }

        /// <summary>
        /// Gets all the data for a given variable
        /// </summary>
        /// <param name="variableName">Variable name.</param>
        /// <returns>All data associated with the variable.</returns>
        public IArray this[string variableName]
        {
            get
            {
                var maybeIndex = Enumerable.Range(0, VariableNames.Length)
                    .Where(i => VariableNames[i] == variableName)
                    .Select(i => (int?)i)
                    .FirstOrDefault();
                if (!(maybeIndex is int index))
                {
                    throw new IndexOutOfRangeException($"Variable '{variableName}' not found.");
                }

                var data = matObject["data"] as ICellArray;
                return data[index];
            }
        }
    }
}