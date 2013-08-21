SortExpression
====

The sort expression utility extends the Linq OrderBy methods to accept a SQL-formatted string as an argument. This is useful when connecting certain ASP.NET controls to an underlying Linq-based data source like an Entity Framework model.

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
