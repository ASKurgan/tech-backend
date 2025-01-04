using CSharpFunctionalExtensions;
using FaqService.Infrastructure.Repositories;
using SharedKernel;

namespace FaqService.Features.Commands.Answer.ChangeRating;

public class DecreaseAnswerRatingHandler
{
    private readonly ILogger<DecreaseAnswerRatingHandler> _logger;
    private readonly PostsRepository _repository;

    public DecreaseAnswerRatingHandler(
        ILogger<DecreaseAnswerRatingHandler> logger,
        PostsRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(DecreaseAnswerRatingCommand command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetById(command.PostId, cancellationToken);
        if (post.IsFailure)
            return post.Error;

        var answer = post.Value.Answers.FirstOrDefault(a => a.Id == command.AnswerId);
        if (answer is null)
            return Error.NotFound("answer", "Answer not found");

        answer.DecreaseRating();

        await _repository.Save(cancellationToken);

        _logger.LogInformation($"Decreased answer rating with id: {command.AnswerId}");

        return answer.Id;
    }
}