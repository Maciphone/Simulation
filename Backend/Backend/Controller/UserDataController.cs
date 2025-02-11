using Backend.Dto;
using Backend.MongoDb.Model;
using Backend.Service.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Backend.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserDataController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ISimulationStateRepository _simulationStateRepository;

    public UserDataController(IUserRepository userRepository, ISimulationStateRepository simulationStateRepository)
    {
        _userRepository = userRepository;
        _simulationStateRepository = simulationStateRepository;
    }
    
    [HttpGet("getAllUser")]
    public async Task<IActionResult> GetAllUser()
    {
        try
        {
            var user = await _userRepository.GetAllAsync();
            
            return Ok(user);
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($" Ismeretlen hiba: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("getUserById/{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
       
        catch (Exception ex)
        {
            Console.WriteLine($" Ismeretlen hiba: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("createUser")]
    public async Task<IActionResult> CreateUser([FromBody] UserDataDto userDataDto)
    {
        var userData = new UserData
        {
            Id = ObjectId.GenerateNewId().ToString(), 
            Name = userDataDto.Name,
            SimulationStateIds = userDataDto.SimulationStateIds,
            GamesPlayed = userDataDto.GamesPlayed,
            GamesWin = userDataDto.GamesWin
        };

        try
        {
            await _userRepository.CreateAsync(userData);
            return Ok(new { message = "User created successfully!", id = userData.Id });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unknown exception: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("updateUser/{userId}")]
    public async Task<IActionResult> UpdateUser([FromBody] UserData userData, string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            return NotFound("no such user");
        }

        try
        {
            await _userRepository.UpdateAsync(userId, userData);
            return Ok(new { message = "User updated successfully!" });
        }
       
        catch (Exception ex)
        {
            Console.WriteLine($"unknown ex: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("updateStatistic")]
    public async Task<IActionResult> UpdateUserStatistic(
      string userId, string type)
    {
        try
        {
            var update = await _userRepository.IncrementWinsAsync(userId, type);
            if (!update)
            {
                return BadRequest("could not update");
            }

            return Ok(new { message = "updated" });

        }
       
        catch (Exception ex)
        {
            Console.WriteLine($"unknown ex: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
        
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUserById(string userId)
    {
        try
        {
            var delete = await _userRepository.DeleteAsync(userId);
            if (!delete)
            {
                return NotFound("could not delete");
            }

            return Ok(new { userId = userId });

        }
        catch (Exception ex)
        {
            Console.WriteLine($"unknown ex: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("saveSimulation")]
    public async Task<IActionResult> SaveSimulation(SimulationState simulationState, string userId)
    {
        if (simulationState.Id == null)//not in db
        {
            simulationState.Id = ObjectId.GenerateNewId().ToString(); //generate id
            await _simulationStateRepository.CreateAsync(simulationState);
        }

        var pushed = await _userRepository.AddSimulationStateIdAsync(userId, simulationState.Id);
        if (!pushed)
        {
            return BadRequest("not found");
        }

        return Ok(new {simulationStateId = simulationState.Id});

    }

        
    
    
    
}