using System;
using System.Collections.Generic;
using System.Text;
using Amazon.IonDotnet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Amazon.Ion.ObjectMapper.Test.Utils;

namespace Amazon.Ion.ObjectMapper.Test
{
    class HobbySerializer : IonSerializer<Hobby>
    {
        public override Hobby Deserialize(IIonReader reader)
        {
            return new Hobby { Name = reader.StringValue() };
        }

        public override void Serialize(IIonWriter writer, Hobby item)
        {
            writer.WriteString(item.Name);
        }
    }

    [IonSerializer(Serializer = typeof(HobbySerializer))]
    class Hobby
    {
        public string Name {get; set;}

        public override string ToString()
        {
            return $"Hobby {{Name={Name}}}";
        }
    }

    class Person {
        public string Name {get; set;}
        public Hobby Hobby {get; set;}

        public override string ToString()
        {
            return $"Person {{Name={Name}, Hobby={Hobby} }}";
        }
    }

    [TestClass]
    public class CustomIonSerializerTest
    {
        [TestMethod]
        public void CanUseACustomSerializerAnnotation()
        {
            var shuai = new Person { Name = "Shuai", Hobby = new Hobby { Name = "Running"} };

            var ionSerializer = new IonSerializer();

            var stream = ionSerializer.Serialize(shuai);

            Assert.AreEqual("{\nname: \"Shuai\", \nhobby: \"Running\"\n}", Utils.PrettyPrint(stream));

            var deserialized = ionSerializer.Deserialize<Person>(stream);

            Assert.AreEqual(shuai, deserialized);
        }
    }
}
