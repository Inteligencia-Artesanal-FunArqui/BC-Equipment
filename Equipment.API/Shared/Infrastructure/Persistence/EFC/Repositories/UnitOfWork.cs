using OsitoPolar.EquipmentService.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace OsitoPolar.EquipmentService.Shared.Infrastructure.Persistence.EFC.Repositories;

public class UnitOfWork(DbContext context) : IUnitOfWork
{
    /// <inheritdoc />
    public async Task CompleteAsync()
    {
        await context.SaveChangesAsync();
    }
}