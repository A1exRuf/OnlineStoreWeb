using Domain.Abstractions;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class CachedRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    private readonly Repository<TEntity> _decorated;
    private readonly IDistributedCache _distributedCache;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly DistributedCacheEntryOptions _cashOptions;

    public CachedRepository(
        Repository<TEntity> decorated, 
        IDistributedCache distributedCache)
    {
        _decorated = decorated;
        _distributedCache = distributedCache;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        _cashOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2),
        };
    }

    public Task AddAsync(
        TEntity entity, 
        CancellationToken cancellationToken = default)
    {
        return _decorated.AddAsync(entity, cancellationToken);
    }

    public async Task<int> RemoveAsync(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default)
    {
        string key = GetKey<TEntity>(filter);

        string? cachedEntity = await _distributedCache.GetStringAsync(
            key,
            cancellationToken);

        if (cachedEntity != null)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
        }

        return await _decorated.RemoveAsync(filter, cancellationToken);
    }

    public void Update(TEntity entity)
    {
        _decorated.Update(entity);
    }

    public async Task<bool> ExistsAsync(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default)
    {
        return await _decorated.ExistsAsync(filter, cancellationToken);
    }

    public async Task<TEntity?> GetAsync(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default, 
        params Expression<Func<TEntity, object>>[] includes)
    {
        return await _decorated.GetAsync(filter, cancellationToken, includes);
    }

    public async Task<TDto?> GetAsync<TDto>(
        IFilter<TEntity> filter, 
        CancellationToken cancellationToken = default)
    {
        string key = GetKey<TDto>(filter);

        string? cachedDto = await _distributedCache.GetStringAsync(
            key, 
            cancellationToken);

        TDto? dto;

        if (string.IsNullOrEmpty(cachedDto))
        {
            dto = await _decorated.GetAsync<TDto>(filter, cancellationToken);
                
            if (dto == null)
                return dto;

            await _distributedCache.SetStringAsync(
                key, 
                JsonSerializer.Serialize(dto, _jsonOptions), 
                _cashOptions,
                cancellationToken);

            return dto;
        }

        dto = JsonSerializer.Deserialize<TDto>(cachedDto, _jsonOptions);

        return dto;
    }

    public async Task<List<TDto>> GetListAsync<TDto>(
        IFilter<TEntity> filter, 
        Expression<Func<TEntity, object>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        string key = GetKey<TDto>(
            filter: filter,
            orderBy: orderBy,
            descending: descending);

        string? cachedList = await _distributedCache.GetStringAsync(
            key,
            cancellationToken);

        List<TDto> list;

        if (string.IsNullOrEmpty(cachedList))
        {
            list = await _decorated.GetListAsync<TDto>(
                filter,
                orderBy,
                descending,
                cancellationToken);

            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(list, _jsonOptions),
                _cashOptions,
                cancellationToken);

            return list;
        }

        list = JsonSerializer.Deserialize<List<TDto>>(cachedList, _jsonOptions)!;

        return list!;
    }

    public async Task<PagedList<TDto>> GetPagedListAsync<TDto>(
        int page, 
        int pageSize, 
        IFilter<TEntity> filter,
        ISearch<TEntity>? search = null, 
        Expression<Func<TEntity, object>>? orderBy = null, 
        bool descending = false, 
        CancellationToken cancellationToken = default)
    {
        string key = GetKey<TDto>(
            filter,
            search,
            page,
            pageSize,
            orderBy,
            descending);

        string? cachedPagedList = await _distributedCache.GetStringAsync(
            key,
            cancellationToken);

        PagedList<TDto> pagedList;

        if (string.IsNullOrEmpty(cachedPagedList))
        {
            pagedList = await _decorated.GetPagedListAsync<TDto>(
                page,
                pageSize,
                filter,
                search,
                orderBy,
                descending,
                cancellationToken);

            await _distributedCache.SetStringAsync(
                key,
                JsonSerializer.Serialize(pagedList, _jsonOptions),
                _cashOptions,
                cancellationToken);

            return pagedList;
        }
        
        pagedList = JsonSerializer.Deserialize<PagedList<TDto>>(cachedPagedList, _jsonOptions)!;

        return pagedList!;
    }

    private string GetKey<TDto>(
        IFilter<TEntity> filter,
        ISearch<TEntity>? search = null,
        int? page = null,
        int? pageSize = null,
        Expression<Func<TEntity, object>>? orderBy = null,
        bool? descending = null)
    {
        var parts = new List<string>
        {
            typeof(TDto).Name,
            filter.GetCacheKey()
        };

        if (search != null) parts.Add(JsonSerializer.Serialize(search, _jsonOptions));
        if (page != null) parts.Add($"page={page}");
        if (pageSize != null) parts.Add($"size={pageSize}");
        if (orderBy != null) parts.Add(orderBy.ToString());
        if (descending != null) parts.Add($"desc={descending}");

        return string.Join(":", parts);
    }
}