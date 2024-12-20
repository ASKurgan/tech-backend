// using CSharpFunctionalExtensions;
// using SachkovTech.Core.Abstractions;
// using SachkovTech.Issues.Application.Interfaces;
// using SachkovTech.SharedKernel;
//
// namespace SachkovTech.Issues.Application.Features.GrpcNotificationServiceTester.Commands.PushNotification;
//
// public class PushNotificationHandler : ICommandHandler<Guid, PushNotificationCommand>
// {
//     private readonly IGrpcNotificationServiceClient _grpcClient;
//
//     public PushNotificationHandler(IGrpcNotificationServiceClient grpcClient)
//     {
//         _grpcClient = grpcClient;
//     }
//     public async Task<Result<Guid, ErrorList>> Handle(PushNotificationCommand command, CancellationToken cancellationToken = default)
//     {
//         var result = await _grpcClient.PushNotificationAsync(command, cancellationToken);
//         return result;
//     }
// }