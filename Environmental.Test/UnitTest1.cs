using System.Reflection;

namespace Environmental.Test
{
    public class Tests
    {
        [Environmental]
        class TestClass
        {
            [Env(Name = "test_int", Required = true)]
            public int SomeInt { get; set; }
            [Env(Name = "test_float", Required = true)]
            public float SomeFloat { get; set; }
            [Env(Name = "test_double", Required = true)]
            public double SomeDouble { get; set; }
            [Env(Name = "test_string", Required = true)]
            public string? SomeString { get; set; }
            [Env(Name = "test_bool", Required = true)]
            public bool SomeBool { get; set; }
            [Env(Name = "test_date", Required = false)]
            public DateTime SomeDateTime { get; set; }
            [Env(Name = "test_timespan", Required = true)]
            public TimeSpan SomeTimeSpan { get; set; }
        }

        [Environmental]
        class TestException
        {
            [Env(Name = "required_but_non_exist", Required = true)]
            public int NonExistent { get; set; }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            TestClass t = EnvMapper.Map<TestClass>(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "test.env"));

            Assert.That(t.SomeInt, Is.EqualTo(123));
            Assert.That(t.SomeFloat, Is.EqualTo(25.1f));
            Assert.That(t.SomeString, Is.EqualTo("somestring"));
            Assert.That(t.SomeBool, Is.EqualTo(true));
            Assert.That(t.SomeDouble, Is.EqualTo(12.5));
            Assert.That(t.SomeDateTime, Is.EqualTo(DateTime.Parse("2023-02-12")));
            Assert.That(t.SomeTimeSpan.Seconds, Is.EqualTo(6));

            Assert.Throws<Exception>(() => EnvMapper.Map<TestException>());
        }
    }
}