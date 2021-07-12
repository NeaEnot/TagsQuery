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

        [Fact]
        public void TestManyOperatorsAnd()
        {
            Query query = new Query("\"tag1\" & \"tag 2\" & \"tag 3\"");
            Assert.True(query.Validate("\"tag1\" \"tag 2\" \"tag 3\""));
            Assert.False(query.Validate("\"tag1\" \"tag 2\" \"some tag\""));
            Assert.False(query.Validate("\"tag1\""));
            Assert.False(query.Validate("\"tag3\""));
        }

        [Fact]
        public void TestOperatorOr()
        {
            Query query = new Query("\"tag1\" | \"tag 2\"");
            Assert.True(query.Validate("\"tag1\" \"tag 2\" \"some tag\""));
            Assert.True(query.Validate("\"tag1\""));
            Assert.False(query.Validate("\"tag3\""));
        }

        [Fact]
        public void TestManyOperatorsOr()
        {
            Query query = new Query("\"tag1\" | \"tag 2\" | \"tag 3\"");
            Assert.True(query.Validate("\"tag1\" \"tag 2\" \"some tag\""));
            Assert.True(query.Validate("\"tag1\""));
            Assert.True(query.Validate("\"tag 3\""));
            Assert.False(query.Validate("\"tag4\""));
        }

        [Fact]
        public void TestManyOperators()
        {
            Query query = new Query("\"tag1\" | \"tag2\" & -\"tag3\"");
            Assert.True(query.Validate("\"tag1\" \"tag2\" \"tag3\""));
            Assert.False(query.Validate("\"tag2\" \"tag3\""));
            Assert.True(query.Validate("\"tag1\" \"tag3\""));
            Assert.True(query.Validate("\"tag1\" \"tag2\""));
            Assert.True(query.Validate("\"tag1\""));
            Assert.True(query.Validate("\"tag2\""));
            Assert.False(query.Validate("\"tag3\""));
            Assert.True(query.Validate("\"tag4\" \"tag2\""));
        }

        [Fact]
        public void TestBrackets()
        {
            Query query = new Query("\"tag1\" & (\"tag2\" | \"tag3\")");
            Assert.True(query.Validate("\"tag1\" \"tag2\""));
            Assert.True(query.Validate("\"tag1\" \"tag3\""));
            Assert.False(query.Validate("\"tag2\" \"tag3\""));
            Assert.False(query.Validate("\"tag1\" \"tag-\""));
        }

        [Fact]
        public void TestNestedBrackets()
        {
            Query query = new Query("\"tag1\" & (\"tag2\" | -(-\"tag3\"))");
            Assert.True(query.Validate("\"tag1\" \"tag2\""));
            Assert.True(query.Validate("\"tag1\" \"tag3\""));
            Assert.False(query.Validate("\"tag2\" \"tag3\""));
            Assert.False(query.Validate("\"tag1\" \"tag-\""));
        }
    }
}
