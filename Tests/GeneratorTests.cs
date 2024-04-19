using BinarySerializer;
using NUnit.Framework;
namespace Tests;

[TestFixture]
public class GeneratorTests
{
    [Test]
    public void TestSample()
    {
        var sample = new Sample
        {
            IntProperty = 1,
            LongProperty = 2,
            DateTimeProperty = DateTime.Now,
            StringProperty = "Test sample"
        };

        var stream = new TypedBinaryStream();
        sample.Serialize(stream);
        var buffer = stream.Buffer;

        stream = new TypedBinaryStream(buffer);
        var newSample = new Sample();
        newSample.Deserialize(stream);
        
        Assert.That(newSample.IntProperty==sample.IntProperty);
        Assert.That(newSample.LongProperty==sample.LongProperty);
        Assert.That(newSample.DateTimeProperty==sample.DateTimeProperty);
        Assert.That(newSample.StringProperty.Equals(sample.StringProperty));
    } 
}