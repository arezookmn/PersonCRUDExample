namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            MyMath myMath = new MyMath();
            int input1 = 1, input2 = 5;
            int expectedValue = 6;
            //Acts
            int actual = myMath.Add(input1, input2);
            //Assert
            Assert.Equal(actual, expectedValue);
        }
    }
}