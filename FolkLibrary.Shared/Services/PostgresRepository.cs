﻿using Ardalis.Specification.EntityFrameworkCore;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;

namespace FolkLibrary.Services;

internal sealed class PostgresRepository<T> : RepositoryBase<T>, IPostgresRepository<T> where T : class, IDomainObject
{
    public PostgresRepository(FolkDbContext context) : base(context)
    {
    }
}
