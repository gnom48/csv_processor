using CsvProcessor.Exceprtons;
using CsvProcessor.Services;

[TestClass]
public sealed class CsvValidationTest
{
    [TestMethod]
    public void MapAndThrowIfInvalid_ValidInput_ReturnsCorrectValueObject()
    {
        // Arrange
        var validRow = new[] { "2023-10-06T17:00:00.000Z", "1.9", "48.3" };

        // Act
        var result = Validations.MapAndThrowIfInvalid(validRow);

        // Assert
        Assert.AreEqual(new DateTime(2023, 10, 6, 17, 0, 0, DateTimeKind.Utc), result.Date);
        Assert.AreEqual(1.9, result.ExecutionTime);
        Assert.AreEqual(48.3, result.PointerValue);
    }

    [TestMethod]
    public void MapAndThrowIfInvalid_IncorrectNumberOfColumns_ThrowsValidationException()
    {
        // Arrange
        var invalidRow = new[] { "2023-10-06T17:00:00.000Z", "1.9" }; // Недостаточно колонок

        // Act & Assert
        Assert.ThrowsException<ValidationException>(() => Validations.MapAndThrowIfInvalid(invalidRow));
    }

    [TestMethod]
    public void MapAndThrowIfInvalid_EmptyFields_ThrowsValidationException()
    {
        // Arrange
        var emptyFieldRow = new[] { "", "", "" };

        // Act & Assert
        Assert.ThrowsException<ValidationException>(() => Validations.MapAndThrowIfInvalid(emptyFieldRow));
    }

    [TestMethod]
    public void MapAndThrowIfInvalid_InvalidDateFormat_ThrowsValidationException()
    {
        // Arrange
        var invalidDateRow = new[] { "2023-10-06T17:00", "1.9", "48.3" }; // Неправильный формат даты

        // Act & Assert
        Assert.ThrowsException<ValidationException>(() => Validations.MapAndThrowIfInvalid(invalidDateRow));
    }

    [TestMethod]
    public void MapAndThrowIfInvalid_NegativeExecutionTime_ThrowsValidationException()
    {
        // Arrange
        var negativeExecTimeRow = new[] { "2023-10-06T17:00:00.000Z", "-1.9", "48.3" };

        // Act & Assert
        Assert.ThrowsException<ValidationException>(() => Validations.MapAndThrowIfInvalid(negativeExecTimeRow));
    }

    [TestMethod]
    public void MapAndThrowIfInvalid_NegativePointerValue_ThrowsValidationException()
    {
        // Arrange
        var negativeValueRow = new[] { "2023-10-06T17:00:00.000Z", "1.9", "-48.3" };

        // Act & Assert
        Assert.ThrowsException<ValidationException>(() => Validations.MapAndThrowIfInvalid(negativeValueRow));
    }
}