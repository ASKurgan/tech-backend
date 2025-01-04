using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;
using FaqService.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace FaqService.Infrastructure.Repositories;

public class PostsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Add(Post post, CancellationToken cancellationToken)
    {
        await _dbContext.Posts.AddAsync(post, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return post.Id;
    }

    public async Task<UnitResult<Error>> Save(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success<Error>();
    }

    public async Task<Result<Post, Error>> GetById(Guid postId, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Answers)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post == null)
            return Error.NotFound("post","Post not found");
        return post;
    }

    public async Task<UnitResult<Error>> Delete(Guid postId, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Include(p => p.Answers)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
        if (post == null)
            return Error.NotFound("post","Post not found");
        _dbContext.Posts.Remove(post);
        return Result.Success<Error>();
    }

    public IQueryable<Post> QueryPostsByIds(List<Guid> postIds, CancellationToken cancellationToken)
    {
        return _dbContext.Posts
            .Include(p => p.Answers)
            .Where(p => postIds.Contains(p.Id));
    }
}