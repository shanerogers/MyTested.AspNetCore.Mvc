﻿namespace MyTested.Mvc.Test.BuildersTests.AndTests
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Setups;
    using Setups.Controllers;
    using Xunit;

    public class AndProvideTestBuilderTests
    {
        [Fact]
        public void AndProvideShouldReturnProperController()
        {
            MyMvc
                .Controller<MvcController>()
                .Calling(c => c.BadRequestWithErrorAction())
                .ShouldReturn()
                .BadRequest()
                .WithErrorMessage()
                .ShouldPassFor()
                .TheController(controller =>
                {
                    Assert.NotNull(controller);
                    Assert.IsAssignableFrom<MvcController>(controller);
                });
        }
        
        [Fact]
        public void AndProvideTheControllerAttributesShouldReturnProperAttributes()
        {
            MyMvc
                .Controller<MvcController>()
                .ShouldHave()
                .Attributes()
                .ShouldPassFor()
                .TheControllerAttributes(attributes =>
                {
                    Assert.Equal(2, attributes.Count());
                });
        }

        [Fact]
        public void AndProvideShouldReturnProperActionName()
        {
            var actionName = MyMvc
                .Controller<MvcController>()
                .Calling(c => c.BadRequestWithErrorAction())
                .ShouldReturn()
                .BadRequest()
                .WithErrorMessage()
                .AndProvideTheActionName();

            Assert.Equal("BadRequestWithErrorAction", actionName);
        }

        [Fact]
        public void AndProvideShouldReturnProperActionAttributes()
        {
            var attributes = MyMvc
                .Controller<MvcController>()
                .Calling(c => c.VariousAttributesAction())
                .ShouldHave()
                .ActionAttributes()
                .AndProvideTheActionAttributes();

            Assert.Equal(6, attributes.Count());
        }

        [Fact]
        public void AndProvideShouldReturnProperActionResult()
        {
            var actionResult = MyMvc
                .Controller<MvcController>()
                .Calling(c => c.LocalRedirect("URL"))
                .ShouldReturn()
                .LocalRedirect()
                .AndProvideTheActionResult();

            Assert.NotNull(actionResult);
            Assert.IsAssignableFrom<LocalRedirectResult>(actionResult);
        }

        [Fact]
        public void AndProvideShouldReturnProperCaughtException()
        {
            var caughtException = MyMvc
                .Controller<MvcController>()
                .Calling(c => c.ActionWithException())
                .ShouldThrow()
                .Exception()
                .AndProvideTheCaughtException();

            Assert.NotNull(caughtException);
            Assert.IsAssignableFrom<NullReferenceException>(caughtException);
        }

        [Fact]
        public void AndProvideShouldThrowExceptionIfActionIsVoid()
        {
            Test.AssertException<InvalidOperationException>(
                () =>
                {
                    MyMvc
                        .Controller<MvcController>()
                        .Calling(c => c.EmptyActionWithException())
                        .ShouldHave()
                        .ValidModelState()
                        .AndProvideTheActionResult();
                }, 
                "Void methods cannot provide action result because they do not have return value.");
        }
    }
}
