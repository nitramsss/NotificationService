using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore;
using NotificationService.Model;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDb>(opt => opt.UseInMemoryDatabase("InMem"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
     app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// create user for notification reference
app.MapPost("/notification/createuser", async (User user, AppDb db) =>
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
    
        return Results.Created($"/notification/{user.Id}", user);
    });

// get all notification of a specific user
app.MapGet("/notification", async (int userId, AppDb db) => 
{
    var notifications = await db.Notifications
    .Where(n => n.UserId == userId)
    .ToListAsync();

    return notifications is not null ? Results.Ok(notifications) : Results.NotFound();
});

// get specific notification of a specific user
app.MapGet("notification/{userId}/{notifId}", 
    async (int userId, int notifId,  AppDb db) =>
{
    var notification = await db.Notifications
        .Where(n => n.Id == notifId && n.UserId == userId)
        .FirstOrDefaultAsync();

    return notification is not null ? Results.Ok(notification) : Results.NotFound();
});

// send notification to all user
app.MapPost("/notification/send", async(Notification message, AppDb db) =>
{
    var users = await db.Users.ToListAsync();

    foreach (var user in users)
    {
        var userNotification = new Notification
        {
            Title = message.Title,
            Body = message.Body,
            Status = "Sent",
            UserId = user.Id,
            IsRead = false,            
            SentAt = DateTime.UtcNow
        };

        db.Notifications.Add(userNotification);
    }

    await db.SaveChangesAsync();

    return Results.Ok("Notification sent to all users.");
});

// send notification to specific user
app.MapPost("/notification/send/{userId}", async (int userId, Notification message, AppDb db) =>
{
    var userNotification = new Notification
    {
        Title = message.Title,
        Body = message.Body,
        Status = "Sent",
        UserId = userId,
        IsRead = false,            
        SentAt = DateTime.UtcNow
    };
    db.Notifications.Add(userNotification);
    
    await db.SaveChangesAsync();
    return userNotification is not null ? Results.Ok(userNotification) : Results.NotFound();
});

// delete all notification of specific user
app.MapDelete("/notification/delete/{userId}", async (int userId, AppDb db) =>
{
    var notifications = await db.Notifications
        .Where(n => n.UserId == userId)
        .ToListAsync();
    
    if (notifications.Count > 0)
    {
        db.Notifications.RemoveRange(notifications);
        await db.SaveChangesAsync();
    }

    return Results.NoContent();
});

// edit user information



// delete specific notification of specific user
app.MapDelete("/notification/{notifId}/{userId}", async (int userId, int notifId, AppDb db) =>
{
    var notification = await db.Notifications
        .SingleOrDefaultAsync(n => n.UserId == userId && n.Id == notifId);

    if (notification is not null)
    {
        db.Notifications.Remove(notification);
        await db.SaveChangesAsync();
        return Results.NoContent();     
    }

    return Results.NotFound();
});

app.Run();