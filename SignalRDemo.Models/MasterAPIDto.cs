﻿

namespace StudentManagement_API.Models.Models.DTO
{

    public class MasterAPIIds
    {
        public int? StudentId { get; set; } = null;

        public int? CourseId { get; set; } = null;

    }

    public class ApiRequest
    {
        public string ControllerName { get; set; }

        public string MethodName { get; set; }

        public string DataObject { get; set; }

        public string? PageName { get; set; }

        public int? RoleId { get; set; } = null;

        public List<string>? RoleIds { get; set; } = null;

        public string? MethodType { get; set; } = null;

    }

    public class UpdateJwtDTo
    {
        public string token { get; set; }

        public int Id { get; set; }
    }


    public class CountStudentProfessorDto
    {
        public DateTime? CreatedDate1 { get; set; } = null;

        public DateTime? CreatedDate2 { get; set; } = null;

        public int? StudentDayWiseCount { get; set; } = null;

        public int? ProfessorDayWiseCount { get; set; } = null;

        public int? month { get; set; } = 0;

        public int? year { get; set; } = 0;
    }

    public class StudentListCountFromDateDto
    {

        public DateTime? CreatedDate { get; set; } = null;

        public int StudentsCount { get; set; }

        public DateOnly? FromDate { get; set; }

        public DateOnly? ToDate { get; set; }
    }
}