using System.Globalization;
using FluentValidation;
using FluentValidation.Results;
using GClaims.BuildingBlocks.Core.Common;
using GClaims.BuildingBlocks.Core.Mediator;
using GClaims.BuildingBlocks.Core.Messages.CommonMessages.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GClaims.BuildingBlocks.Core.Messages
{
    public abstract class Command : ICommandMessage
    {
        public IMediatorHandler MediatorHandler { get; set; }

        public string MessageType
        {
            get
            {
                return GetType().Name;
            }
        }

        public dynamic AggregateId { get; set; }

        public DateTime Timestamp { get; set; }

        public ValidationResult ValidationResult { get; set; }

        public object Data { get; set; }
        

        public ILogger Logger { protected get; set; }
        protected Command()
        {
            Timestamp = DateTime.Now;
        }

    }

    public abstract class Command<TResponse> : Command, ICommandMessage<TResponse>
    {
        protected Command()
        {
            Timestamp = DateTime.Now;
        }
    }

    public abstract class Command<TRequest, TResponse> : Command, ICommandMessage<TRequest, TResponse>, IInputRequest<TRequest>
        where TRequest : IRequest<TResponse>
    {
        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public TRequest Input { get; set; }
    }

    public abstract class Command<TRequest, TResponse, TValidator, TInput> : Command, ICommandMessage<TRequest, TResponse, TValidator, TInput>
        where TRequest : IBaseRequest, IRequest<TResponse>
        where TInput : class, new()
        where TValidator : AbstractValidator<TRequest>, new()
    {
        protected Command()
        {
            Timestamp = DateTime.Now;
        }

        public async Task<bool> Validate(IBaseRequest request)
        {
            if (request == null)
            {
                ValidationResult = new TValidator().Validate((TRequest)request!);
            }


            if (ValidationResult is null)
            {
                throw new Exception("Erro ao efetuar validação!");
            }
            var isValid = ValidationResult.IsValid && ValidationResult.Errors?.Count == 0;

            if (!isValid)
            {
                foreach (var error in ValidationResult.Errors!)
                {
                    await MediatorHandler.PublishNotification(new DomainNotification(nameof(ValidationResult),
                        error.ErrorMessage));
                }
            }

            return isValid;
        }

    }

    public abstract class CommandHandler<TRequest> : Command, ICommandHandler<TRequest>
       where TRequest : IRequest<Unit>
    {
        protected CommandHandler(IMediatorHandler mediatorHandler)
        {
            MediatorHandler = mediatorHandler;
        }

        protected abstract Task<Unit> Execute(TRequest request, CancellationToken cancellationToken);

        public async Task<Unit> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await Execute(request, cancellationToken);
        }

    }

    public abstract class CommandHandler<TRequest, TResponse> : Command, ICommandHandler<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
    {

        protected CommandHandler(IMediatorHandler mediatorHandler)
        {
            MediatorHandler = mediatorHandler;
        }

        protected abstract Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken);

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            return await Execute(request, cancellationToken);
        }

    }
}
