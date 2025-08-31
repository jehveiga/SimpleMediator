# SimpleMediator

A lightweight and straightforward mediator implementation for .NET applications, facilitating in-process messaging with minimal setup.

## Samples

You can find complete example projects demonstrating how to use the SimpleMediator in the [`/samples`](./samples) folder.

These include:

- ✅ Basic usage with `Send` and `Publish`
- ✅ Modular application structure
- ✅ Manual and automatic registration of handlers

Feel free to explore and run them to see how the mediator works in different scenarios.

## Getting Started

### Using Contracts-Only Package

To reference only the contracts for SimpleMediator, which includes:

- `IRequest` (including generic variants)
  - Represents a command or query that expects a single response
- `INotification`
  - Represents an event broadcast to multiple handlers (if any)

### Advanced Usage: Request + Notification

This example demonstrates how to combine a `Request` (command/query) and a `Notification` (event) in a real-world use case.

> ✅ This scenario uses only `Microsoft.Extensions.DependencyInjection.Abstractions` for DI registration — no framework-specific packages.

---

#### 1. Define the Request and Notification

```csharp
public class CreateCustomerCommand : IRequest<string>
{
    public string Name { get; set; }
}

public class CustomerCreatedEvent : INotification
{
    public Guid CustomerId { get; }

    public CustomerCreatedEvent(Guid customerId)
    {
        CustomerId = customerId;
    }
}
```

---

#### 2. Implement the Handlers

```csharp
public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, string>
{
    private readonly IMediator _mediator;

    public CreateCustomerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        // Simulate persistence...

        // Publish event
        await _mediator.Publish(new CustomerCreatedEvent(id), cancellationToken);

        return $"Customer '{request.Name}' created with ID {id}";
    }
}

public class SendWelcomeEmailHandler : INotificationHandler<CustomerCreatedEvent>
{
    public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending welcome email to customer {notification.CustomerId}");
        return Task.CompletedTask;
    }
}
```

---

#### 3. Register the Handlers (Dependency Injection)

You can register everything manually if you want full control:

```csharp
services.AddSingleton<IMediator, Mediator>();

services.AddTransient<IRequestHandler<CreateCustomerCommand, string>, CreateCustomerHandler>();
services.AddTransient<INotificationHandler<CustomerCreatedEvent>, SendWelcomeEmailHandler>();
```

Or use assembly scanning with:

```csharp
services.AddSimpleMediator();
```

---

#### 4. Execute the Flow

```csharp
public class CustomerAppService
{
    private readonly IMediator _mediator;

    public CustomerAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> CreateCustomer(string name)
    {
        return await _mediator.Send(new CreateCustomerCommand { Name = name });
    }
}
```

---

When the `CreateCustomer` method is called:

1. `CreateCustomerHandler` handles the request
2. It creates and persists the customer (simulated)
3. It publishes a `CustomerCreatedEvent`
4. `SendWelcomeEmailHandler` handles the event

This structure cleanly separates **commands** (which change state and return a result) from **notifications** (which communicate to the rest of the system that something happened).

## Features

- **Lightweight**: Minimal dependencies and straightforward setup.
- **In-Process Messaging**: Facilitates in-process communication between components.
- **Handler Registration**: Automatically registers handlers from specified assemblies.



