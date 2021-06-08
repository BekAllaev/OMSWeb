using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Wrappers
{
    /// <summary>
    /// Base class for wrappers which contains metadata of response
    /// </summary>
    /// <typeparam name="T">
    /// Type of data response
    /// </typeparam>
    public class Response<T>
    {
        public Response()
        {
        }

        public Response(T data)
        {
            Succeeded = true;
            Message = string.Empty;
            Errors = null;
            Data = data;
        }

        public T Data { get; set; }
        public bool Succeeded { get; set; }
        public string[] Errors { get; set; }
        public string Message { get; set; }
    }
}
