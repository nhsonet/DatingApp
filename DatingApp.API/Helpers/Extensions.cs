using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse httpResponse, string message)
        {
            httpResponse.Headers.Add("Application-Error", message);
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            httpResponse.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AddPagination(this HttpResponse httpResponse, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var PaginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();

            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            httpResponse.Headers.Add("Pagination", JsonConvert.SerializeObject(PaginationHeader, camelCaseFormatter));
            httpResponse.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;

            if (dateOfBirth.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}