#pragma checksum "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "52cb196453dcf90cf584e99ff65cc2412d5ace40"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(MHFoodBank.Web.Areas.Volunteer.Pages.Areas_Volunteer_Pages_VolunteerCalendar), @"mvc.1.0.razor-page", @"/Areas/Volunteer/Pages/VolunteerCalendar.cshtml")]
namespace MHFoodBank.Web.Areas.Volunteer.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\_ViewImports.cshtml"
using Microsoft.AspNetCore.Identity;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\_ViewImports.cshtml"
using MHFoodBank.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\_ViewImports.cshtml"
using MHFoodBank.Web.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
using MHFoodBank.Common;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
using MHFoodBank.Common.Dtos;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
using Microsoft.AspNetCore.Antiforgery;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
using Syncfusion.EJ2;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"52cb196453dcf90cf584e99ff65cc2412d5ace40", @"/Areas/Volunteer/Pages/VolunteerCalendar.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"6e988aa77ce865085e4663c7ccc53a9808e16709", @"/Areas/Volunteer/Pages/_ViewImports.cshtml")]
    public class Areas_Volunteer_Pages_VolunteerCalendar : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("field", "PositionId", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("title", "Positions", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("idField", "Id", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("textField", "Name", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "Positions", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("header", "#headerTemplate", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("content", "#contentTemplate", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("footer", "#footerTemplate", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_8 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", "schedule", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_9 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("width", "100%", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_10 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("height", "100%", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_11 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("popupOpen", "onPopupOpen", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_12 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("created", "initializeDataManager", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Syncfusion.EJ2.Schedule.Schedule __Syncfusion_EJ2_Schedule_Schedule;
        private global::Syncfusion.EJ2.Schedule.ScheduleResources __Syncfusion_EJ2_Schedule_ScheduleResources;
        private global::Syncfusion.EJ2.Schedule.ScheduleResource __Syncfusion_EJ2_Schedule_ScheduleResource;
        private global::Syncfusion.EJ2.Schedule.ScheduleEventSettings __Syncfusion_EJ2_Schedule_ScheduleEventSettings;
        private global::Syncfusion.EJ2.Schedule.ScheduleQuickInfoTemplates __Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 8 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
  
    ViewData["Title"] = "VolunteerCalendar";
    Layout = "~/Areas/Volunteer/Pages/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<script>\r\n    var shifts;\r\n</script>\r\n\r\n");
#nullable restore
#line 17 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
  
    var shiftManager = new DataManager
    {
        Adaptor = "UrlAdaptor",
        Url = "VolunteerCalendar?handler=GetShifts",
        CrossDomain = true,
        Headers = new object[] { new { RequestVerificationToken = antiForgery.GetTokens(HttpContext).RequestToken } }
    };

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<div style=\"width: 97%; height: 85vh; margin: auto;\">\r\n    <button type=\"button\" onclick=\"showOpenShifts()\">show open shifts</button>\r\n    <button type=\"button\" onclick=\"showPrivateShifts()\">show private shifts</button>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("ejs-schedule", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "52cb196453dcf90cf584e99ff65cc2412d5ace409860", async() => {
                WriteLiteral("\r\n        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("e-schedule-resources", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "52cb196453dcf90cf584e99ff65cc2412d5ace4010134", async() => {
                    WriteLiteral("\r\n            ");
                    __tagHelperExecutionContext = __tagHelperScopeManager.Begin("e-schedule-resource", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "52cb196453dcf90cf584e99ff65cc2412d5ace4010429", async() => {
                    }
                    );
                    __Syncfusion_EJ2_Schedule_ScheduleResource = CreateTagHelper<global::Syncfusion.EJ2.Schedule.ScheduleResource>();
                    __tagHelperExecutionContext.Add(__Syncfusion_EJ2_Schedule_ScheduleResource);
#nullable restore
#line 32 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
__Syncfusion_EJ2_Schedule_ScheduleResource.DataSource = Model.Positions;

#line default
#line hidden
#nullable disable
                    __tagHelperExecutionContext.AddTagHelperAttribute("dataSource", __Syncfusion_EJ2_Schedule_ScheduleResource.DataSource, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                    __Syncfusion_EJ2_Schedule_ScheduleResource.Field = (string)__tagHelperAttribute_0.Value;
                    __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
                    __Syncfusion_EJ2_Schedule_ScheduleResource.Title = (string)__tagHelperAttribute_1.Value;
                    __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
#nullable restore
#line 32 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
__Syncfusion_EJ2_Schedule_ScheduleResource.AllowMultiple = false;

#line default
#line hidden
#nullable disable
                    __tagHelperExecutionContext.AddTagHelperAttribute("allowMultiple", __Syncfusion_EJ2_Schedule_ScheduleResource.AllowMultiple, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                    __Syncfusion_EJ2_Schedule_ScheduleResource.IdField = (string)__tagHelperAttribute_2.Value;
                    __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
                    __Syncfusion_EJ2_Schedule_ScheduleResource.TextField = (string)__tagHelperAttribute_3.Value;
                    __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_3);
                    __Syncfusion_EJ2_Schedule_ScheduleResource.Name = (string)__tagHelperAttribute_4.Value;
                    __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
                    await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                    if (!__tagHelperExecutionContext.Output.IsContentModified)
                    {
                        await __tagHelperExecutionContext.SetOutputContentAsync();
                    }
                    Write(__tagHelperExecutionContext.Output);
                    __tagHelperExecutionContext = __tagHelperScopeManager.End();
                    WriteLiteral("\r\n        ");
                }
                );
                __Syncfusion_EJ2_Schedule_ScheduleResources = CreateTagHelper<global::Syncfusion.EJ2.Schedule.ScheduleResources>();
                __tagHelperExecutionContext.Add(__Syncfusion_EJ2_Schedule_ScheduleResources);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("e-schedule-eventsettings", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "52cb196453dcf90cf584e99ff65cc2412d5ace4014173", async() => {
                }
                );
                __Syncfusion_EJ2_Schedule_ScheduleEventSettings = CreateTagHelper<global::Syncfusion.EJ2.Schedule.ScheduleEventSettings>();
                __tagHelperExecutionContext.Add(__Syncfusion_EJ2_Schedule_ScheduleEventSettings);
#nullable restore
#line 34 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
__Syncfusion_EJ2_Schedule_ScheduleEventSettings.DataSource = shiftManager;

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("dataSource", __Syncfusion_EJ2_Schedule_ScheduleEventSettings.DataSource, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n        ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("e-schedule-quickinfotemplates", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "52cb196453dcf90cf584e99ff65cc2412d5ace4015643", async() => {
                    WriteLiteral("\r\n        ");
                }
                );
                __Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates = CreateTagHelper<global::Syncfusion.EJ2.Schedule.ScheduleQuickInfoTemplates>();
                __tagHelperExecutionContext.Add(__Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates);
                __Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates.Header = (string)__tagHelperAttribute_5.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
                __Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates.Content = (string)__tagHelperAttribute_6.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
                __Syncfusion_EJ2_Schedule_ScheduleQuickInfoTemplates.Footer = (string)__tagHelperAttribute_7.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_7);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    ");
            }
            );
            __Syncfusion_EJ2_Schedule_Schedule = CreateTagHelper<global::Syncfusion.EJ2.Schedule.Schedule>();
            __tagHelperExecutionContext.Add(__Syncfusion_EJ2_Schedule_Schedule);
            __Syncfusion_EJ2_Schedule_Schedule.Id = (string)__tagHelperAttribute_8.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_8);
#nullable restore
#line 30 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
__Syncfusion_EJ2_Schedule_Schedule.SelectedDate = DateTime.Now;

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("selectedDate", __Syncfusion_EJ2_Schedule_Schedule.SelectedDate, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Syncfusion_EJ2_Schedule_Schedule.Width = (string)__tagHelperAttribute_9.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_9);
            __Syncfusion_EJ2_Schedule_Schedule.Height = (string)__tagHelperAttribute_10.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_10);
#nullable restore
#line 30 "C:\Users\dofus\Downloads\VS Proj\FB-HR-Manager\MHFoodBank.Web\Areas\Volunteer\Pages\VolunteerCalendar.cshtml"
__Syncfusion_EJ2_Schedule_Schedule.Readonly = false;

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("readonly", __Syncfusion_EJ2_Schedule_Schedule.Readonly, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            __Syncfusion_EJ2_Schedule_Schedule.PopupOpen = (string)__tagHelperAttribute_11.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_11);
            __Syncfusion_EJ2_Schedule_Schedule.Created = (string)__tagHelperAttribute_12.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_12);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

</div>

<script>
    var dataManager;
    function initializeDataManager() {
        let schedule = document.getElementById('schedule').ej2_instances[0];
        dataManager = schedule.dataModule.dataManager;
    }

    function showOpenShifts() {
        let schedule = document.getElementById('schedule').ej2_instances[0];
        dataManager.executeQuery(new ej.data.Query().addParams('type', 'open')).then(function (e) {
            schedule.eventSettings.dataSource = e.result;
            schedule.dataBind();
        });
        schedule.currentView = ""TimelineDay"";
        schedule.views = ['TimelineDay', 'TimelineWeek', 'TimelineMonth'];
        schedule.group.resources = [""Positions""];
    }

    function showPrivateShifts() {
        let schedule = document.getElementById('schedule').ej2_instances[0];
        dataManager.executeQuery(new ej.data.Query().addParams('type', 'private')).then(function (e) {
            schedule.eventSettings.dataSource = e.result;
            sched");
            WriteLiteral(@"ule.dataBind();
        });
        schedule.currentView = ""Week"";
        schedule.views = ['Day', 'Week', 'Month'];
    }

</script>

<script id=""headerTemplate"" type=""text/x-template"">
    ${if (elementType !== 'cell')}
    <div class=""e-header-icon-wrapper"">
        <button class=""e-close"" title=""CLOSE""></button>
    </div>
    <div class=""e-subject-wrap"">
        ${if (Subject)}
        <div class=""e-subject e-text-ellipsis"">${Subject}</div>
        ${/if}
    </div>
    ${/if}
</script>
<script id=""contentTemplate"" type=""text/x-template"">
    <div>
        ${if (elementType !== ""cell"")}
        <div class=""e-cell-content"">
            ${if (StartTime)}
                <input id=""StartTime"" class=""e-field"" type=""text"" name=""StartTime"" />
            ${/if}
            ${if (Position)}
                <div class=""e-resource""><div class=""e-resource-icon e-icons""></div><div class=""e-resource-details e-text-ellipsis"">${Position}</div></div>
            ${/if}
            <button");
            WriteLiteral(@" class=""e-control e-btn e-lib e-primary e-event-save e-flat e-center"">Work This Shift</button>
        </div>
        ${/if}
    </div>
</script>
<script id=""footerTemplate"" type=""text/x-template"">
    <div>
        ${if (elementType === 'cell')}
        <div class=""e-cell-footer"">
            <button class=""e-event-details"" title=""Extra Details"">Extra Details</button>
            <button class=""e-event-create"" title=""Add"">Add</button>
        </div>
        ${else}
        <div class=""e-event-footer"">
            <button class=""e-event-edit"" title=""Edit"">Edit</button>
            <button class=""e-event-delete"" title=""Delete"">Delete</button>
        </div>
        ${/if}
    </div>
</script>

<script>
    function onPopupOpen(args) {
        if (args.type === ""QuickInfo"") {
            var startTimeElement = args.element.querySelector('#StartTime');
            if (startTimeElement.classList.contains('e-field')) {
                startTimeObject = new DateTimePicker({
              ");
            WriteLiteral("      value: startTimeElement.value,\r\n                    allowEdit: false,\r\n                });\r\n                startTimeObject.appendTo(startTimeElement);\r\n            }\r\n        }\r\n    }\r\n</script>\r\n\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public IAntiforgery antiForgery { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MHFoodBank.Web.Areas.Volunteer.Pages.VolunteerCalendarModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<MHFoodBank.Web.Areas.Volunteer.Pages.VolunteerCalendarModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<MHFoodBank.Web.Areas.Volunteer.Pages.VolunteerCalendarModel>)PageContext?.ViewData;
        public MHFoodBank.Web.Areas.Volunteer.Pages.VolunteerCalendarModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
