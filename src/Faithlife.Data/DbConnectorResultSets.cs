using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Faithlife.Data
{
	/// <summary>
	/// Encapsulates multiple result sets.
	/// </summary>
	public sealed class DbConnectorResultSets : IDisposable
	{
		/// <summary>
		/// Reads a result set, converting each record to the specified type.
		/// </summary>
		public IReadOnlyList<T> Read<T>() =>
			DoRead<T>(null);

		/// <summary>
		/// Reads a result set, converting each record to the specified type with the specified delegate.
		/// </summary>
		public IReadOnlyList<T> Read<T>(Func<IDataRecord, T> read) =>
			DoRead(read ?? throw new ArgumentNullException(nameof(read)));

		/// <summary>
		/// Reads a result set, converting each record to the specified type.
		/// </summary>
		public ValueTask<IReadOnlyList<T>> ReadAsync<T>(CancellationToken cancellationToken = default) =>
			DoReadAsync<T>(null, cancellationToken);

		/// <summary>
		/// Reads a result set, converting each record to the specified type with the specified delegate.
		/// </summary>
		public ValueTask<IReadOnlyList<T>> ReadAsync<T>(Func<IDataRecord, T> read, CancellationToken cancellationToken = default) =>
			DoReadAsync(read ?? throw new ArgumentNullException(nameof(read)), cancellationToken);

		/// <summary>
		/// Reads a result set, reading one record at a time and converting it to the specified type.
		/// </summary>
		public IEnumerable<T> Enumerate<T>() =>
			DoEnumerate<T>(null);

		/// <summary>
		/// Reads a result set, reading one record at a time and converting it to the specified type with the specified delegate.
		/// </summary>
		public IEnumerable<T> Enumerate<T>(Func<IDataRecord, T> read) =>
			DoEnumerate(read ?? throw new ArgumentNullException(nameof(read)));

		/// <summary>
		/// Reads a result set, reading one record at a time and converting it to the specified type.
		/// </summary>
		public IAsyncEnumerable<T> EnumerateAsync<T>(CancellationToken cancellationToken = default) =>
			DoEnumerateAsync<T>(null, cancellationToken);

		/// <summary>
		/// Reads a result set, reading one record at a time and converting it to the specified type with the specified delegate.
		/// </summary>
		public IAsyncEnumerable<T> EnumerateAsync<T>(Func<IDataRecord, T> read, CancellationToken cancellationToken = default) =>
			DoEnumerateAsync(read ?? throw new ArgumentNullException(nameof(read)), cancellationToken);

		/// <summary>
		/// Disposes resources used by the result sets.
		/// </summary>
		public void Dispose()
		{
			m_reader.Dispose();
			m_command.Dispose();
		}

		internal DbConnectorResultSets(IDbCommand command, IDataReader reader, DbProviderMethods methods)
		{
			m_command = command;
			m_reader = reader;
			m_methods = methods;
		}

		private IReadOnlyList<T> DoRead<T>(Func<IDataRecord, T>? read)
		{
			if (m_next && !m_reader.NextResult())
				throw new InvalidOperationException("No more results.");
			m_next = true;

			var list = new List<T>();
			while (m_reader.Read())
				list.Add(read != null ? read(m_reader) : m_reader.Get<T>());
			return list;
		}

		private async ValueTask<IReadOnlyList<T>> DoReadAsync<T>(Func<IDataRecord, T>? read, CancellationToken cancellationToken)
		{
			if (m_next && !await m_methods.NextResultAsync(m_reader, cancellationToken).ConfigureAwait(false))
				throw new InvalidOperationException("No more results.");
			m_next = true;

			var list = new List<T>();
			while (await m_methods.ReadAsync(m_reader, cancellationToken).ConfigureAwait(false))
				list.Add(read != null ? read(m_reader) : m_reader.Get<T>());
			return list;
		}

		private IEnumerable<T> DoEnumerate<T>(Func<IDataRecord, T>? read)
		{
			if (m_next && !m_reader.NextResult())
				throw new InvalidOperationException("No more results.");
			m_next = true;

			while (m_reader.Read())
				yield return read != null ? read(m_reader) : m_reader.Get<T>();
		}

		private async IAsyncEnumerable<T> DoEnumerateAsync<T>(Func<IDataRecord, T>? read, [EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (m_next && !await m_methods.NextResultAsync(m_reader, cancellationToken).ConfigureAwait(false))
				throw new InvalidOperationException("No more results.");
			m_next = true;

			while (m_reader.Read())
				yield return read != null ? read(m_reader) : m_reader.Get<T>();
		}

		private readonly IDbCommand m_command;
		private readonly IDataReader m_reader;
		private readonly DbProviderMethods m_methods;
		private bool m_next;
	}
}
