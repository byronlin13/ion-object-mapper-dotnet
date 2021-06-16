using System;
using System.Collections.Generic;
using System.Text;
using Amazon.IonDotnet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Amazon.Ion.ObjectMapper.Test.Utils;

namespace Amazon.Ion.ObjectMapper.Test
{

    class HobbySerializerFactory : IonSerializerFactory<Hobby>
    {
        public IonSerializer<Hobby> create(IonSerializationOptions options, Dictionary<string, object> context)
        {
            return new HobbySerializer((Translator)context.GetValueOrDefault("translator", null));
        }
    }

    internal class Translator
    {
        public string ToMandarin(string english)
        {
            var d = new Dictionary<string, string>();
            d.Add("hello", "nihao");
            d.Add("running", "paobu");
            return d.GetValueOrDefault(english, "unknown");
        }

        public string ToEnglish(string mandarin)
        {
            var d = new Dictionary<string, string>();
            d.Add("nihao", "hello";
            d.Add("paobu", "running");
            return d.GetValueOrDefault(mandarin, "unknown");
        }
    }

    class HobbySerializer : IonSerializer<Hobby>
    {
        private readonly Translator translator;

        public HobbySerializer(Translator translator)
        {
            this.translator = translator;
        }

        public override Hobby Deserialize(IIonReader reader)
        {
            return new Hobby { Name = translator.ToMandarin(reader.StringValue()) };
        }

        public override void Serialize(IIonWriter writer, Hobby item)
        {
            writer.WriteString(translator.ToEnglish(item.Name));
        }
    }

    [IonSerializer(Factory = typeof(HobbySerializerFactory))]
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

            var ionSerializer = new IonSerializer(new IonSerializationOptions {
                Context = new Dictionary<string, object>() 
                {
                    { "translator", new Translator()}
                }
            });

            var stream = ionSerializer.Serialize(shuai);

            Assert.AreEqual("\n{\n  name: \"Shuai\",\n  hobby: \"Running\"\n}", Utils.PrettyPrint(stream));

            var deserialized = ionSerializer.Deserialize<Person>(stream);

            Assert.AreEqual(shuai.ToString(), deserialized.ToString());
        }
    }
}
