using Microsoft.EntityFrameworkCore;

namespace amorphie.shield.Transactions;

public class TransactionRepository
{
    private readonly ShieldDbContext _dbContext;
    private readonly DbSet<Transaction> _dbSet;

    public TransactionRepository(ShieldDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<Transaction>();
    }

    public async Task<Transaction> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var transaction = await _dbContext.Transactions.Include(i => i.Activities).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (transaction != null)
        {
            return transaction;
        }

        throw new EntityNotFoundException(typeof(Transaction), id);
    }

    public async Task<Transaction> InsertAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        _dbSet.Add(transaction);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        if (_dbContext.Set<Transaction>().Local.All(e => e != transaction))
        {
            _dbContext.Set<Transaction>().Attach(transaction);
            _dbContext.Update(transaction);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return transaction;
    }
}
