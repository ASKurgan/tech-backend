using FaqService.Contracts;
using FaqService.Entities;
using FaqService.Features.Elastic;
using Nest;

namespace FaqService.Infrastructure.Repositories;

public class SearchRepository
{
    private readonly IElasticClient _client;

    public SearchRepository(IElasticClient client)
    {
        _client = client;
    }

    public async Task<List<Guid>> SearchPosts(GetPostsQuery query, CancellationToken cancellationToken)
    {
        var searchResponse = await ExecuteSearchQuery(query, cancellationToken);

        var ids = searchResponse.Hits
            .Select(hit => Guid.TryParse(hit.Id, out var guid) ? guid : Guid.Empty)
            .Where(guid => guid != Guid.Empty)
            .ToList();
        return ids;
    }

    public async Task<bool> IndexPost(Post post)
    {
        var postElastic = new PostElastic
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            ReplLink = post.ReplLink,
            Status = post.Status.ToString(),
            CreatedAt = post.CreatedAt,
            Tags = post.Tags,
            IssueId = post.IssueId,
            LessonId = post.LessonId,
        };

        var response = await _client.IndexDocumentAsync(postElastic);
        if (response.IsValid)
        {
            Console.WriteLine("Document indexed successfully.");
            return true;
        }
        else
        {
            Console.WriteLine($"Failed to index document: {response.ServerError?.Error?.Reason}");
            return false;
        }
    }
    
    public async Task DeletePost(Guid postId, CancellationToken cancellationToken)
    {
        var response = await _client.DeleteAsync<PostElastic>(postId, d => d
                .Index("posts"),
            cancellationToken
        );

        Console.WriteLine(response.IsValid
            ? $"Document with ID {postId} deleted successfully."
            : $"Failed to delete document with ID {postId}: {response.ServerError?.Error?.Reason}");
    }

    private async Task<ISearchResponse<PostElastic>> ExecuteSearchQuery(GetPostsQuery query,
        CancellationToken cancellationToken)
    {
        return await _client.SearchAsync<PostElastic>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(
                            m => m.MultiMatch(mm => mm
                                .Fields(f => f
                                    .Field(p => p.Title, boost: 1.0)
                                    .Field(p => p.Description, boost: 2.0)
                                )
                                .Query(query.SearchText ?? "")
                                .Fuzziness(Fuzziness.Auto)
                            ),
                            m => m.Term(t =>
                                t.Field(p => p.Status.Suffix("keyword"))
                                    .Value(query.Status?.ToString())),
                            m => query.Tags != null && query.Tags.Any()
                                ? m.Terms(t => t.Field(p => p.Tags.Suffix("keyword")).Terms(query.Tags))
                                : null, 
                            m => query.IssueId.HasValue
                                ? m.Term(t => t.Field(p => p.IssueId.Suffix("keyword")).Value(query.IssueId.ToString()))
                                : null, 
                            m => query.LessonId.HasValue
                                ? m.Term(t =>
                                    t.Field(p => p.LessonId.Suffix("keyword")).Value(query.LessonId.ToString()))
                                : null 
                        )
                    )
                )
                .Size(100),
            cancellationToken
        );
    }
}