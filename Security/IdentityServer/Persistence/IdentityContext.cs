using IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Persistence;

public sealed class IdentityContext(DbContextOptions<IdentityContext> options)
    : IdentityDbContext<ApplicationUser>(options);

