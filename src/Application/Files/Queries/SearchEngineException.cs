using System.Runtime.Serialization;

namespace ImageBrowser.Application.Files.Queries;
[Serializable]
internal class SearchEngineException : Exception
{
    public SearchEngineException()
    {
    }

    public SearchEngineException(string? message) : base(message)
    {
    }

    public SearchEngineException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected SearchEngineException(SerializationInfo info, StreamingContext context)/* : base(info, context)*/
    {
    }
}
