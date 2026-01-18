using CsvProcessor.Services;

[TestClass]
public sealed class MedianCalcTest
{
    [TestMethod]
    public void CalculateMedian_OddNumberOfElements_ReturnsMiddleElement()
    {
        // Arrange
        var oddNumbers = new double[] { 1, 3, 5 };

        // Act
        var result = Validations.CalculateMedian(oddNumbers);

        // Assert
        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public void CalculateMedian_EvenNumberOfElements_ReturnsAverageOfTwoMiddleElements()
    {
        // Arrange
        var evenNumbers = new double[] { 1, 2, 3, 4 };

        // Act
        var result = Validations.CalculateMedian(evenNumbers);

        // Assert
        Assert.AreEqual(2.5, result);
    }

    [TestMethod]
    public void CalculateMedian_EmptyArray_ReturnsZero()
    {
        // Arrange
        var emptyArray = Array.Empty<double>();

        // Act
        var result = Validations.CalculateMedian(emptyArray);

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void CalculateMedian_SingleElement_ReturnsThatElement()
    {
        // Arrange
        var singleElementArray = new double[] { 10 };

        // Act
        var result = Validations.CalculateMedian(singleElementArray);

        // Assert
        Assert.AreEqual(10, result);
    }
}
