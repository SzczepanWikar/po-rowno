using Core.Common.Code;
using Core.Common.Exceptions;
using Core.Common.Projections;
using Core.Group;
using Core.Group.Events;
using Infrastructure.EventStore.Repository;
using Infrastructure.Projections.InternalProjections.Repository;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CommandModel.Group.Commands
{
    public record GenerateJoinGroupCode(DateTime ValidTo, Guid GroupId, Core.User.User User)
        : IRequest;

    public class GenerateJoinGroupCodeHandler : IRequestHandler<GenerateJoinGroupCode>
    {
        private readonly IGroupService _groupService;
        private readonly IIndexProjectionRepository _indexProjectionRepository;

        public GenerateJoinGroupCodeHandler(
            IGroupService groupService,
            [FromKeyedServices(InternalProjectionName.GroupCodeIndex)]
                IIndexProjectionRepository indexProjectionRepository
        )
        {
            _groupService = groupService;
            _indexProjectionRepository = indexProjectionRepository;
        }

        public async Task Handle(GenerateJoinGroupCode request, CancellationToken cancellationToken)
        {
            var group = await _groupService.FindOneAsync(request.GroupId, cancellationToken);

            if (!group.UsersIds.Contains(request.User.Id))
            {
                throw new ForbiddenException();
            }

            var code = await GenerateCode(request, cancellationToken);

            var @event = new GroupCodeGenerated(group.Id, code);

            await _groupService.AppendAsync(group.Id, @event, cancellationToken);
        }

        private async Task<Code<GroupCodeType>> GenerateCode(
            GenerateJoinGroupCode request,
            CancellationToken cancellationToken
        )
        {
            var attempts = 0;
            do
            {
                try
                {
                    if (attempts > 500)
                    {
                        throw new InvalidOperationException(
                            "Unable to generate a unique code after several attempts."
                        );
                    }

                    var code = new Code<GroupCodeType>(GroupCodeType.Join, request.ValidTo);

                    await _indexProjectionRepository.CheckAvailibility(
                        code.Value,
                        cancellationToken
                    );
                    return code;
                }
                catch (ConflictException ex)
                {
                    attempts++;
                }
            } while (true);
        }
    }
}
