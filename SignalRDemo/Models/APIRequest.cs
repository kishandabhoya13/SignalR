﻿

using static SignalRDemo.Models.APIMethodType;

namespace SignalRDemo.Models
{
    public class APIRequest
    {
        public APIType ApiType { get; set; } = APIType.GET;

        public string url { get; set; }

        public object Data { get; set; }
    }

    public class SecondApiRequest
    {
        public string ControllerName { get; set; }

        public string MethodName { get; set; }

        public string DataObject { get; set; }

        public string? PageName { get; set; }

        public int? RoleId { get; set; } = null;

        public string? MethodType { get; set; } = null;

        //public int CurrentPageNumber { get; set; } = 0;
        public int StartIndex { get; set; } = 0;


        public int PageSize { get; set; } = 5;

        public string? searchQuery { get; set; } = null;

        public string? OrderBy { get; set; } = null;

        public string? OrderDirection { get; set; } = null;

        public string token { get; set; }

        public List<string>? RoleIds { get; set; } = null;

        public DateOnly? FromDate { get; set; } = null;

        public DateOnly? ToDate { get; set; } = null;

    }

    public class BaseApiRequest<T>
    {
        public string ControllerName { get; set; }

        public string MethodName { get; set; }

        public int? RoleId { get; set; } = null;

        public string token { get; set; }

        public string? MethodType { get; set; } = null;

        public T dataObject { get; set; }

        public List<string>? RoleIds { get; set; } = null;

        public string? PageName { get; set; }


        public int Id { get; set; }


    }

    public class UpdateJwtViewModel
    {
        public string token { get; set; }

        public int Id { get; set; }
    }
}
