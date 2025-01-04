using CSharpFunctionalExtensions;
using FaqService.Infrastructure.Repositories;
using SharedKernel;

namespace FaqService.Features.Commands.Answer.UpdateMainInfo;

public class UpdateAnswerMainInfoHandler
{
    private readonly ILogger<UpdateAnswerMainInfoHandler> _logger;
    private readonly PostsRepository _repository;

    public UpdateAnswerMainInfoHandler(
        ILogger<UpdateAnswerMainInfoHandler> logger,
        PostsRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<Result<Guid, Error>> Handle(UpdateAnswerMainInfoCommand command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetById(command.PostId, cancellationToken);
        if (post.IsFailure)
            return post.Error;

        var answer = post.Value.Answers.FirstOrDefault(a => a.Id == command.AnswerId);
        if (answer is null)
            return Error.NotFound("answer", "Answer not found");

        var result = answer.UpdateMainInfo(command.Text);
        if (result.IsFailure)
            return result.Error;

        await _repository.Save(cancellationToken);

        _logger.LogInformation($"Updated answer main info with id: {command.PostId}");

        return answer.Id;
    }
}