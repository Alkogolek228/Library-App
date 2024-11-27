using Library.Application.Interfaces.Auth;
using Library.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(LibraryDbContext context, IPasswordHasher hasher)
        {
            if (context.Authors.Any() || context.Books.Any())
            {
                return; 
            }
            context.Database.EnsureCreated();
            var users = context.Users.ToList();  
            context.Users.RemoveRange(users);   
            context.SaveChanges();
            var authors = new List<Author>
            {
                new Author
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    DateOfBirth = DateTime.UtcNow,
                    Country = "USA",
                    Books = new List<Book>
                    {
                        new Book
                        {
                            Id = Guid.NewGuid(),
                            ISBN = "9783161484100", 
                            Title = "Sample Book 1",
                            Genre = "Fiction",
                            Description = "A sample book description.",
                            BorrowedOn = null,
                            ReturnBy = null,
                            ImagePath = null
                        },
                        new Book
                        {
                            Id = Guid.NewGuid(),
                            ISBN = "9783161484101",
                            Title = "Sample Book 2",
                            Genre = "Non-Fiction",
                            Description = "Another sample book description.",
                            BorrowedOn = null,
                            ReturnBy = null,
                            ImagePath = null
                        }
                    }
                }
            };
            if (!context.Users.Any())
            {
                var testUser = User.Create(Guid.NewGuid(), "TestUser", hasher.GenerateHash("123qwe"), "test@mail.com", UserRole.User);
                var testAdmin = User.Create(Guid.NewGuid(), "TestAdmin", hasher.GenerateHash("123qwe"), "test_admin@mail.com", UserRole.Admin);
                await context.Users.AddAsync(testUser);
                await context.Users.AddAsync(testAdmin);
                await context.SaveChangesAsync();
            }
            context.Authors.AddRange(authors);
            await context.SaveChangesAsync();
        }
    }
}