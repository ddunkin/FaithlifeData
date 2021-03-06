# DbConnector class

Encapsulates a database connection and any current transaction.

```csharp
public abstract class DbConnector : IAsyncDisposable, IDisposable
```

## Public Members

| name | description |
| --- | --- |
| static [Create](DbConnector/Create.md)(…) | Creates a new DbConnector. |
| abstract [Connection](DbConnector/Connection.md) { get; } | The database connection. |
| abstract [Transaction](DbConnector/Transaction.md) { get; } | The current transaction, if any. |
| abstract [BeginTransaction](DbConnector/BeginTransaction.md)() | Begins a transaction. |
| abstract [BeginTransaction](DbConnector/BeginTransaction.md)(…) | Begins a transaction. |
| abstract [BeginTransactionAsync](DbConnector/BeginTransactionAsync.md)(…) | Begins a transaction. (2 methods) |
| [Command](DbConnector/Command.md)(…) | Creates a new command. (3 methods) |
| abstract [CommitTransaction](DbConnector/CommitTransaction.md)() | Commits the current transaction. |
| abstract [CommitTransactionAsync](DbConnector/CommitTransactionAsync.md)(…) | Commits the current transaction. |
| abstract [Dispose](DbConnector/Dispose.md)() | Disposes the connector. |
| abstract [DisposeAsync](DbConnector/DisposeAsync.md)() | Disposes the connector. |
| abstract [GetConnectionAsync](DbConnector/GetConnectionAsync.md)(…) | Returns the database connection. |
| abstract [OpenConnection](DbConnector/OpenConnection.md)() | Opens the connection. |
| abstract [OpenConnectionAsync](DbConnector/OpenConnectionAsync.md)(…) | Opens the connection. |
| abstract [RollbackTransaction](DbConnector/RollbackTransaction.md)() | Rolls back the current transaction. |
| abstract [RollbackTransactionAsync](DbConnector/RollbackTransactionAsync.md)(…) | Rolls back the current transaction. |

## Protected Members

| name | description |
| --- | --- |
| [DbConnector](DbConnector/DbConnector.md)() | The default constructor. |
| virtual [CommandCache](DbConnector/CommandCache.md) { get; } | Gets the command cache, if supported. |
| abstract [ProviderMethods](DbConnector/ProviderMethods.md) { get; } | Special methods provided by the database provider. |

## See Also

* namespace [Faithlife.Data](../Faithlife.Data.md)
* [DbConnector.cs](https://github.com/Faithlife/FaithlifeData/tree/master/src/Faithlife.Data/DbConnector.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.Data.dll -->
