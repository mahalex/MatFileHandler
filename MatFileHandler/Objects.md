# MATLAB objects

This document describes the format that MATLAB uses to store (new-style)
objects in .mat files. As far as I know, this format is not documented
anywhere. The [official MATLAB documentation](https://www.mathworks.com/help/pdf_doc/matlab/matfile_format.pdf)
(as of version 2018) does not even mention the corresponding MX class.

We assume the reader's familiarity with the document linked above.
It describes how the "normal" MATLAB data types (matrices, cell arrays,
structure arrays, etc.) are stored in .mat files. Objects (instances
of user-defined classes, or of build-in MATLAB types such as tables, etc.)
are stored in a slightly different way. If you save a variable into a .mat
file, it appears alongside the "normal" variables, but its data only contains
a "reference" that points to the actual object contents.
The actual data are stored in a so called "subsystem data" in a separate
portion of a .mat file.
Recall that .mat file header contains (among some text information and
a couple of flags) a "subsystem data offset field".
As described in the official documentation, it appears right after
the first 124 bytes of a .mat file (which contain a "header text field")
and occupies 16 bit, i.e., is a short (16-bit) integer.
This integer in fact tells you the offset in the .mat file
(starting from the very beginning of the file) where the subsystem data
starts. Typically it appears after all the "normal" variables.

So, we have to describe two things: how the subsystem data is organized and how
the references to it are stored.

Let's start with the second. Every MATLAB matrix is stored in a data element
of type `miMATRIX = 14`. The type of its elements is determined by
a "MATLAB array type" field (see table on page 1-16):
`mxCELL_CLASS = 1` means cell array, `mxSTRUCT_CLASS = 2` means structure
array, and so on, up to `mxUINT64_CLASS = 15` (unsigned 64-bit integer).
The official table ends here, but in fact there are (at least) to more
classes: `mxFUNCTION_CLASS = 16` for function handles,
and `mxOPAQUE_CLASS = 17` for "opaque objects", and that is what we are
interested in.

## Opaque data element format

After the array flags element that has array class field set
to `mxOPAQUE_CLASS = 17`, come four more data elements:
* Array name. This is the variable name in case of variables, and empty
in other cases (same as for "normal" data types).
* Type system name. This is stored in the same format as array name
(class `miINT8`), and contains four characters: `MCOS`. It stands for
"MATLAB Class Object System".
* Class name. Again, the same format for storing strings. This element
contains name of the class that this object belongs to.
* Data reference. This element is a matrix of class `mxUINT32`
and has size *n*×1, where *n* is at least 6.
The 32-bit unsigned integers inside contain information that you need to find
the actual object's data inside subsystem data:
    *  First number is 3707764736 (0xdd000000). This is the "magic" number
    that is used to identify MATLAB object data. We'll see it used
    (quite unexpectedly) below.
    * Second number (let's call it *d*) is the number of dimensions
    in the object array. Typically it's 2 (so, we usually deal with
    2-dimensional object arrays).
    * Next *d* numbers contain the size of each of all the dimensions.
    So, for a single object we have 2 numbers: 1 and 1.
    * Then, for each of the object in the object array, comes
    **objectId**. Naturally, the number of the objects is just a
    product of all the dimension sizes.
    * Finally, the last number is **classId**.

So, the main information you can learn from this element is two numbers:
*objectId* is the index of object in the list of all the objects
stored in the file (numbering starts with 1), while *classId*
is the index of our object's class in the list of all the classes
that are present in the file (again, numbering starts with 1).
For example, if we only store one object in out file,
both of these numbers will be equal to 1.

These two numbers is all you need to find your object's data
in the subsystem data, so let's proceed to that.


## Subsystem data format

### General structure

First, comes a small header.

* `0x00`, `0x01` (2 bytes)
* `0x4d49` (`MI`, 16-bit integer): endian indicator (see page 1-7).

After that come variables that are stored in the same way as in the
"normal" portion of the file.

I think that there should be only two variables, and the second one does
not contain any useful information.
So we'll focus on the first variables, which should be a structure array
with a single field `MCOS` (MATLAB Class Object System, again).
The value of this field is an array of class `mxOPAQUE_CLASS` again,
so it has the structure described above. It is different, though:
its class name is now `FileWrapper__`, and its data is not a matrix of class
`mxUINT32` with object number and class number, but a cell array.
This cell array is what we are interested in.
Its first element is a byte array (matrix of class `mxUINT8`)
that ties together all the references to object data, i.e.,
object numbers and class numbers. Let's call this byte array
**object metadata**.
The second element doesn't really contain anything, as far as I can say:
it is an empty cell array.
The following elements contain the fields of the objects that are stored
in the file; the object metadata tells you which fields belong to which
objects. We shall refer to these elements as **field contents**.
Finally, the last element is of unknown purpose: in all the examples
I've seen, it contains a column cell array with several empty
structure arrays.

### Object metadata

Object metadata is a single byte array with several regions inside:

* **Table of contents**: a list of 16-bit integers:
    * first is of unknown purpose;
    * second is **number of field/class names** (see below);
    * the rest are treated as offsets (in bytes) to the regions inside
    object metdata. The offsets are listed in ascending order, and the list
    is 0-terminated. Note that every region, including table of contents,
    is aligned on 8-byte boundaries, so there might be extra 4 bytes of padding
    after the terminating 0.
    In all the examples I've seen, this list contains 6 offsets, and the last
    one actually points to the end of object metadata (its value is the same
    as the metadata length). Thus we shall assume that the object metadata has
    6 regions: the last one is probably empty.
    * Then come **field/class names**: a list of zero-terminated ASCII
    strings. The number of the strings was mentioned above; again, the data is
    aligned on 8-byte boundaries.
    This region gives you names for all the classes that are stored in the file,
    and all their fields. The order is arbitrary, and the information that
    connects the field to classes (and tells you which of the names are
    actually names of classes) comes later.

* Region 1 contains several blocks, four 32-bit integers in each block.
First block contains integers (0, 0, 0, 0), and the rest of the blocks
contain (0, *n*, 0, 0), where *n* is an index in the **field/class names**
list that points to a class name. Thus, the number of this 128-byte blocks
is the number of different classes stored in the file, plus one zero-filled
block in the beginning.
This allows you to get class names from region 1; the rest are field names.
Moreover, the order of the class names in this region corresponds to
the "classId" numbers, which allows you to connect class names to
classIds.

* Region 2 seems to be empty.

* Region 3 connects objects to classes. It contains a sequence of blocks,
six 32-bit integers in each block. First block is filled with zeros,
and the rest contain (*classId*, 0, 0, 0, *objectId*, *objectDependencyId*).
Each block describes one object, and tells you which class this object
has. The meaning of *objectDependencyId* is not exactly clear
(but see below for some hints). Note that *classId* and *objectId* here
are the same that you may find in the references stored in
"opaque" objects.

* Region 4 connects objects to their fields. This is a list
of 32-bit integers. The first two integers (i.e., first 8 bytes) are zero.
Then comes a sequence of blocks, one for each object, in the ascending order
of objectIds. Each block starts with one 32-bit integer containing the number
of sub-blocks to follow; a sub-block consists of three 32-bit integers.
Note that each block is aligned on 64-bit boundaries, so there might
be padding involved.
As we said, a block corresponds to an object. Its sub-blocks
correspond to the fields in this object, and contain the following three
numbers: (*fieldId*, 1, *fieldContentsId*).
Here *fieldId* allows you to get field name from the *field/class names*
list of Region 1, and *fieldContentsId* points to the location of the value
of this field in the current object. As we said earlier, the field contents
are stored in the same cell array where object metadata is the first element.
The fieldContentsId numbering, again, starts with 1, and does not include
the first two cells (the object metadata cell and the unknown stuff cell).

This all might seem a little complicated, but actually is rather simple.
We have four lists: classes, objects, fields, and field contents.
Region 1 connects classes to their names (stored in **field/class names**
list, which allows you to get names for the field, as well).
Region 3 connects objects to classes.
Region 4 connects objects to fields, and fields to field contents.
The meaning of *objectDependencyId* values in Region 3 is not totally clear,
but it seems to suggest an alternative numbering of objects, which takes
into account their dependencies: if one object contains another one as a
field, then the second one should be constructed before the first.
Thus, the *objectDependencyId* of the second object should be lower
that that of the first object.

Actually, this situation (objects as fields of other objects) contains
a caveat: if the field that is stored in the *field contents* cells is
itself an object, it is not wrapped in an "opaque object" class data type,
in contrast to the objects stored in the "normal" section of a .mat file.
Instead, a *field contents* cell just contains the `mxUINT32` matrix
that would be the "data reference" part of an "opaque object".
This can lead to hilarious results: if you have an object that has
a field containing a column matrix of type `int32` whose first element
is the magic number 0xdd000000, this object can be saved into a .mat file
using MATLAB, but cannot be read back, since MATLAB treats it as an object
reference and tries to extract its value based on the other values of the
matrix, which can easily lead to crashes or even infinite loops.

