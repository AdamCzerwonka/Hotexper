﻿using Hotexper.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Hotexper.Api.DTOs;

public class UserResponse
{
    public Guid Id { get; init; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;

    public IEnumerable<string>? Roles { get; set; }

    public static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email!,
            Username = user.UserName!
        };
    }
}