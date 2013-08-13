Core
====

Core is a small .NET library that intends to extend the .NET Framework with some methods and utilities I find missing. The library is in an early stage of development, so if you want to chip in, go right ahead!

For now I will just put the minimal documentation of the few available components here, but a future goal is to create a separate documantation section.





SortExpression
====

The sort expression utility extends the Linq OrderBy methods to accept a SQL-formatted string as an argument. This is useful when connecting certain controls to an underlying Linq-based data source like an Entity Framework model.

In its simplest form the sytax is as follows:

```
db.MyTable.OrderBy("FirstName DESC");
```

Miscelanious Functions
====

__String.Replace(string, string, StringComparison)__

Overload for the String.Replace method accepting a StringComparison value.

__String.Remove(String, [StringComparison])__
__String.Remove([StringComparison], params String[])__

Overload for String.Remove that removes the provided string(s) from the source string.
