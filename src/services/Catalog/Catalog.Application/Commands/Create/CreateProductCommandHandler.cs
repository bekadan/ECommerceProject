using Caching.Core.Abstractions;
using Catalog.Application.DTOs;
using Catalog.Application.Repositories;
using Catalog.Domain.Factories;
using Core.Events.Dispatching;
using Core.Exceptions.Types;
using Core.Logging.Abstractions;
using Core.Validation.Abstractions;
using MediatR;
using Microsoft.Extensions.Azure;
using Shared.Core;
using Shared.Core.Primitives;

namespace Catalog.Application.Commands.Create;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IProductAggregateRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IEventDispatcher _eventPublisher;
    private readonly ILogger _logger;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductCommandHandler(
        IProductAggregateRepository repository,
        ICacheService cacheService,
        IEventDispatcher eventPublisher,
        ILogger logger,
        IValidator<CreateProductCommand> validator)
    {
        _repository = repository;
        _cacheService = cacheService;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _validator = validator;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.Information("Starting creation of product: {ProductName}", command.Name);

        // 1️⃣ Validate command
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (validationResult.IsFailure)
        {
            _logger.Warning("Validation failed for product {ProductName}: {Error}", command.Name, validationResult.Error.Message);
            return Result.Failure<ProductDto>(validationResult.Error);
        }

        try
        {
            // 2️⃣ Create aggregate using factory
            var aggregate = ProductAggregateFactory.CreateNew(
                command.Name,
                command.Price,
                command.Currency,
                command.CategoryId,
                command.Stock,
                command.CategoryName
            );

            // 3️⃣ Persist aggregate
            await _repository.AddAsync(aggregate, cancellationToken);

            // 4️⃣ Publish domain events
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                await _eventPublisher.DispatchAsync(domainEvent, cancellationToken);
            }

            // 5️⃣ Update cache
            await _cacheService.SetAsync(aggregate.Product.Name.Value, cancellationToken);

            _logger.Information("Product {ProductId} created successfully", aggregate.Product.Id);

            // 6️⃣ Map to DTO
            var dto = new ProductDto
            {
                Id = aggregate.Product.Id,
                Name = aggregate.Product.Name.Value,
                Price = aggregate.Product.Price.Amount,
                Currency = aggregate.Product.Price.Currency,
                Stock = aggregate.Product.Stock,
                CategoryId = aggregate.Product.CategoryId,
                CategoryName = aggregate.Product.Category?.Name
            };

            return Result.Success(dto);
        }
        catch (DomainException ex)
        {
            _logger.Error(ex, "Domain error while creating product {ProductName}", command.Name);
            return Result.Failure<ProductDto>(Error.Create(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Unexpected error while creating product {ProductName}", command.Name);
            return Result.Failure<ProductDto>(Error.Create("An unexpected error occurred."));
        }
    }
}
