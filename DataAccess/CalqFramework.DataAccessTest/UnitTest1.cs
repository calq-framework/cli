using CalqFramework.DataAccess.Parsing;

namespace CalqFramework.DataAccessTest;

public class StringParserTest {
    private readonly StringParser _parser = new();

    [Fact]
    public void ParseValue_PrimitiveInt_ReturnsCorrectValue() {
        var result = _parser.ParseValue<int>("42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void ParseValue_DateOnly_ReturnsCorrectValue() {
        var result = _parser.ParseValue<DateOnly>("2024-01-15");
        Assert.Equal(new DateOnly(2024, 1, 15), result);
    }

    [Fact]
    public void ParseValue_TimeOnly_ReturnsCorrectValue() {
        var result = _parser.ParseValue<TimeOnly>("14:30:00");
        Assert.Equal(new TimeOnly(14, 30, 0), result);
    }

    [Fact]
    public void ParseValue_Guid_ReturnsCorrectValue() {
        var guidString = "12345678-1234-1234-1234-123456789abc";
        var result = _parser.ParseValue<Guid>(guidString);
        Assert.Equal(Guid.Parse(guidString), result);
    }

    [Fact]
    public void ParseValue_DateTimeOffset_ReturnsCorrectValue() {
        var result = _parser.ParseValue<DateTimeOffset>("2024-01-15T14:30:00+00:00");
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 14, 30, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void IsParsable_DateOnly_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(DateOnly)));
    }

    [Fact]
    public void IsParsable_TimeOnly_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(TimeOnly)));
    }

    [Fact]
    public void IsParsable_Guid_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(Guid)));
    }

    [Fact]
    public void IsParsable_Int_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(int)));
    }

    [Fact]
    public void ParseValue_InvalidFormat_ThrowsFormatException() {
        Assert.Throws<FormatException>(() => _parser.ParseValue<DateOnly>("not-a-date"));
    }

    [Fact]
    public void IsParsable_NullableInt_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(int?)));
    }

    [Fact]
    public void IsParsable_NullableDateOnly_ReturnsTrue() {
        Assert.True(_parser.IsParsable(typeof(DateOnly?)));
    }

    [Fact]
    public void ParseValue_NullableInt_ReturnsCorrectValue() {
        var result = _parser.ParseValue("42", typeof(int?));
        Assert.Equal(42, result);
    }
}
