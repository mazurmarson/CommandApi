using System;
using System.Collections.Generic;
using AutoMapper;
using CommandAPI.Controllers;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using CommandAPI.Profiles;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CommandAPI.Tests
{
    public class CommandControllerTests : IDisposable
    {

        Mock<ICommandAPIRepo> mockRepo;
        CommandsProfile realProfile;
        MapperConfiguration configuration;
        IMapper mapper;

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            configuration = null;
            realProfile = null;
        }

        public CommandControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();
            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg => cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);
        }

        [Fact]
        public void GetCommandItems_Returns200OK_WhenDBIsEmpty()
        {
            //Arrange
            var mockRepo = new Mock<ICommandAPIRepo>();
            mockRepo.Setup(repo =>
            repo.GetAllCommands()).Returns(GetCommands(0));

            var realProfile = new CommandsProfile();
            var configuration = new MapperConfiguration(cfg =>
            cfg.AddProfile(realProfile));
            IMapper mapper = new Mapper(configuration);

            var controller = new CommandsController(mockRepo.Object, mapper);

            //Act
            var result = controller.GetAllCommands();
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0)
            {
                commands.Add(new Command
                {
                    Id = 0,
                    HowTo = "How to generate a migration",
                    CommandLine = "dotnet ef migrations add <Name of Migration>",
                    Platform = ".Net Core EF"
                });
            }
            return commands;
        }

        [Fact]
        public void GetAllCommands_ReturnsOneItem_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.GetAllCommands();
            //Assert
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as List<CommandReadDto>;
            Assert.Single(commands);
        }

        [Fact]
        public void GetAllCommands_Returns200OK_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.GetAllCommands();
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetAllCommands_ReturnsCorrectType_WhenDBHasOneResource()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetAllCommands()).Returns(GetCommands(1));
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.GetAllCommands();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        [Fact]
        public void GetCommandByID_Returns404NotFound_WhenNonExistentIDProvided()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.GetCommandById(1);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_Returns200OK_WhenValidIDProvided()
        {
            mockRepo.Setup(x => x.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);
            //ACT
            var result = controller.GetCommandById(1);
            //Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandById_ReturnsCorrectType_WhenValidIDProvided()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetCommandById(1)).Returns(new Command
          {
              Id = 1,
              HowTo = "mock",
              Platform = "Mock",
              CommandLine = "Mock"
          });
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.GetCommandById(1);
            //Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);

        }

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetCommandById(1)).Returns(new Command
          {
              Id = 1,
              HowTo = "mock",
              Platform = "Mock",
              CommandLine = "Mock"
          });
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.CreateCommand(new CommandCreateDto { });
            //Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        }

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.CreateCommand(new CommandCreateDto
            {

            });

            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        [Fact]
        public void UpdateCommant_Return204NoContent_WhenValidObjectSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command
            {
                Id = 1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);
            var result = controller.UpdateCommand(1, new CommandUpdateDto
            {

            });

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateCommand_Return404NotFound_WhenNonExistentResourdeIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);
            var result = controller.UpdateCommand(0, new CommandUpdateDto { });

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void PartialCommandUpdate_Returns404NotFound_WhenNonExistentResourceIDSubmitted()
        {
            //Arrange
            mockRepo.Setup(repo =>
          repo.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);
            //Act
            var result = controller.PartialCommandUpdate(0,
          new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<CommandUpdateDto> { });

          Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns204NoContent_WhenValidResourceIDSubmitter()
        {
            mockRepo.Setup(repo => repo.GetCommandById(1)).Returns(new Command {
                Id =1,
                HowTo = "mock",
                Platform = "Mock",
                CommandLine = "Mock"
            });

            var controller = new CommandsController(mockRepo.Object, mapper);

            var result = controller.DeleteCommand(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns404NotFound_WhenNonExistantResourceIDSubmitted()
        {
            mockRepo.Setup(repo => repo.GetCommandById(0)).Returns(() => null);
            var controller = new CommandsController(mockRepo.Object, mapper);
            var result = controller.DeleteCommand(0);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}