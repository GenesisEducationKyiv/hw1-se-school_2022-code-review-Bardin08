using EmailValidation;
using Xunit;

namespace UnitTests.Validators;

public class EmailValidatorTests
{
    [Theory]
    [InlineData("valid@email.com")]
    [InlineData("also.valid@email.com")]
    [InlineData("one.more_valid@email.com")]
    public void ValidEmail_ReturnsTrue(string email)
    {
        Assert.True(EmailValidator.Validate(email));
    }
    
    [Theory]
    [InlineData("invalid_email")]
    [InlineData("invalid2135email")]
    [InlineData("invalid2135.email.com")]
    [InlineData("2..invalid@email.com")]
    public void InvalidEmail_ReturnsFalse(string email)
    {
        Assert.False(EmailValidator.Validate(email));
    }
}