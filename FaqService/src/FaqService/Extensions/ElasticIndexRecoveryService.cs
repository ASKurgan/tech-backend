using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FaqService.Extensions;

public class ElasticIndexRecoveryService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SearchRepository _searchRepository;
    private readonly ILogger<ElasticIndexRecoveryService> _logger;

    public ElasticIndexRecoveryService(
        ApplicationDbContext dbContext,
        SearchRepository searchRepository,
        ILogger<ElasticIndexRecoveryService> logger)
    {
        _dbContext = dbContext;
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task RestoreElasticIndex(Guid postId, bool indexResult, CancellationToken cancellationToken)
    {
        if (!indexResult)
            return;

        _logger.LogWarning("Restoring previous index state for post {PostId}.", postId);

        var post = await _dbContext.Posts.SingleOrDefaultAsync(p => p.Id == postId, cancellationToken);

        if (post is null)
        {
            _logger.LogError("Failed to restore index for post {PostId}. Post not found after rollback.", postId);
            return;
        }

        try
        {
            await _searchRepository.IndexPost(post);
            _logger.LogInformation("Successfully restored index for post {PostId}.", postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to restore index for post {PostId} after rollback.", postId);
        }
    }
}