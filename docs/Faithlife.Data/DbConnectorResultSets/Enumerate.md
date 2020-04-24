# DbConnectorResultSets.Enumerate&lt;T&gt; method (1 of 2)

Reads a result set, reading one record at a time and converting it to the specified type.

```csharp
public IEnumerable<T> Enumerate<T>()
```

## See Also

* method [EnumerateAsync&lt;T&gt;](EnumerateAsync.md)
* class [DbConnectorResultSets](../DbConnectorResultSets.md)
* namespace [Faithlife.Data](../../Faithlife.Data.md)

---

# DbConnectorResultSets.Enumerate&lt;T&gt; method (2 of 2)

Reads a result set, reading one record at a time and converting it to the specified type with the specified delegate.

```csharp
public IEnumerable<T> Enumerate<T>(Func<IDataRecord, T> map)
```

## See Also

* method [EnumerateAsync&lt;T&gt;](EnumerateAsync.md)
* class [DbConnectorResultSets](../DbConnectorResultSets.md)
* namespace [Faithlife.Data](../../Faithlife.Data.md)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Data.dll -->