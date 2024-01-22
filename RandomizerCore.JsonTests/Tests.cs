using RandomizerCore.JsonTests.Mocks;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems.Templates;
using RandomizerCore.StringItems;

namespace RandomizerCore.JsonTests
{
    public class Tests
    {
        [Fact]
        public void ReadonlyStructRoundTripDeserializationTest()
        {
            RawLogicDef[] arr1 = [new("A", "B | C")];
            JsonUtil.DeserializeFromString<RawLogicDef[]>(JsonUtil.SerializeToString(arr1)).Should().Equal(arr1);

            RawWaypointDef[] arr2 = [new("A", "B | C", true), new("W", "X + Y")];
            JsonUtil.DeserializeFromString<RawWaypointDef[]>(JsonUtil.SerializeToString(arr2)).Should().Equal(arr2);

            LogicManagerBuilder lmb = new();
            Term t = lmb.GetOrAddTerm("T");
            TermValue[] arr3 = [new(t, 1)];
            JsonSerializer ls = JsonUtil.GetLogicSerializer(new(lmb));
            ls.SerializeToString(arr3);
            ls.DeserializeFromString<TermValue[]>(ls.SerializeToString(arr3)).Should().Equal(arr3);
        }

        [Fact]
        public void SmallLMSerializationTest()
        {
            LogicManagerBuilder lmb = new();
            lmb.GetOrAddTerm("A");
            lmb.GetOrAddTerm("B", TermType.Int);
            lmb.LP.SetMacro("M", "A + B");
            lmb.AddTransition(new("T", "T"));
            lmb.AddLogicDef(new("L", "W | T + M"));
            lmb.AddWaypoint(new("W", "T + A"));
            lmb.AddItem(new StringItemTemplate("I1", "A++ >> B+=2"));
            lmb.AddItem(new SingleItemTemplate("I2", ("B", 1)));
            lmb.AddItem(new BoolItemTemplate("I3", "A"));
            JsonUtil.SerializeToString(new LogicManager(lmb));
        }

        [Fact]
        public void MockBinderTest()
        {
            MockSerializationBinder binder = new();
            binder.BindToType("RandomizerMod", "RandomizerMod.RC.LogicGeoCost").Should().Be(typeof(LogicGeoCost));

            LogicManagerBuilder lmb = new();
            lmb.AddWaypoint(new("Can_Replenish_Geo", "TRUE", true));

            JsonSerializer js = JsonUtil.GetLogicSerializer(new(lmb));
            js.SerializationBinder = binder;
            js.DeserializeFromEmbeddedResource<object>(GetType().Assembly, "RandomizerCore.JsonTests.Resources.MockBinderTest.json")
                .Should().BeEquivalentTo(new LogicGeoCost { CanReplenishGeoWaypoint = lmb.GetOrAddTerm("Can_Replenish_Geo"), GeoAmount = 880 });
        }

        [Theory]
        [InlineData("2024-01-20")]
        [InlineData("2023-04-02")]
        [InlineData("2023-01-10")]
        [InlineData("2022-07-19")]
        [InlineData("2022-07-08")]
        [InlineData("2022-03-18")]
        public void CtxRoundTripNoError(string filename)
        {
            JsonSerializer js = JsonUtil.GetNonLogicSerializer();  
            js.SerializationBinder = new MockSerializationBinder();
            RandoModContext ctx = js.DeserializeFromEmbeddedResource<RandoModContext>(GetType().Assembly, $"RandomizerCore.JsonTests.Resources.{filename}.json");
            js.SerializeToString(ctx);
        }


        [Fact]
        public void CtxRoundTripIdentity()
        {
            JsonSerializer js = JsonUtil.GetNonLogicSerializer();
            js.SerializationBinder = new MockSerializationBinder();

            // the stable version differs from the original due to mock types
            // also, original float values do not round trip with nuget newtonsoft
#if NET472
            string filename = "2024-01-20-stable472";
#else
            string filename = "2024-01-20-stable";
#endif

            // not a true round trip since "stable" differs from orig in mock $types and certain float values
            using Stream s = GetType().Assembly.GetManifestResourceStream($"RandomizerCore.JsonTests.Resources.{filename}.json");
            using StreamReader sr = new(s);
            string json = sr.ReadToEnd();
            RandoModContext ctx = js.DeserializeFromString<RandoModContext>(json);
            //js.SerializeToFile("C:\\dev\\RandomizerCore.Json\\RandomizerCore.JsonTests\\Resources\\2024-01-20-stable.json", ctx);
            js.SerializeToString(ctx).Should().Be(json);
        }
        

    }
}