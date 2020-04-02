using System.Collections.Generic;
using System.Linq;
using JsonApiDotNetCore.Internal;
using JsonApiDotNetCore.Query;
using Microsoft.Extensions.Primitives;
using UnitTests.TestModels;
using Xunit;

namespace UnitTests.QueryParameters
{
    public sealed class IncludeServiceTests : QueryParametersUnitTestCollection
    {
        public IncludeService GetService(ResourceContext resourceContext = null)
        {
            return new IncludeService(_resourceGraph, MockCurrentRequest(resourceContext ?? _articleResourceContext));
        }

        [Fact]
        public void CanParse_FilterService_SucceedOnMatch()
        {
            // Arrange
            var filterService = GetService();

            // Act
            bool result = filterService.CanParse("include");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CanParse_FilterService_FailOnMismatch()
        {
            // Arrange
            var filterService = GetService();

            // Act
            bool result = filterService.CanParse("includes");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Parse_MultipleNestedChains_CanParse()
        {
            // Arrange
            const string chain = "author.blogs.reviewer.favoriteFood,reviewer.blogs.author.favoriteSong";
            var query = new KeyValuePair<string, StringValues>("include", new StringValues(chain));
            var service = GetService();

            // Act
            service.Parse(query.Key, query.Value);

            // Assert
            var chains = service.Get();
            Assert.Equal(2, chains.Count);
            var firstChain = chains[0];
            Assert.Equal("author", firstChain.First().PublicRelationshipName);
            Assert.Equal("favoriteFood", firstChain.Last().PublicRelationshipName);
            var secondChain = chains[1];
            Assert.Equal("reviewer", secondChain.First().PublicRelationshipName);
            Assert.Equal("favoriteSong", secondChain.Last().PublicRelationshipName);
        }

        [Fact]
        public void Parse_ChainsOnWrongMainResource_ThrowsJsonApiException()
        {
            // Arrange
            const string chain = "author.blogs.reviewer.favoriteFood,reviewer.blogs.author.favoriteSong";
            var query = new KeyValuePair<string, StringValues>("include", new StringValues(chain));
            var service = GetService(_resourceGraph.GetResourceContext<Food>());

            // Act, assert
            var exception = Assert.Throws<JsonApiException>( () => service.Parse(query.Key, query.Value));
            Assert.Contains("Invalid", exception.Message);
        }

        [Fact]
        public void Parse_NotIncludable_ThrowsJsonApiException()
        {
            // Arrange
            const string chain = "cannotInclude";
            var query = new KeyValuePair<string, StringValues>("include", new StringValues(chain));
            var service = GetService();

            // Act, assert
            var exception = Assert.Throws<JsonApiException>(() => service.Parse(query.Key, query.Value));
            Assert.Contains("not allowed", exception.Message);
        }

        [Fact]
        public void Parse_NonExistingRelationship_ThrowsJsonApiException()
        {
            // Arrange
            const string chain = "nonsense";
            var query = new KeyValuePair<string, StringValues>("include", new StringValues(chain));
            var service = GetService();

            // Act, assert
            var exception = Assert.Throws<JsonApiException>(() => service.Parse(query.Key, query.Value));
            Assert.Contains("Invalid", exception.Message);
        }

        [Fact]
        public void Parse_EmptyChain_ThrowsJsonApiException()
        {
            // Arrange
            const string chain = "";
            var query = new KeyValuePair<string, StringValues>("include", new StringValues(chain));
            var service = GetService();

            // Act, assert
            var exception = Assert.Throws<JsonApiException>(() => service.Parse(query.Key, query.Value));
            Assert.Contains("Include parameter must not be empty if provided", exception.Message);
        }
    }
}
