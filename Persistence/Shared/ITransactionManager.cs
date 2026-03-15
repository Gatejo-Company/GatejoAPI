namespace API.Persistence.Shared;

public interface ITransactionManager {
	Task BeginAsync();
	Task CommitAsync();
	Task RollbackAsync();
}
