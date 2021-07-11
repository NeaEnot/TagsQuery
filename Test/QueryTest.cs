using TagsQuery;
using Xunit;

namespace Test
{
    public class QueryTest
    {
        [Fact]
        public void TestSingleToken()
        {
            Query query = new Query("\"tag\"");
            Assert.True(query.Validate("\"tag\""));
            Assert.False(query.Validate("\"not tag\""));
        }

        [Fact]
        public void TestOperatorNot()
        {
            Query query = new Query("-\"tag\"");
            Assert.False(query.Validate("\"tag\""));
            Assert.True(query.Validate("\"not tag\""));
        }

        [Fact]
        public void TestOperatorAnd()
        {
            Query query = new Query("\"tag1\" & \"tag 2\"");
            Assert.True(query.Validate("\"tag1\" \"tag 2\" \"some tag\""));
            Assert.False(query.Validate("\"tag1\""));
        }
    }
}
