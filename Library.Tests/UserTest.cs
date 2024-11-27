using System;
using System.Collections.Generic;
using Library.Core.Entities;
using Xunit;

namespace Library.Tests
{
    public class UserTest
    {
        [Fact]
        public void CanCreateUser()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            // Act & Assert
            Assert.NotNull(user);
            Assert.Equal("testuser", user.UserName);
            Assert.Equal("hashedpassword", user.PasswordHash);
            Assert.Equal("testuser@example.com", user.Email);
            Assert.Equal(UserRole.User, user.Role);
            Assert.Empty(user.BookPicks);
            Assert.NotEqual(default(DateTime), user.RefreshTokenExpiryTime);
        }

        [Fact]
        public void CanUpdateUserDetails()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            // Act
            user.UserName = "updateduser";
            user.Email = "updateduser@example.com";
            user.Role = UserRole.Admin;

            // Assert
            Assert.Equal("updateduser", user.UserName);
            Assert.Equal("updateduser@example.com", user.Email);
            Assert.Equal(UserRole.Admin, user.Role);
        }

        [Fact]
        public void CanAddBookPicksToUser()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            var bookPick1 = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = Guid.NewGuid()
            };

            var bookPick2 = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = Guid.NewGuid()
            };

            // Act
            user.BookPicks.Add(bookPick1);
            user.BookPicks.Add(bookPick2);

            // Assert
            Assert.Equal(2, user.BookPicks.Count);
            Assert.Contains(bookPick1, user.BookPicks);
            Assert.Contains(bookPick2, user.BookPicks);
        }

        [Fact]
        public void CanRemoveBookPicksFromUser()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            var bookPick1 = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = Guid.NewGuid()
            };

            var bookPick2 = new BookPick
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                BookId = Guid.NewGuid()
            };

            user.BookPicks.Add(bookPick1);
            user.BookPicks.Add(bookPick2);

            // Act
            user.BookPicks.Remove(bookPick1);

            // Assert
            Assert.Single(user.BookPicks);
            Assert.DoesNotContain(bookPick1, user.BookPicks);
            Assert.Contains(bookPick2, user.BookPicks);
        }

        [Fact]
        public void CanSetRefreshToken()
        {
            // Arrange
            var user = new User(
                id: Guid.NewGuid(),
                userName: "testuser",
                passwordHash: "hashedpassword",
                email: "testuser@example.com",
                role: UserRole.User
            );

            // Act
            user.RefreshToken = "newrefreshtoken";
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(14);

            // Assert
            Assert.Equal("newrefreshtoken", user.RefreshToken);
            Assert.True(user.RefreshTokenExpiryTime > DateTime.UtcNow);
        }
    }
}