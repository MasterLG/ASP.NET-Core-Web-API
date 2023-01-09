using Microsoft.AspNetCore.Mvc;
using Cms.Data.Repository.Repositories;
using Cms.Data.Repository.Models;
using Cms.WebApi.DTOs;
using AutoMapper;

namespace Cms.WebApi.Controllers{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("Courses")]
    //[Route("v{version:ApiVersion}/Courses")]
    public class Courses2Controller:ControllerBase {
        private readonly ICmsRepository cmsRepository;
        private readonly IMapper mapper;

        public Courses2Controller(ICmsRepository cmsRepository, IMapper mapper){//IMapper mapper injection i için eklendi
            this.cmsRepository = cmsRepository;
            this.mapper = mapper;
        }
        //Approach 1
        // [HttpGet]
        // public IEnumerable<Course> GetCourses(){
        //     //return "Hellooo";
        //     return cmsRepository.GetAllCourses();
        // }

        //Approach 1
        // [HttpGet]
        // public IEnumerable<CourseDto> GetCourses(){
        //     //return "Hellooo";
        //     try
        //     {
        //         IEnumerable<Course> courses=cmsRepository.GetAllCourses();
        //         var result=MapCourseToCourseDto(courses);
        //         return result;
        //     }
        //     catch (System.Exception)
        //     {
                
        //         throw;
        //     }
        // }
        //Approach 2- IActionResult
        // public IActionResult GetCourses(){
        //     //return "Hellooo";
        //     try
        //     {
        //         IEnumerable<Course> courses=cmsRepository.GetAllCourses();
        //         var result=MapCourseToCourseDto(courses);
        //         return Ok(result);
        //     }
        //     catch (System.Exception ex)
        //     {
                
        //         return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
        //     }
        // }
        //Approach 3- ActionResult<T>
        //  public ActionResult<IEnumerable<CourseDto>> GetCourses(){
        //     //return "Hellooo";
        //     try
        //     {
        //         IEnumerable<Course> courses=cmsRepository.GetAllCourses();
        //         var result=MapCourseToCourseDto(courses);
        //         return result.ToList();//Convert to support ActionResult<T>
        //     }
        //     catch (System.Exception ex)
        //     {
                
        //         return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
        //     }
        // }
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCoursesAsync(){
            //return "Hellooo";
            try
            {
                IEnumerable<Course> courses= await cmsRepository.GetAllCoursesAsync();
                //var result=MapCourseToCourseDto(courses);//automapper eklendikten sonra commentlendi
                var result=mapper.Map<CourseDto[]>(courses);
                //version 2 changes
                foreach (var item in result)
                {
                    item.CourseName+=" (v2.0)";
                }


                return result.ToList();//Convert to support ActionResult<T>
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }
        [MapToApiVersion("3.0")]
         public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses_v3_Async(){
            try
            {
                IEnumerable<Course> courses= await cmsRepository.GetAllCoursesAsync();
                //var result=MapCourseToCourseDto(courses);//automapper eklendikten sonra commentlendi
                var result=mapper.Map<CourseDto[]>(courses);
                //version 2 changes
                foreach (var item in result)
                {
                    item.CourseName+=" (v3.0)";
                }


                return result.ToList();//Convert to support ActionResult<T>
            }
            catch (System.Exception ex)
            {
                
                return StatusCode(StatusCodes.Status500InternalServerError,ex.Message);
            }
        }


        [HttpPost]
        public ActionResult<CourseDto> AddCourse([FromBody]CourseDto course){

            try
            {
                var newCourse=mapper.Map<Course>(course);
                newCourse=cmsRepository.AddCourse(newCourse);
                return mapper.Map<CourseDto>(newCourse);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("{courseId}")]
        public ActionResult<CourseDto> GetCourse(int courseId){
            try
            {
                if(!cmsRepository.IsCourseExists(courseId))
                    return NotFound();
                
                Course course=cmsRepository.GetCourse(courseId);
                var result=mapper.Map<CourseDto>(course);
                return result;
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut("{courseId}")]
        public ActionResult<CourseDto> UpdateCourse(int courseId, CourseDto course){
            try{
                    if(!cmsRepository.IsCourseExists(courseId))
                        return NotFound();
                    Course updatedCourse=mapper.Map<Course>(course);
                    updatedCourse=cmsRepository.UpdateCourse(courseId, updatedCourse);
                    var result=mapper.Map<CourseDto>(updatedCourse);

                    return result;
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete("{courseId}")]
        public ActionResult<CourseDto> DeleteCourse(int courseId){
            try
            {
                if(!cmsRepository.IsCourseExists(courseId))
                    return NotFound();
                
                Course course=cmsRepository.DeleteCourse(courseId);
                if(course==null)
                    return BadRequest();
                var result=mapper.Map<CourseDto>(course);
                return result;
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //GET ../courses/1/students
        [HttpGet("{courseId}/students")]
        public ActionResult<IEnumerable<StudentDto>> GetStudents(int courseId){
            try
            {
                if(!cmsRepository.IsCourseExists(courseId))
                    return NotFound();
                
                IEnumerable<Student> students=cmsRepository.GetStudents(courseId);
                var result=mapper.Map<StudentDto[]>(students);
                return result;
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        //POST ../courses/1/students
        [HttpPost("{courseId}/students")]
        public ActionResult<StudentDto> AddStudent(int courseId, StudentDto student){
            try
            {
                if(!cmsRepository.IsCourseExists(courseId))
                    return NotFound();
                
                Student newStudent=mapper.Map<Student>(student);
                
                //Assign course
                Course course=cmsRepository.GetCourse(courseId);
                newStudent.Course=course;
                
                newStudent=cmsRepository.AddStudent(newStudent);
                var result=mapper.Map<StudentDto>(newStudent);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        //Custom mapper functions
        // private CourseDto MapCourseToCourseDto(Course course){
        //     return new CourseDto(){
        //         CourseId=course.CourseId;
        //         CourseName=course.CourseName;
        //         CourseDuration=course.CourseDuration;
        //         CourseType=(Cms.WebApi.DTOs.COURSE_TYPE)course.CourseType;
        //     };
        // }

        // private IEnumerable<CourseDto> MapCourseToCourseDto(IEnumerable<Course> courses){
        //     IEnumerable<CourseDto> result;
        //     result=courses.Select(c=> new CourseDto()
        //     {
        //         CourseId=c.CourseId,
        //         CourseName=c.CourseName,
        //         CourseDuration=c.CourseDuration,
        //         CourseType=(Cms.WebApi.DTOs.COURSE_TYPE)c.CourseType
        //     });
        //     return result;
        //     // return new CourseDto(){
        //     //     CourseId=course.CourseId;
        //     //     CourseName=course.CourseName;
        //     //     CourseDuration=course.CourseDuration;
        //     //     CourseType=(Cms.WebApi.DTOs.COURSE_TYPE)course.CourseType;
        //     // };
        // }
    }
}