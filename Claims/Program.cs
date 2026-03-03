using Claims;
using Claims.Application.Interfaces;
using Claims.Application.Services;
using Claims.Domain.Events;
using Claims.Infrastructure.Data;
using Claims.Infrastructure.Events;
using Claims.Infrastructure.Handlers;
using Claims.Infrastructure.Queue;
using Claims.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContexts
builder.Services.AddDbContext<ClaimsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ClaimsConnection")));

builder.Services.AddDbContext<AuditContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuditConnection")));

// Repositories
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ICoverRepository, CoverRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();

// Services
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();

builder.Services.AddScoped<IEventDispatcher, InMemoryEventDispatcher>();

// Queue
builder.Services.AddSingleton<IBackgroundQueue, BackgroundQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

// Event Handlers
builder.Services.AddScoped<IEventHandler<ClaimCreatedEvent>, ClaimCreatedAuditHandler>();
builder.Services.AddScoped<IEventHandler<ClaimDeletedEvent>, ClaimDeletedAuditHandler>();
builder.Services.AddScoped<IEventHandler<CoverCreatedEvent>, CoverCreatedAuditHandler>();
builder.Services.AddScoped<IEventHandler<CoverDeletedEvent>, CoverDeletedAuditHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ValidationMiddleware>();
app.MapControllers();
app.Run();