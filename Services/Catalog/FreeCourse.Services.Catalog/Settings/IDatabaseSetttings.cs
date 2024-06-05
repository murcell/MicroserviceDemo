namespace FreeCourse.Services.Catalog.Settings
{
    public interface IDatabaseSetttings
    {
        public string CourseCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string ConnectionStrings { get; set; }
        public string DatabaseName { get; set; }
    }
}
