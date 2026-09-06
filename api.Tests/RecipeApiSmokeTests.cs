using Xunit;
using System;
namespace RecipeApi.Tests;

public sealed class RecipeApiSmokeTests
{
    [Fact]
    public void Recipe_summary_preserves_optional_description()
    {
        var recipe = new RecipeSummary(Guid.NewGuid(), "Tomato soup", null);

        Assert.Equal("Tomato soup", recipe.Title);
        Assert.Null(recipe.Description);
    }
}

