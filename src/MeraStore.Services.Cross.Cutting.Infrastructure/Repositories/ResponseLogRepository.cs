using MeraStore.Services.Cross.Cutting.Application.Repositories;
using MeraStore.Services.Cross.Cutting.Domain.Entities;
using MeraStore.Shared.Kernel.Persistence.Interfaces;
using MeraStore.Shared.Kernel.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeraStore.Services.Cross.Cutting.Infrastructure.Repositories;

public class ResponseLogRepository(AppDbContext context, ICommitStrategy commit)
    : Repository<ResponseLog>(context, commit), IResponseLogRepository;

