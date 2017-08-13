namespace HomeWizard.Net
{
    internal class ApiResponse<T>
    {
        public string Status { get; set; }
        public string Version { get; set; }
        public ApiRequest Request { get; set; }
        public T Response { get; set; }
    }
}
