﻿using Xunit;
using FluentAssertions;
using CSULB.GetUsGrub.UserAccessControl;

namespace CSULB.GetUsGrub.UnitTests
{
    /// <summary>
    /// Unit testing for the Claims Factory.
    /// 
    /// @author: Rachel Dang
    /// @updated: 04/07/2018
    /// </summary>
    public class ClaimsFactoryUnitTests
    {
        // Arrange
        ClaimsFactory factory = new ClaimsFactory();

        [Fact]
        public void Should_ReturnIndividualClaims_Given_IndividualAccountType()
        {
            // Arrange
            var accountType = AccountTypes.Individual;

            // Act
            var result = factory.Create(accountType);
            var expected = new IndividualUser().Claims;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_ReturnRestaurantClaims_Given_RestaurantAccountType()
        {
            // Arrange
            var accountType = AccountTypes.Restaurant;

            // Act
            var result = factory.Create(accountType);
            var expected = new RestaurantUser().Claims;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_ReturnAdminClaims_Given_AdminAccountType()
        {
            // Arrange
            var accountType = AccountTypes.Admin;

            // Act
            var result = factory.Create(accountType);
            var expected = new AdminUser().Claims;

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Should_ReturnEmptyClaims_Given_InvalidAccountType()
        {
            // Arrange
            var accountType = "InvalidAccountType";

            // Act
            var result = factory.Create(accountType);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnEmptyClaims_Given_Null()
        {
            // Act
            var result = factory.Create(null);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Should_ReturnEmptyClaims_Given_EmptyType()
        {
            // Arrange
            var accountType = "";

            // Act
            var result = factory.Create(accountType);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
