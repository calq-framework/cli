using CalqFramework.Extensions.System;

namespace CalqFramework.DataAccessTest;

public class TypeExtensionsTest {

    [Fact]
    public void Parse_PrimitiveInt_ReturnsCorrectValue() {
        var result = typeof(int).Parse<int>("42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void Parse_DateOnly_ReturnsCorrectValue() {
        var result = typeof(DateOnly).Parse<DateOnly>("2024-01-15");
        Assert.Equal(new DateOnly(2024, 1, 15), result);
    }

    [Fact]
    public void Parse_TimeOnly_ReturnsCorrectValue() {
        var result = typeof(TimeOnly).Parse<TimeOnly>("14:30:00");
        Assert.Equal(new TimeOnly(14, 30, 0), result);
    }

    [Fact]
    public void Parse_Guid_ReturnsCorrectValue() {
        var guidString = "12345678-1234-1234-1234-123456789abc";
        var result = typeof(Guid).Parse<Guid>(guidString);
        Assert.Equal(Guid.Parse(guidString), result);
    }

    [Fact]
    public void Parse_DateTimeOffset_ReturnsCorrectValue() {
        var result = typeof(DateTimeOffset).Parse<DateTimeOffset>("2024-01-15T14:30:00+00:00");
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 14, 30, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void IsParsable_DateOnly_ReturnsTrue() {
        Assert.True(typeof(DateOnly).IsParsable());
    }

    [Fact]
    public void IsParsable_TimeOnly_ReturnsTrue() {
        Assert.True(typeof(TimeOnly).IsParsable());
    }

    [Fact]
    public void IsParsable_Guid_ReturnsTrue() {
        Assert.True(typeof(Guid).IsParsable());
    }

    [Fact]
    public void IsParsable_Int_ReturnsTrue() {
        Assert.True(typeof(int).IsParsable());
    }

    [Fact]
    public void Parse_InvalidFormat_ThrowsFormatException() {
        Assert.Throws<FormatException>(() => typeof(DateOnly).Parse<DateOnly>("not-a-date"));
    }

    [Fact]
    public void IsParsable_NullableInt_ReturnsTrue() {
        Assert.True(typeof(int?).IsParsable());
    }

    [Fact]
    public void IsParsable_NullableDateOnly_ReturnsTrue() {
        Assert.True(typeof(DateOnly?).IsParsable());
    }

    [Fact]
    public void Parse_NullableInt_ReturnsCorrectValue() {
        var result = typeof(int?).Parse("42");
        Assert.Equal(42, result);
    }

    [Fact]
    public void IsParsable_Enum_ReturnsTrue() {
        Assert.True(typeof(DayOfWeek).IsParsable());
    }

    [Fact]
    public void IsParsable_NullableEnum_ReturnsTrue() {
        Assert.True(typeof(DayOfWeek?).IsParsable());
    }

    [Fact]
    public void Parse_Enum_ReturnsCorrectValue() {
        var result = typeof(DayOfWeek).Parse<DayOfWeek>("Monday");
        Assert.Equal(DayOfWeek.Monday, result);
    }

    [Fact]
    public void Parse_EnumCaseInsensitive_ReturnsCorrectValue() {
        var result = typeof(DayOfWeek).Parse<DayOfWeek>("monday");
        Assert.Equal(DayOfWeek.Monday, result);
    }
}
