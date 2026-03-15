using API.DataAccess.Interfaces;
using API.Utils.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace API.Persistence.Shared;

[Injectable(ServiceLifetime.Scoped)]
public class TransactionManager : ITransactionManager {
	private readonly ICConnection _connection;

	public TransactionManager(ICConnection connection) => _connection = connection;

	public Task BeginAsync() => _connection.BeginTransaction();
	public Task CommitAsync() => _connection.CommitTransaction();
	public Task RollbackAsync() => _connection.CancelTransaction();
}
