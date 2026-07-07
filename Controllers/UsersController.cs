using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using UserCacheApi.Services;

namespace UserCacheApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService userService, ILogger<UsersController> logger) : Controller
    {
        /// <summary>
        /// Gets all users from cache or external API
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of all users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await userService.GetUsersAsync(cancellationToken);
                return Ok(users);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error while getting users.");
                return Problem("The database is not available or has not been initialized.", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Third-party API error while getting users.");
                return Problem("The third-party user API is not available.", statusCode: StatusCodes.Status502BadGateway);
            }
        }

        /// <summary>
        /// Gets user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Specific user by Id</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid user Id.");
            }

            try
            {
                var user = await userService.GetUserByIdAsync(id, cancellationToken);
                return user is null ? NotFound($"User by id: {id} not found") : Ok(user);
            }
            catch (SqlException ex)
            {
                logger.LogError(ex, "Database error while getting user {UserId}.", id);
                return Problem("Database error while retrieving user details", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Unexpected error while getting user {UserId}.", id);
                return Problem("Unexpected error occured.", statusCode: StatusCodes.Status502BadGateway);
            }
        }
    }
}
