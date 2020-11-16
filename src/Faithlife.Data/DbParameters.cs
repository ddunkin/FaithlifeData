using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Faithlife.Reflection;

namespace Faithlife.Data
{
	/// <summary>
	/// An immutable list of parameters.
	/// </summary>
	public readonly struct DbParameters : IReadOnlyList<(string Name, object? Value)>
	{
		/// <summary>
		/// An empty list of parameters.
		/// </summary>
		[SuppressMessage("Performance", "CA1805:Do not initialize unnecessarily", Justification = "Intentional API.")]
		public static readonly DbParameters Empty = default;

		/// <summary>
		/// Creates a list of parameters with one parameter.
		/// </summary>
		public static DbParameters Create(string name, object? value) =>
			new DbParameters(new[] { (name, value) });

		/// <summary>
		/// Creates a list of parameters from a single name and a collection of values.
		/// </summary>
		public static DbParameters Create(string name, IEnumerable<object?> values)
		{
			int index = 0;
			var parameters = DbParameters.Empty;
			foreach (object? value in values ?? throw new ArgumentNullException(nameof(values)))
				parameters = parameters.Add($"{name}_{index++}", value);
			return parameters;
		}

		/// <summary>
		/// Creates a list of parameters from a collection of values.
		/// <param name="name">A function taking the index of the value in the collection as an argument and returning the name of its parameter.</param>
		/// <param name="values">The collection of values to add.</param>
		/// </summary>
		public static DbParameters Create(Func<int, string> name, IEnumerable<object?> values)
		{
			int index = 0;
			var parameters = DbParameters.Empty;
			foreach (object? value in values ?? throw new ArgumentNullException(nameof(values)))
				parameters = parameters.Add(name(index++), value);
			return parameters;
		}

		/// <summary>
		/// Creates a list of parameters from tuples.
		/// </summary>
		public static DbParameters Create(params (string Name, object? Value)[] parameters) =>
			new DbParameters(parameters ?? throw new ArgumentNullException(nameof(parameters)));

		/// <summary>
		/// Creates a list of parameters from a sequence of tuples.
		/// </summary>
		public static DbParameters Create(IEnumerable<(string Name, object? Value)> parameters) =>
			new DbParameters(parameters ?? throw new ArgumentNullException(nameof(parameters)));

		/// <summary>
		/// Creates a list of parameters from a sequence of tuples.
		/// </summary>
		public static DbParameters Create<T>(IEnumerable<(string Name, T Value)> parameters) =>
			new DbParameters((parameters ?? throw new ArgumentNullException(nameof(parameters))).Select(x => (x.Name, (object?) x.Value)));

		/// <summary>
		/// Creates a list of parameters from a dictionary.
		/// </summary>
		public static DbParameters Create<T>(IEnumerable<KeyValuePair<string, T>> parameters) =>
			new DbParameters((parameters ?? throw new ArgumentNullException(nameof(parameters))).Select(x => (x.Key, (object?) x.Value)));

		/// <summary>
		/// Creates a list of parameters from the properties of a DTO.
		/// </summary>
		public static DbParameters FromDto(object dto, string? suffix = null) =>
			new DbParameters(DtoInfo.GetInfo((dto ?? throw new ArgumentNullException(nameof(dto))).GetType()).Properties.Select(x => (x.Name + (suffix == null ? "" : $"_{suffix}"), x.GetValue(dto))));

		/// <summary>
		/// Creates a list of parameters from the properties of a DTO.
		/// </summary>
		public static DbParameters FromDto(string name, object dto, string? suffix = null) =>
			new DbParameters(DtoInfo.GetInfo((dto ?? throw new ArgumentNullException(nameof(dto))).GetType()).Properties.Select(x => ($"{name}_{x.Name}" + (suffix == null ? "" : $"_{suffix}"), x.GetValue(dto))));

		/// <summary>
		/// Creates a list of parameters from the properties of a DTO.
		/// <param name="name">A function taking the name of a DTO property as an argument and returning the name of its database parameter.</param>
		/// <param name="dto">The DTO to retrieve parameters from.</param>
		/// <param name="suffix">An optional suffix to add to the end of each parameter name after adding an underscore.</param>
		/// </summary>
		public static DbParameters FromDto(Func<string, string> name, object dto, string? suffix = null) =>
			new DbParameters(DtoInfo.GetInfo((dto ?? throw new ArgumentNullException(nameof(dto))).GetType()).Properties.Select(x => (name(x.Name) + (suffix == null ? "" : $"_{suffix}"), x.GetValue(dto))));

		/// <summary>
		/// Creates a list of parameters from the collective properties of a sequence of DTOs.
		/// </summary>
		public static DbParameters FromDtos(IEnumerable<object> dtos)
		{
			int index = 0;
			var parameters = DbParameters.Empty;
			foreach (var dto in dtos ?? throw new ArgumentNullException(nameof(dtos)))
				parameters = parameters.AddDto(dto, suffix: $"{index++}");
			return parameters;
		}

		/// <summary>
		/// Creates a list of parameters from the collective properties of a sequence of DTOs.
		/// </summary>
		public static DbParameters FromDtos(string name, IEnumerable<object> dtos)
		{
			int index = 0;
			var parameters = DbParameters.Empty;
			foreach (object dto in dtos ?? throw new ArgumentNullException(nameof(dtos)))
				parameters = parameters.AddDto(name, dto, suffix: $"{index++}");
			return parameters;
		}

		/// <summary>
		/// Creates a list of parameters from the collective properties of a sequence of DTOs.
		/// <param name="name">A function taking the name of a DTO property and the index of the DTO in the collection as arguments and returning the name of its database parameter.</param>
		/// <param name="dtos">The collection of DTOs to retrieve parameters from.</param>
		/// </summary>
		public static DbParameters FromDtos(Func<string, int, string> name, IEnumerable<object> dtos)
		{
			int index = 0;
			var parameters = DbParameters.Empty;
			foreach (object dto in dtos ?? throw new ArgumentNullException(nameof(dtos)))
			{
				parameters = parameters.AddDto((string prop) => name(prop, index), dto);
				index++;
			}
			return parameters;
		}

		/// <summary>
		/// The number of parameters.
		/// </summary>
		public int Count => Parameters.Count;

		/// <summary>
		/// The parameter at the specified index.
		/// </summary>
		public (string Name, object? Value) this[int index] => Parameters[index];

		/// <summary>
		/// Adds a parameter.
		/// </summary>
		public DbParameters Add(string name, object? value) => new DbParameters(Parameters.Append((name, value)));

		/// <summary>
		/// Adds parameters from another instance.
		/// </summary>
		public DbParameters Add(DbParameters parameters) => new DbParameters(Parameters.Concat(parameters));

		/// <summary>
		/// Adds parameters from a single name and a collection of values.
		/// </summary>
		public DbParameters Add(string name, IEnumerable<object?> values) => Add(Create(name, values));

		/// <summary>
		/// Adds parameters from a collection of values.
		/// <param name="name">A function taking the index of the value in the collection as an argument and returning the name of its parameter.</param>
		/// <param name="values">The collection of values to add.</param>
		/// </summary>
		public DbParameters Add(Func<int, string> name, IEnumerable<object?> values) => Add(Create(name, values));

		/// <summary>
		/// Adds parameters from tuples.
		/// </summary>
		public DbParameters Add(params (string Name, object? Value)[] parameters) => Add(Create(parameters));

		/// <summary>
		/// Adds parameters from a sequence of tuples.
		/// </summary>
		public DbParameters Add(IEnumerable<(string Name, object? Value)> parameters) => Add(Create(parameters));

		/// <summary>
		/// Adds parameters from a sequence of tuples.
		/// </summary>
		public DbParameters Add<T>(IEnumerable<(string Name, T Value)> parameters) => Add(Create(parameters));

		/// <summary>
		/// Adds parameters from a dictionary.
		/// </summary>
		public DbParameters Add<T>(IEnumerable<KeyValuePair<string, T>> parameters) => Add(Create(parameters));

		/// <summary>
		/// Adds parameters from the properties of a DTO.
		/// </summary>
		public DbParameters AddDto(object dto, string? suffix = null) => Add(FromDto(dto, suffix));

		/// <summary>
		/// Adds parameters from the properties of a DTO.
		/// </summary>
		public DbParameters AddDto(string name, object dto, string? suffix = null) => Add(FromDto(name, dto, suffix));

		/// <summary>
		/// Adds parameters from the properties of a DTO.
		/// <param name="name">A function taking the name of a DTO property as an argument and returning the name of its database parameter.</param>
		/// <param name="dto">The DTO to retrieve parameters from.</param>
		/// <param name="suffix">An optional suffix to add to the end of each parameter name after adding an underscore.</param>
		/// </summary>
		public DbParameters AddDto(Func<string, string> name, object dto, string? suffix = null) => Add(FromDto(name, dto, suffix));

		/// <summary>
		/// Adds parameters from the collective properties of a sequence of DTOs.
		/// </summary>
		public DbParameters AddDtos(IEnumerable<object> dtos) => Add(FromDtos(dtos));

		/// <summary>
		/// Adds parameters from the collective properties of a sequence of DTOs.
		/// </summary>
		public DbParameters AddDtos(string name, IEnumerable<object> dtos) => Add(FromDtos(name, dtos));

		/// <summary>
		/// Adds parameters from the collective properties of a sequence of DTOs.
		/// <param name="name">A function taking the name of a DTO property and the index of the DTO in the collection as arguments and returning the name of its database parameter.</param>
		/// <param name="dtos">The collection of DTOs to retrieve parameters from.</param>
		/// </summary>
		public DbParameters AddDtos(Func<string, int, string> name, IEnumerable<object> dtos) => Add(FromDtos(name, dtos));

		/// <summary>
		/// Creates a dictionary of parameters.
		/// </summary>
		public Dictionary<string, object?> ToDictionary()
		{
			var dictionary = new Dictionary<string, object?>();
			foreach (var parameter in Parameters)
				dictionary[parameter.Name] = parameter.Value;
			return dictionary;
		}

		/// <summary>
		/// Used to enumerate the parameters.
		/// </summary>
		public IEnumerator<(string Name, object? Value)> GetEnumerator() => Parameters.GetEnumerator();

		/// <summary>
		/// Used to enumerate the parameters.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator() => Parameters.GetEnumerator();

		private DbParameters(IEnumerable<(string Name, object? Value)> parameters) => m_parameters = parameters.ToList();

		private IReadOnlyList<(string Name, object? Value)> Parameters => m_parameters ?? Array.Empty<(string Name, object? Value)>();

		private readonly IReadOnlyList<(string Name, object? Value)>? m_parameters;
	}
}
