using System;
using System.Linq;

namespace MatFileHandler
{
    public class TableAdapter
    {
        private readonly IMatObject matObject;

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
            NumberOfRows = (int) matObject["nrows"].ConvertToDoubleArray()[0];
            var rowNamesArrays = matObject["rownames"] as ICellArray;
            RowNames = Enumerable
                .Range(0, rowNamesArrays.Count)
                .Select(i => (cellArray[i] as ICharArray).String)
                .ToArray();
        }

        public int NumberOfRows { get; }

        public string[] RowNames { get; }

        public int NumberOfVariables { get; }

        public string[] VariableNames { get; }

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
        
        public string Description { get; }
    }
}
