using FaqService.Contracts;
using FaqService.Dtos;
using FaqService.Extensions;
using FaqService.Infrastructure;
using FaqService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using SachkovTech.Framework.Endpoints;

namespace FaqService.Features.Post;

public class GetPostsWithCursorPagination
{
    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("posts{postId:guid}", Handler);
        }
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid postId,
        [AsParameters] GetPostsQuery query,
        [FromServices] SearchRepository searchRepository,
        [FromServices] ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var postIds = await searchRepository.SearchPosts(query, cancellationToken);

        var postsQuery = dbContext.Posts.Where(p => postIds.Contains(p.Id));

        var paginatedPosts = await postsQuery.ToCursorListWithOrderedIds(
            cursor: query.Cursor,
            orderedIds: postIds,
            limit: query.Limit,
            cancellationToken: cancellationToken);

        var postDtos = paginatedPosts.Items.Select(post => new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            ReplLink = post.ReplLink,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            Tags = post.Tags,
            IssueId = post.IssueId,
            LessonId = post.LessonId,
            //TODO: Надо доделать запрос
            // AnswerId = post.AnswerId,
            // CountOfAnswers = post.Answers.Count,
        }).ToList();

        return ResultResponse.Ok(new CursorList<PostDto>(
            items: postDtos,
            cursor: paginatedPosts.Cursor,
            nextCursor: paginatedPosts.NextCursor,
            limit: paginatedPosts.Limit,
            totalCount: paginatedPosts.TotalCount
        ));
    }
}