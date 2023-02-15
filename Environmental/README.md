Environmental is a mapper that maps envrionment variables/files to classes

### Example:

```
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
```

```
        var t = EnvMapper.Map<TestClass>();
```

## or

```
        TestClass t = EnvMapper.Map<TestClass>("test.env");
```