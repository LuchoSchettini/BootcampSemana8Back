namespace Store.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Add_ReturnCorrectValue()
        {
            //arrange
            var a = 1;
            var b = 2;
            var c = a + b;

            //act


            //Assert
            Assert.Equal(3, c);

        }

        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(2, 2, 4)]
        public void Add_MultipleParamas_ReturnCorrectValue(int a, int b, int expected)
        {
            //arrange
            var c = a + b;

            //act


            //Assert
            Assert.Equal(expected, c);

         }
    }
}