using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Contextual;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.Framework.Scenarios.Fluent;
using LightBDD.XUnit2;
using Microsoft.AspNetCore;
using OrdersApi.Validators;
using Xunit;

namespace OrdersApi.Tests
{
    [FeatureDescription("Add orders to event store")]
    public class OrderControllerTests : FeatureFixture
    {
        [Label("OFS-11")]
        [Scenario(DisplayName = "Valid order saved in event store")]
        [ScenarioCategory("Functional")]
        [MultiAssert]
        public async Task Valid_order_saved_in_event_store()
        {
            await Runner.WithContext<OrderControllerContext>()
                .NewScenario()
                .AddSteps(
                    given => given.Valid_order_request_is_received()
                )
                .AddAsyncSteps(
                    when => when.Order_is_submitted()
                )
                .AddSteps(
                    then => then.Order_accepted_response_is_returned()
                )
                .RunAsync();
        }

        [Label("OFS-11")]
        [Scenario(DisplayName = "Invalid email returns bad request with error message")]
        [ScenarioCategory("Validation")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("     ")]
        [InlineData("a.smilth@")]
        [InlineData("a.smilthgmail.com")]
        public async Task Invalid_email_returns_bad_request_with_error_message(string value)
        {
            await Runner.WithContext<OrderControllerContext>()
                .NewScenario()
                .AddSteps(
                    given => given.Model_has_email_set_to(value)
                )
                .AddAsyncSteps(
                    when => when.Order_is_submitted()
                )
                .AddSteps(
                    then => then.Then_event_data_is_Not_actioned_on_correct_event_stream(),
                    and => and.Then_bad_request_is_returned_with_value(
                        string.Format(AddOrderModelValidator.FormatErrorMessageEmailNotValid, value))
                )
                .RunAsync();
        }
    }
}