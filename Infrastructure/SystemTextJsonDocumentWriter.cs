using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;

namespace Afisha.Graphql.Infrastructure
{
    public class Utf8DocumentWriter : IDocumentWriter
    {
        public JsonSerializerOptions Options { get; }

        public Utf8DocumentWriter()
        {
            Options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new ExecutionResultJsonFormatter() }
            };
        }

        public async Task WriteAsync<T>(Stream stream, T value)
        {
            await JsonSerializer.SerializeAsync(stream, value, Options);
        }

        public Task<IByteResult> WriteAsync<T>(T value)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, Options);

            var byteResult = new MyBytes(new ArraySegment<byte>(bytes));

            return Task.FromResult<IByteResult>(byteResult);
        }

        private class MyBytes : IByteResult
        {
            public MyBytes(ArraySegment<byte> result)
            {
                Result = result;
            }

            public void Dispose()
            {
            }

            public ArraySegment<byte> Result { get; }
        }

        public string Write(object value)
        {
            return JsonSerializer.Serialize(value, Options);
        }
    }

    public class ExecutionResultJsonFormatter : JsonConverter<ExecutionResult>
    {
        private void WriteExtensions(Utf8JsonWriter writer, ExecutionResult value, JsonSerializerOptions options)
        {
            writer.WritePropertyName("extensions");

            JsonSerializer.Serialize(writer, value.Extensions, options);
        }

        private void WriteData(Utf8JsonWriter writer, ExecutionResult value, JsonSerializerOptions options)
        {
            writer.WritePropertyName("data");

            JsonSerializer.Serialize(writer, value.Data, options);
        }

        private void WriteErrors(Utf8JsonWriter writer, ExecutionErrors errors, JsonSerializerOptions options, bool exposeExceptions)
        {
            writer.WritePropertyName(nameof(errors));
            writer.WriteStartArray();
            foreach (var error in errors)
            {
                writer.WriteStartObject();
                writer.WriteString("message", exposeExceptions ? error.ToString() : error.Message);
                if (error.Locations != null)
                {
                    writer.WritePropertyName("locations");
                    writer.WriteStartArray();
                    foreach (var location in error.Locations)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("line");
                        writer.WriteNumberValue(location.Line);
                        writer.WritePropertyName("column");
                        writer.WriteNumberValue(location.Column);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                }

                if (error.Path != null && error.Path.Any())
                {
                    writer.WritePropertyName("path");

                    JsonSerializer.Serialize(writer, error.Path, options);
                }

                WriteErrorExtensions(writer, error, options);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        private void WriteErrorExtensions(Utf8JsonWriter writer, ExecutionError error, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(error.Code) && error.Data.Count == 0)
            {
                return;
            }

            writer.WritePropertyName("extensions");
            writer.WriteStartObject();

            if (!string.IsNullOrWhiteSpace(error.Code))
            {
                writer.WriteString("code", error.Code);
            }

            if (error.Data.Count > 0)
            {
 
                writer.WritePropertyName("data");
                writer.WriteStartObject();
                foreach (var entry in error.DataAsDictionary)
                {
                    writer.WritePropertyName(entry.Key);

                    JsonSerializer.Serialize(writer, entry.Value, options);
                }
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }

        public override ExecutionResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ExecutionResult value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            var shouldWriteData = (value.Errors == null || value.Errors.Count == 0) && value.Data != null;
            var shouldWriteErrors = value.Errors != null && value.Errors.Count > 0;
            var shouldWriteExtensions = value.Data != null && value.Extensions != null && value.Extensions.Count > 0;
            if (shouldWriteData)
            {
                WriteData(writer, value, options);
            }

            if (shouldWriteErrors)
            {
                WriteErrors(writer, value.Errors, options, value.ExposeExceptions);
            }

            if (shouldWriteExtensions)
            {
                WriteExtensions(writer, value, options);
            }

            writer.WriteEndObject();
        }
    }
}
