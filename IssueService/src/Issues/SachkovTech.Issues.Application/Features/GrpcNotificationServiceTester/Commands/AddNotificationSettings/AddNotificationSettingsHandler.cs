// using CSharpFunctionalExtensions;
// using SachkovTech.Core.Abstractions;
// using SachkovTech.Issues.Application.Interfaces;
// using SachkovTech.SharedKernel;
//
// namespace SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.AddNotificationSettings;
// public class AddNotificationSettingsHandler : ICommandHandler<Guid, AddNotificationSettingsCommand>
// {
//     private readonly IGrpcNotificationServiceClient _grpcClient;
//
//     public AddNotificationSettingsHandler(IGrpcNotificationServiceClient grpcClient)
//     {
//         _grpcClient = grpcClient;
//     }
//
//     public async Task<Result<Guid, ErrorList>> Handle(AddNotificationSettingsCommand command, CancellationToken cancellationToken = default)
//     {
//         var result = await _grpcClient.AddNotificationSettingsAsync(command, cancellationToken);
//         return result;
//     }
// }