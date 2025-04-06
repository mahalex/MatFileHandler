# MatFileHandler

[![NuGet Version](https://img.shields.io/nuget/vpre/MatFileHandler?color=green)](https://www.nuget.org/packages/MatFileHandler/absoluteLatest)

MatFileHandler is a .NET library for reading and writing MATLAB .mat files
(of so-called "Level 5"). MatFileHandler supports numerical arrays,
logical arrays, sparse arrays, char arrays, cell arrays and structure arrays.
Moreover, MatFileHandler is able to read the contents of MATLAB objects,
and is currently probably the only third-party library that can do that.
Since the format for storing MATLAB objects seems to be obscure and
undocumented, support for them is preliminary and probably contains bugs.
You can find (partial) technical description of MATLAB object data format
[here](MatFileHandler/Objects.md).

This document briefly describes how to perform simple operations with .mat files
using MatFileHandler.

If you have questions and/or ideas, you can [file a new issue](https://github.com/mahalex/MatFileHandler/issues/new)
or contact me directly at <mahalex@gmail.com>.

## Changelog

* Version `1.3.0` adds (read-only) support for Matlab objects, as well as an
interface to read tables.

* Version `1.2.0` makes data compression when writing files optional.

* Version `1.1.0` adds multi-targeting: the project now targets .NET Framework
4.6.1 as well as .NET Standard 2.0.

## Reading a .mat file

```csharp
using MatFileHandler;
IMatFile matFile;
using (var fileStream = new System.IO.FileStream("example.mat", System.IO.FileMode.Open)) {
    var reader = new MatFileReader(fileStream);
    matFile = reader.Read();
}
```

After that, you can access the variables inside using the indexer
```csharp
IVariable variable = matFile["array"];
```
or iterating over all the variables:
```csharp
foreach (IVariable variable in matFile.Variables) {
    // Do stuff
}
```
(all of the interfaces and classes described in this text are in the
`MatFileHandler` namespace).

Each `IVariable` has a name, a value, and a flag indicating if it's a “global”
variable:
```csharp
public interface IVariable
{
    string Name { get; set; }
    IArray Value { get; }
    bool IsGlobal { get; }
}
```

The interesting part here is the `IArray` interface. This is a base interface,
which is extended by other interfaces that provide access to more specific
MATLAB arrays (numerical, cell, structure, char, etc.).
We can't do much with `IArray` itself: check for emptiness, get its dimensions
and total number of elements in it, or try to convert it to an array of double 
(or complex) numbers:
```csharp
public interface IArray
{
    bool IsEmpty { get; }
    int[] Dimensions { get; }
    int Count { get; }
    double[] ConvertToDoubleArray();
    System.Numerics.Complex[] ConvertToComplexArray();
}
```

Note that `Dimensions` is a list, since all arrays in MATLAB are (at least
potentially) multi-dimensional. However, `ConvertToDoubleArray()` and
`ConvertToComplexArray()` return flat arrays, arranging all multi-dimensional
data in columns (MATLAB-style). This functions return `null` if conversion
failed (for example, if you tried to apply it to a structure array, or cell
array).

### Numerical and logical arrays

The simplest type of array is a numerical array, which implements the
`IArrayOf<T>` interface, where `T` is a numerical type, i. e., one of `Int8`,
`UInt8`, `Int16`, `UInt16`, `Int32`, `UInt32`, `Int64`, `UInt64`, `Single`,
`Double`. Arrays can contain complex values, which are just pairs of
ordinary numbers. These pairs of `Double`s are represented by
`System.Numerics.Complex`, and pairs of other numerical types are
represented by a simple `ComplexOf<T>` struct, which has two properties:
```csharp
public struct ComplexOf<T> : IEquatable<ComplexOf<T>>
    where T : struct
{
    public T Real { get; }
    public T Imaginary { get; }
    // Some other stuff
}
```

All of this means that you can also have an `IArrayOf<T>` for `T` being
`ComplexOf<Int8>`, `ComplexOf<UInt8>`, `ComplexOf<Int16>`, `ComplexOf<UInt16>`,
`ComplexOf<Int32>`, `ComplexOf<UInt32>`, `ComplexOf<Int64>`,
`ComplexOf<UInt64>`, `ComplexOf<Single>`, and, of course, `Complex` (note that
we don't use `ComplexOf<Double>`). Finally, you can access a logical array as
`IArrayOf<Boolean>`.

The `IArrayOf<T>` interface allows you to refer to a specific element by using a
(multi-dimensional) indexer, or get all data at once as a flat array 
(multidimensional arrays get converted to flat using MATLAB conventions).
Indexes start with 0 (note that in MATLAB they start with 1, so there is a
shift in notation).
```csharp
public interface IArrayOf<T> : IArray
{
    T[] Data { get; }
    T this[params int[] list] { get; set; }
}
```
You can use a one-dimensional indexer or a multi-dimensional one, which is
consistent with MATLAB notation. For example, a 2×3 array named `a` has elements
`a[0, 0]`, `a[1, 0]` (first column), `a[0, 1]`, `a[1, 1]` (second column), `a[0,
2]`, `a[1, 2]` (third column), which can also be accessed as `a[0]`, `a[1]`, `a
[2]`, `a[3]`, `a[4]`, and `a[5]`, respectively.

### Cell arrays

Cell array is just an array of arrays, so `ICellArray` implements
`IArrayOf<IArray>`, and adds nothing to it. This means that you can refer to
specific cells in a cell array by using the indexer, or by inspecting the
`Data` array described in the previous section.

### Char arrays

Char arrays implement `IArrayOf<char>`, so you can refer to individual chars in
it via an indexer. Often a char array is used to carry a string, so there is
a property for that:
```csharp
public interface ICharArray : IArrayOf<char>
{
    string String { get; }
}
```
This can be slightly weird for multi-dimensional arrays: the characters are
stuffed into this string by columns (the same way the numerical array elements
are flattened into a one-dimensional array). Moreover, each character array you
read from a file actually implements either `IArrayOf<UInt8>`, or
`IArrayOf<UInt16>`, depending on whether it was stored as a UTF-8 or UTF-16
encoded string. Characters arrays produced by MatFileHandler are always encoded
as UTF-16.

### Structure arrays

Structure arrays have elements that are indexed not only by their positions in
the array, but also by structure fields. For example, a 1×2 structure array `s`
with fields `x` and `y` has four elements: `s(1).x`, `s(1).y`, `s(2).x`, `s
(2).y` (in MATLAB notation). This means that if you only specify the numerical
indices, you get a dictionary that maps `string` to `IArray`; in order to reach
a specific element, you need to provide both the indices and the field name:
```csharp
public interface IStructureArray : IArrayOf<IReadOnlyDictionary<string, IArray>>
{
    IEnumerable<string> FieldNames { get; }
    IArray this[string field, params int[] list] { get; set; }
}
```
Here `FieldNames` gives you a list of all fields in the structure.

### Sparse arrays

Sparse array is like a numerical array, but not all of the values in it have to
be specified; the rest are assumed to be 0.
```csharp
public interface ISparseArrayOf<T> : IArrayOf<T>
  where T : struct
{
    new IReadOnlyDictionary<(int, int), T> Data { get; }
}
```
Since `ISparseArrayOf<T>` implements `IArrayOf<T>`, you still can access all the
elements in a sparse array (you'll get 0 when the element is not present).
Alternatively, you can get a dictionary of all (possibly) non-zero elements.
MATLAB only supports double, complex, and logical sparse arrays, so `T` here
can be `Double`, `Complex` or `Boolean` (which, of course, uses `false` as
the default value).

### Object arrays

Matlab objects are similar to structures in that they have some data associated
with fields. As an example, consider a simple `Point` class defined in Matlab as
```matlab
classdef Point
    properties
        x
        y
    end
end
```
We omit any methods (and constructors) such a class might have, because they are
not stored when you save an object of a class into a `.mat` file.

Imagine that you have a `1x2 Point` object array `p` (an array of two points)
where the first point has `x=3`, `y=5`, and the second point has `x=-2`, `y=6`.
You can load a mat file containing the variable `p` as usual (using
`MatFileReader`) and access the data using the following interface:
```csharp
public interface IMatObject : IArrayOf<IReadOnlyDictionary<string, IArray>>
{
    string ClassName { get; }
    IEnumerable<string> FieldNames { get; }
    IArray this[string field, params int[] list] { get; set; }
}
```
As you can see, the interface is very similar to `IStructureArray`. The only
addition is the `ClassName` string, which returns the name of object's class 
(in our case that would be `Point`). Otherwise, the idea is the same.
In our example, if we load the `.mat` file containing the variable `p` into a
variable named `matFile`, we could then use
```csharp
var matObject = matFile["p"].Value as IMatObject
```
and access the values: `matObject["x", 0] = 3`, `matObject["y", 1] = 6`,
`matObject[1]["x"] = -2`, and so on.

### Tables

Tables in Matlab are just objects of type `table`, so you could use the
interface `IMatObject` described above and get access to all the data in a table
stored in a `.mat` file. However, this is not very convenient, since all the
actual data in a table is stored in one field called `data`, and the
properties are scattered across other fields.

This is why `MatFileHandler` provides a simple wrapper class to work with
tables:
```
public class TableAdapter
{
	public TableAdapter(IArray array);
	public string Description { get; }
	public int NumberOfRows { get; }
	public int NumberOfVariables { get; }
	public string[] RowNames { get; }
	public string[] VariableNames { get; }
	public IArray this[string variableName] { get; }
}
```
The constructor creates a `TableAdapter` from an object that you read from a
file. You can access table's description field, query number and names of the
rows and variables of the table, and access all data associated with a single
variable. This accessor returns an array (or a cell array) that has the same
number of rows as table's `NumberOfRows`, and contains values for a given
variable from all the rows (so this is equivalent to Matlab's `t.variable` for
a table `t` having a variable named `variable`).

## Writing a .mat file

After reading a file into `IMatFile matFile`, you can alter some values using
the described interfaces, and write the result to a new file:
```csharp
using (var fileStream = new System.IO.FileStream("output.mat", System.IO.FileMode.Create)) {
    var writer = new MatFileWriter(fileStream);
    writer.Write(matFile);
}
```
By default, all variables are written in a compressed format; you can turn that
off by using another constructor for `MatFileWriter`:
```csharp
var writer = new MatFileWriter(fileStream, new MatFileWriterOptions { UseCompression = CompressionUsage.Never });
```

Another option is to create a file from scratch. You can do it with
`DataBuilder` class:

```csharp
public class DataBuilder
{
    public IArrayOf<T> NewArray<T>(params int[] dimensions)
        where T : struct;
    public IArrayOf<T> NewArray<T>(T[] data, params int[] dimensions)
        where T : struct;
    public ICellArray NewCellArray(params int[] dimensions);
    public IStructureArray NewStructureArray(IEnumerable<string> fields, params int[] dimensions);
    public ICharArray NewCharArray(string contents);
    public ICharArray NewCharArray(string contents, params int[] dimensions);
    public IArray NewEmpty();
    public ISparseArrayOf<T> NewSparseArray<T>(params int[] dimensions)
      where T : struct;
    public IVariable NewVariable(string name, IArray value, bool isGlobal = false);
    public IMatFile NewFile(IEnumerable<IVariable> variables);
}
```
Numerical/logical arrays can be created with `NewArray<T>()` using the provided
data; char arrays can be created with `NewCharArray()` using a string. All
other types of arrays are created empty. Then you can wrap an array into a
variable with `NewVariable()`, and put a bunch of variables into a file using
`NewFile()`. The resulting file can be written to a stream using
`MatFileWriter`, as shown above.
