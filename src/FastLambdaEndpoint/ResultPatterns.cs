namespace FastLambdaEndpoint
{
    public class ResponseResult<T>
    {
        public List<string> Errors { get; private set; } = new List<string>();
        public T Data { get; private set; }
        public object HttpStatus { get; private set; }

        public ResponseResult(T data)
            : this(data, new List<string>()) { }
        private ResponseResult(T data, List<string> errors)
        {
            Data = data;
            Errors = errors;
        }

        public static ResponseResult<T> Success() => new ResponseResult<T>(default, new List<string>());
        public static ResponseResult<T> Success(T data) => new ResponseResult<T>(data, new List<string>());
        public static ResponseResult<T> Response(ValidationMessage message)
        {
            if (message.IsValid)
                return new ResponseResult<T>(default, new List<string>());
            return new ResponseResult<T>(default, new List<string>() { message.Message });
        }

        public static implicit operator ResponseResult<T>(T data) => Success(data);
        public static implicit operator ResponseResult<T>(ValidationMessage message) => Response(message);

        public static ResponseResult<T> NotFound(NotFound response) => new ResponseResult<T>(default, new List<string>() { response.Message }) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(NotFound response) => NotFound(response);

        public static ResponseResult<T> Ok(Success response) => new ResponseResult<T>(default, new List<string>()) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(Success response) => Ok(response);

        public static ResponseResult<T> Created(Created response) => new ResponseResult<T>(default, new List<string>()) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(Created response) => Created(response);

        public static ResponseResult<T> Deleted(Deleted response) => new ResponseResult<T>(default, new List<string>()) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(Deleted response) => Deleted(response);

        public static ResponseResult<T> Updated(Updated response) => new ResponseResult<T>(default, new List<string>()) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(Updated response) => Updated(response);

        public static ResponseResult<T> BadRequest(BadRequest response) => new ResponseResult<T>(default, new List<string>() { response.Message }) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(BadRequest response) => BadRequest(response);

        public static ResponseResult<T> Unauthorized(Unauthorized response) => new ResponseResult<T>(default, new List<string>() { response.Message }) { HttpStatus = response };
        public static implicit operator ResponseResult<T>(Unauthorized response) => Unauthorized(response);
    }

    public record struct BadRequest
    {
        public string Message { get; set; }
        public BadRequest(string message = "")
        {
            Message = message;
        }
    }
    public record struct NotFound
    {
        public string Message { get; set; }

        public NotFound()
            : this("Recurso não encontrado") { }
        public NotFound(string message)
        {
            Message = message;
        }
    }

    public record struct Unauthorized
    {
        public string Message { get; set; }
        public Unauthorized(string message = "")
        {
            Message = message;
        }
    }

    public readonly record struct Success;
    public readonly record struct Created;
    public readonly record struct Deleted;
    public readonly record struct Updated;
    public readonly record struct Response;

    public static class Result
    {
        public static Success Success => new Success();
        public static Created Created => new Created();
        public static Deleted Deleted => new Deleted();
        public static Updated Updated => new Updated();
        public static Unauthorized Unauthorized => new Unauthorized();
        public static BadRequest BadRequest => new BadRequest();
        public static NotFound NotFound => new NotFound();
    }

    public class ValidationMessage
    {
        public bool IsValid { get { return string.IsNullOrEmpty(Message); } }
        public string Message { get; set; }

        public ValidationMessage()
            : this(string.Empty) { }
        public ValidationMessage(string message)
        {
            Message = message;
        }
    }
}