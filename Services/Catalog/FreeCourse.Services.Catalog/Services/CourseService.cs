using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using Mass=MassTransit;
using MongoDB.Driver;
using FreeCourse.Shared.Messages;


public class CourseService : ICourseService
{
    private readonly IMongoCollection<Course> _courseCollection;
    private readonly IMongoCollection<Category> _categoryCollection;
    private readonly IMapper _mapper;
    private readonly Mass.IPublishEndpoint _publishEndpoint;
    public CourseService(IMapper mapper, IDatabaseSetttings databaseSetttings, Mass.IPublishEndpoint publishEndpoint)
    {
        var client = new MongoClient(databaseSetttings.ConnectionStrings);
        var database = client.GetDatabase(databaseSetttings.DatabaseName);

        _courseCollection = database.GetCollection<Course>(databaseSetttings.CourseCollectionName);
        _categoryCollection = database.GetCollection<Category>(databaseSetttings.CategoryCollectionName);
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Response<List<CourseDto>>> GetAllAsync()
    {
        var courses = await _courseCollection.Find(course => true).ToListAsync();

        if (courses.Any())
        {
            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = new List<Course>();
        }

        return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
    }

    public async Task<Response<CourseDto>> GetByIdAsync(string id)
    {
        var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();
        if (course == null)
            return Response<CourseDto>.Fail("Course not found", 404);

        course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
        return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
    }

    public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
    {
        var courses = await _courseCollection.Find<Course>(x => x.UserId == userId).ToListAsync();

        if (courses.Any())
        {
            foreach (var course in courses)
            {
                course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
            }
        }
        else
        {
            courses = new List<Course>();
        }

        return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
    }

    public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
    {
        var newCourse = _mapper.Map<Course>(courseCreateDto);
        newCourse.CreatedDate = DateTime.Now;
        await _courseCollection.InsertOneAsync(newCourse);
        return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
    }

    public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
    {
        var updateCourse = _mapper.Map<Course>(courseUpdateDto);

        var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);
        if (result == null)
        {
            return Response<NoContent>.Fail("Course not found", 404);
        }

        // Publisher olarak course servisiayarladık
        await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent() { CourseId = updateCourse.Id, UpdatedNamed = courseUpdateDto.Name });

        //await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent() { CourseId = updateCourse.Id, UpdatedNamed = courseUpdateDto.Name });

        return Response<NoContent>.Success(204);
    }

    public async Task<Response<NoContent>> DeleteAsync(string id)
    {
        var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);
        if (result.DeletedCount > null)
            return Response<NoContent>.Success(204);

        return Response<NoContent>.Fail("Course not found", 404);

    }
}

