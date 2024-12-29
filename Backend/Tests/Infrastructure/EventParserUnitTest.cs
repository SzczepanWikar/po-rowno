using System.Text.Json;
using EventStore.Client;
using Infrastructure.EventStore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Tests.Infrastructure
{
    public class EventParserUnitTest
    {
        public class TestEvent
        {
            public int Id { get; set; }
        }

        public class TestEvent2
        {
            public string Name { get; set; }
        }

        EventParser eventParser = new();

        [Fact]
        public void GetEventType_ExistingTypeName_ReturnsType()
        {
            var type = typeof(TestEvent);
            var name = nameof(TestEvent);

            var eventType = eventParser.GetEventType(name);

            Assert.Equal(type, eventType);
        }

        [Fact]
        public void GetEventType_NotExistingTypeName_ReturnsNull()
        {
            var name = nameof(TestEvent) + "Test";

            var eventType = eventParser.GetEventType(name);

            Assert.Null(eventType);
        }

        [Fact]
        public void GetEventData_CorrectJSONExisitingType_ReturnsNotNull()
        {
            var testEvent = new TestEvent() { Id = 1 };
            var json = JsonSerializer.SerializeToUtf8Bytes(testEvent);
            var readOnlyMemory = new ReadOnlyMemory<byte>(json);
            var @event = new EventRecord(
                "",
                new Uuid(),
                StreamPosition.Start,
                Position.Start,
                new Dictionary<string, string>()
                {
                    { "type", "" },
                    { "created", "123" },
                    { "content-type", "" },
                },
                readOnlyMemory,
                new ReadOnlyMemory<byte>()
            );
            var eventType = typeof(TestEvent);

            var result = eventParser.GetEventData(@event, eventType);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetEventData_CorrectJSONNotGivenType_ReturnsNotNull()
        {
            var testEvent = new TestEvent() { Id = 1 };
            var json = JsonSerializer.SerializeToUtf8Bytes(testEvent);
            var readOnlyMemory = new ReadOnlyMemory<byte>(json);
            var @event = new EventRecord(
                "",
                new Uuid(),
                StreamPosition.Start,
                Position.Start,
                new Dictionary<string, string>()
                {
                    { "type", nameof(TestEvent) },
                    { "created", "123" },
                    { "content-type", "" },
                },
                readOnlyMemory,
                new ReadOnlyMemory<byte>()
            );
            Type? eventType = null;

            var result = eventParser.GetEventData(@event, eventType);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetEventData_IncorrectJSON_ReturnsNull()
        {
            var testEvent = new TestEvent() { Id = 1 };
            var json = new byte[] { };
            var readOnlyMemory = new ReadOnlyMemory<byte>(json);
            var @event = new EventRecord(
                "",
                new Uuid(),
                StreamPosition.Start,
                Position.Start,
                new Dictionary<string, string>()
                {
                    { "type", nameof(TestEvent) },
                    { "created", "123" },
                    { "content-type", "" },
                },
                readOnlyMemory,
                new ReadOnlyMemory<byte>()
            );
            Type? eventType = typeof(TestEvent);

            var result = eventParser.GetEventData(@event, eventType);

            Assert.Null(result);
        }

        [Fact]
        public void GetEventData_CorrectJSONIncorrectType_ReturnsNotNull()
        {
            var testEvent = new TestEvent() { Id = 1 };
            var json = JsonSerializer.SerializeToUtf8Bytes(testEvent);
            var readOnlyMemory = new ReadOnlyMemory<byte>(json);
            var @event = new EventRecord(
                "",
                new Uuid(),
                StreamPosition.Start,
                Position.Start,
                new Dictionary<string, string>()
                {
                    { "type", nameof(TestEvent) },
                    { "created", "123" },
                    { "content-type", "" },
                },
                readOnlyMemory,
                new ReadOnlyMemory<byte>()
            );
            Type? eventType = typeof(TestEvent2);

            var result = eventParser.GetEventData(@event, eventType);

            Assert.NotNull(result);
        }
    }
}
