using Application.Filters;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Services;

public class RepositoryTests
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<User> _repository;
    private readonly UnitOfWork _unitOfWork;

    public RepositoryTests()
    {
        DbContextOptionsBuilder dbOptions = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());

        _context = new ApplicationDbContext(dbOptions.Options);
        _repository = new Repository<User>(_context);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Entity()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "test@example.com", "hashedPass", UserRole.Customer);

        // Act
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var count = await _context.Set<User>().CountAsync();
        Assert.Equal(1, count);
    }

    // RemoveAsync not supported by InMemoryDatabase

    [Fact]
    public async Task Update_Should_Update_Entity()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "old@email.com", "hash1", UserRole.Customer);
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Act
        user.Email = "new@email.com";
        _repository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        var updatedUser = await _context.Set<User>().FindAsync(user.Id);
        Assert.Equal("new@email.com", updatedUser!.Email);
    }

    [Fact]
    public async Task ExistsAsync_Should_Return_True_When_EntityExists()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "exists@example.com", "hashedPass", UserRole.Customer);
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var filter = new UserFilter { Email = "exists@example.com" };

        // Act
        var exists = await _repository.ExistsAsync(filter);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Entity_By_Filter()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "get@example.com", "hashedPass", UserRole.Customer);
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var filter = new UserFilter { Email = "get@example.com" };

        // Act
        var fetchedUser = await _repository.GetAsync(filter);

        // Assert
        Assert.NotNull(fetchedUser);
        Assert.Equal(user.Email, fetchedUser!.Email);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Projected_Dto()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "get@test.com", "hash", UserRole.Admin);
        await _repository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var filter = new UserFilter { Email = "get@test.com" };

        // Act
        var dto = await _repository.GetAsync<UserDto>(filter);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(user.Id, dto!.Id);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_List_Of_Dtos()
    {
        // Arrange
        var users = new[]
        {
            new User(Guid.NewGuid(), "list1@test.com", "hash", UserRole.Admin),
            new User(Guid.NewGuid(), "list2@test.com", "hash", UserRole.Customer)
        };
        foreach (var u in users)
            await _repository.AddAsync(u);
        await _unitOfWork.SaveChangesAsync();

        var filter = new UserFilter(); // no filter, get all

        // Act
        var list = await _repository.GetListAsync<UserDto>(
            filter,
            orderBy: u => u.Email);

        // Assert
        Assert.NotNull(list);
        Assert.Equal(2, list.Count);
        Assert.Contains(list, x => x.Id == users[0].Id);
        Assert.Contains(list, x => x.Id == users[1].Id);
    }

    [Fact]
    public async Task GetPagedListAsync_Should_Return_PagedList_Of_Dtos()
    {
        // Arrange
        for (int i = 1; i <= 10; i++)
        {
            var user = new User(Guid.NewGuid(), $"paged{i}@test.com", "hash", UserRole.Customer);
            await _repository.AddAsync(user);
        }
        await _unitOfWork.SaveChangesAsync();

        var filter = new UserFilter();

        // Act
        var pagedList = await _repository.GetPagedListAsync<UserDto>(
            page: 2,
            pageSize: 3,
            filter: filter,
            orderBy: u => u.Email);

        // Assert
        Assert.NotNull(pagedList);
        Assert.Equal(10, pagedList.TotalCount);
        Assert.Equal(2, pagedList.Page);
        Assert.Equal(3, pagedList.PageSize);
        Assert.Equal(3, pagedList.Items.Count);
    }
}

record UserDto(Guid Id, string Email, UserRole Role);
