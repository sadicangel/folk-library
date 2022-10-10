﻿using Ardalis.Specification;
using Bogus;
using FolkLibrary.Interfaces;
using FolkLibrary.Models;
using HotChocolate.Language;
using Humanizer;
using MongoDB.Driver;
using NAudio.MediaFoundation;
using OfficeOpenXml.ConditionalFormatting;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace FolkLibrary.Services;
internal sealed class MongoRepository<T> : IMongoRepository<T> where T : class, IDataTransterObject
{
    private readonly IMongoCollection<T> _collection;

    public string CollectionName { get; } = typeof(T).Name[..^3].Pluralize().Camelize();

    public MongoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<T>(CollectionName);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        return entities;
    }

    public async Task<bool> AnyAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.AnyAsync(cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(specification.ToFilterDefinition(), specification.ToCountOptions(), cancellationToken);
        return (int)count;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return (int)count;
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.DeleteManyAsync(Builders<T>.Filter.In(e => e.Id, entities.Select(e => e.Id)), cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken) where TId : notnull
    {
        var cursor = await _collection.FindAsync(e => e.Id.Equals(id), cancellationToken: cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    [Obsolete]
    public Task<T?> GetBySpecAsync(ISpecification<T> specification, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    [Obsolete]
    public Task<TResult?> GetBySpecAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<T>> ListAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    private string EncodePageIndex(int pageIndex)
    {
        var id = JsonNode.Parse("{}")!;
        id["name"] = CollectionName;
        id["index"] = pageIndex;
        var json = id.ToJsonString();
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private bool TryDecodePageIndex(string? token, out int pageIndex)
    {
        pageIndex = 0;

        try
        {
            if (String.IsNullOrWhiteSpace(token))
                return false;

            var id = JsonNode.Parse(Convert.FromBase64String(token))!;

            if (id["name"]?.GetValue<string>() != CollectionName)
                return false;

            if (id["index"]?.GetValue<int>() is not int index)
                return false;

            pageIndex = index;

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Page<T>> ListPagedAsync(ISpecification<T> specification, string? continuationToken, CancellationToken cancellationToken = default)
    {
        const string countName = "count";
        const string dataName = "data";
        const int pageSize = 10;

        TryDecodePageIndex(continuationToken, out int pageIndex);

        var countFacet = AggregateFacet.Create(countName, PipelineDefinition<T, AggregateCountResult>.Create(
            new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

        var dataFacet = AggregateFacet.Create(dataName, PipelineDefinition<T, T>.Create(
            new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Sort(specification.ToSortDefinition()),
                PipelineStageDefinitionBuilder.Skip<T>(pageIndex * pageSize),
                PipelineStageDefinitionBuilder.Limit<T>(pageSize),
            }));

        var aggregate = await _collection.Aggregate().Match(specification.ToFilterDefinition()).Facet(countFacet, dataFacet).FirstAsync(cancellationToken);

        var count = aggregate.Facets.Single(f => f.Name == countName).Output<AggregateCountResult>();
        var pageCount = (count.Count > 0 ? (int)count[0].Count : 0) / pageSize;

        var data = aggregate.Facets.Single(f => f.Name == dataName).Output<T>();

        var nextPageIndex = pageIndex + 1;

        return new Page<T>
        {
            ContinuationToken = nextPageIndex < pageCount ? EncodePageIndex(nextPageIndex) : null,
            Elements = data
        };
    }

    public async Task<Page<TResult>> ListPagedAsync<TResult>(ISpecification<T, TResult> specification, string? continuationToken, CancellationToken cancellationToken = default)
    {
        const string countName = "count";
        const string dataName = "data";
        const int pageSize = 10;

        TryDecodePageIndex(continuationToken, out int pageIndex);

        var countFacet = AggregateFacet.Create(countName, PipelineDefinition<T, AggregateCountResult>.Create(
            new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Count<T>()
            }));

        var dataFacet = AggregateFacet.Create(dataName, PipelineDefinition<T, TResult>.Create(
            new IPipelineStageDefinition[]
            {
                PipelineStageDefinitionBuilder.Sort(specification.ToSortDefinition()),
                PipelineStageDefinitionBuilder.Skip<T>(pageIndex * pageSize),
                PipelineStageDefinitionBuilder.Limit<T>(pageSize),
                PipelineStageDefinitionBuilder.Project(specification.Selector)
            }));

        var aggregate = await _collection.Aggregate().Match(specification.ToFilterDefinition()).Facet(countFacet, dataFacet).FirstAsync(cancellationToken);

        var count = aggregate.Facets.Single(f => f.Name == countName).Output<AggregateCountResult>();
        var pageCount = (count.Count > 0 ? (int)count[0].Count : 0) / pageSize;

        var data = aggregate.Facets.Single(f => f.Name == dataName).Output<TResult>();

        var nextPageIndex = pageIndex + 1;

        return new Page<TResult>
        {
            ContinuationToken = nextPageIndex < pageCount ? EncodePageIndex(nextPageIndex) : null,
            Elements = data
        };
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

    public async Task<T?> SingleOrDefaultAsync(ISingleResultSpecification<T> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> SingleOrDefaultAsync<TResult>(ISingleResultSpecification<T, TResult> specification, CancellationToken cancellationToken = default)
    {
        var cursor = await _collection.FindAsync(specification.ToFilterDefinition(), specification.ToFindOptions(), cancellationToken);
        return await cursor.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _collection.FindOneAndReplaceAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _collection.BulkWriteAsync(entities.Select(e => new ReplaceOneModel<T>(Builders<T>.Filter.Eq(f => f.Id, e.Id), e)), cancellationToken: cancellationToken);
    }
}

internal static class SpecificationExtensions
{
    public static FilterDefinition<T> ToFilterDefinition<T>(this ISpecification<T> specification)
    {
        if (!specification.WhereExpressions.Any())
            return Builders<T>.Filter.Empty;
        if (specification.WhereExpressions.Take(2).Count() == 1)
            return Builders<T>.Filter.Where(specification.WhereExpressions.Single().Filter);
        return Builders<T>.Filter.And(specification.WhereExpressions.Select(e => Builders<T>.Filter.Where(e.Filter)));
    }

    public static CountOptions? ToCountOptions<T>(this ISpecification<T> specification)
    {
        if (specification.Take is null && specification.Skip is null)
            return null;
        return new CountOptions
        {
            Limit = specification.Take,
            Skip = specification.Skip
        };
    }

    public static FindOptions<T>? ToFindOptions<T>(this ISpecification<T> specification)
    {
        if (specification.Take is null && specification.Skip is null && !specification.OrderExpressions.Any())
            return null;
        return new FindOptions<T>
        {
            Limit = specification.Take,
            Skip = specification.Skip,
            Sort = specification.ToSortDefinition()
        };
    }

    public static FindOptions<T, TProjection> ToFindOptions<T, TProjection>(this ISpecification<T, TProjection> specification)
    {
        return new FindOptions<T, TProjection>
        {
            Limit = specification.Take,
            Skip = specification.Skip,
            Sort = specification.ToSortDefinition(),
            Projection = Builders<T>.Projection.Expression(specification.Selector),
        };
    }

    public static SortDefinition<T>? ToSortDefinition<T>(this ISpecification<T> specification)
    {
        if (!specification.OrderExpressions.Any())
            return null;

        using var enumerator = specification.OrderExpressions.GetEnumerator();
        enumerator.MoveNext();
        var expression = enumerator.Current;
        var sort = expression.OrderType == OrderTypeEnum.OrderBy || expression.OrderType == OrderTypeEnum.ThenBy
            ? Builders<T>.Sort.Ascending(expression.KeySelector)
            : Builders<T>.Sort.Descending(expression.KeySelector);
        while (enumerator.MoveNext())
        {
            expression = enumerator.Current;
            sort = expression.OrderType == OrderTypeEnum.OrderBy || expression.OrderType == OrderTypeEnum.ThenBy
                ? sort.Ascending(expression.KeySelector)
                : sort.Descending(expression.KeySelector);
        }

        return sort;
    }
}