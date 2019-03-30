using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Data.Tests
{
	public sealed class SqliteProviderMethods : DbProviderMethods
	{
		public override async Task OpenConnectionAsync(IDbConnection connection, CancellationToken cancellationToken) => connection.Open();
	}
}
