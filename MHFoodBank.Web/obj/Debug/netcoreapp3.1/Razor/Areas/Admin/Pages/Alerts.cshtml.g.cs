#pragma checksum "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "77fd25950ff50597c9182c824b286aee70daeefa"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(MHFoodBank.Web.Areas.Admin.Pages.Areas_Admin_Pages_Alerts), @"mvc.1.0.razor-page", @"/Areas/Admin/Pages/Alerts.cshtml")]
namespace MHFoodBank.Web.Areas.Admin.Pages
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
#line 2 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
using MHFoodBank.Web.Data.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"77fd25950ff50597c9182c824b286aee70daeefa", @"/Areas/Admin/Pages/Alerts.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"40450e91827ff24ed37b8f2dd361139389fa3450", @"/Areas/Admin/Pages/_ViewImports.cshtml")]
    public class Areas_Admin_Pages_Alerts : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("name", "_AdminStatusMessage", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-primary btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "ViewApplicant", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("btn btn-danger btn-sm"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "DeleteAlert", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "ViewRequest", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
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
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 4 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
  
    ViewData["Title"] = "Alerts";
    Layout = "~/Areas/Admin/Pages/Shared/_Layout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n    <div class=\"ml-5 mr-5\">\n        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("partial", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "77fd25950ff50597c9182c824b286aee70daeefa5934", async() => {
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.Name = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 10 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.StatusMessage);

#line default
#line hidden
#nullable disable
            __tagHelperExecutionContext.AddTagHelperAttribute("for", __Microsoft_AspNetCore_Mvc_TagHelpers_PartialTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"
        <!--<br />
        <form class=""row w-50 mb-3 ml-1"" method=""post"">
            if (String.IsNullOrWhiteSpace(Model.SearchedName))
            {
            <input class=""form-control col-md-4 mr-1"" placeholder=""Search by name..."" asp-for=""Model.SearchedName"" />
            }
            else
            {
            <input class=""form-control col-md-4 mr-1"" value=""Model.SearchedName"" asp-for=""Model.SearchedName"" />
            }
            <div class=""col-md-3"" method=""post"">
                <button class=""btn btn-sm btn-primary w-50 h-100"" asp-page-handler=""Search""><i class=""fas fa-search""></i></button>
            </div>
        </form>-->
        <h3>New Applicants</h3>
");
#nullable restore
#line 26 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
         if (Model.ApplicationAlerts.Count == 0)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <hr />\n            <p><strong>No new applicants</strong></p>\n");
#nullable restore
#line 30 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <table class=\"table\">\n                <tr>\n                    <th>Date</th>\n                    <th>From</th>\n                    <th>Actions</th>\n                </tr>\n");
#nullable restore
#line 39 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                 foreach (Alert alert in @Model.ApplicationAlerts)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\n");
#nullable restore
#line 42 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                         if (!alert.HasBeenRead)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td><strong>");
#nullable restore
#line 44 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                   Write(alert.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong></td>\n                            <td><strong>");
#nullable restore
#line 45 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                   Write(alert.Volunteer.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 45 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                              Write(alert.Volunteer.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong></td>\n");
#nullable restore
#line 46 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td>");
#nullable restore
#line 49 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                           Write(alert.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td><span>");
#nullable restore
#line 50 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                 Write(alert.Volunteer.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>&nbsp;<span>");
#nullable restore
#line 50 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                              Write(alert.Volunteer.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></td>\n");
#nullable restore
#line 51 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td>\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa12047", async() => {
                WriteLiteral("\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa12334", async() => {
                    WriteLiteral("See Applicant Information");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_2.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
                if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
                {
                    throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
                }
                BeginWriteTagHelperAttribute();
#nullable restore
#line 54 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                                                          WriteLiteral(alert.Id);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa14808", async() => {
                    WriteLiteral("Delete");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_4.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
                if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
                {
                    throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
                }
                BeginWriteTagHelperAttribute();
#nullable restore
#line 55 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                                                       WriteLiteral(alert.Id);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                        </td>\n                    </tr>\n");
#nullable restore
#line 59 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                </tr>\n            </table>\n");
#nullable restore
#line 63 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        <br />\n        <br />\n        <h3>Shift Requests</h3>\n        <br />\n        <h5>Pending Requests</h5>\n");
#nullable restore
#line 69 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
         if (Model.PendingRequests.Count < 1)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <hr />\n            <p><strong>No pending requests</strong></p>\n");
#nullable restore
#line 73 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <table class=\"table\">\n                <tr>\n                    <th>Date</th>\n                    <th>From</th>\n                    <th>Actions</th>\n                </tr>\n");
#nullable restore
#line 82 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                 foreach (Alert alert in @Model.PendingRequests)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\n");
#nullable restore
#line 85 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                         if (!alert.HasBeenRead)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td><strong>");
#nullable restore
#line 87 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                   Write(alert.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong></td>\n                            <td><strong>");
#nullable restore
#line 88 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                   Write(alert.Volunteer.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ");
#nullable restore
#line 88 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                              Write(alert.Volunteer.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</strong></td>\n");
#nullable restore
#line 89 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td>");
#nullable restore
#line 92 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                           Write(alert.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                            <td><span>");
#nullable restore
#line 93 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                 Write(alert.Volunteer.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>&nbsp;<span>");
#nullable restore
#line 93 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                              Write(alert.Volunteer.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></td>\n");
#nullable restore
#line 94 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td>\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa22836", async() => {
                WriteLiteral("\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa23123", async() => {
                    WriteLiteral("See Request");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_6.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
                if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
                {
                    throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
                }
                BeginWriteTagHelperAttribute();
#nullable restore
#line 97 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                                                        WriteLiteral(alert.Id);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                        </td>\n                    </tr>\n");
#nullable restore
#line 101 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                </tr>\n            </table>\n");
#nullable restore
#line 105 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("        <br />\n        <h5>Archived Requests</h5>\n");
#nullable restore
#line 108 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
         if (Model.ArchivedRequests.Count < 1)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <hr />\n            <p><strong>No archived requests</strong></p>\n");
#nullable restore
#line 112 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }
        else
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <table class=\"table\">\n                <tr>\n                    <th>Date</th>\n                    <th>Addressed By</th>\n                    <th>From</th>\n                    <th>Actions</th>\n                </tr>\n");
#nullable restore
#line 122 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                 foreach (Alert alert in @Model.ArchivedRequests)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <tr>\n                        <td>");
#nullable restore
#line 125 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                       Write(alert.Date);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n");
#nullable restore
#line 126 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                         if (alert is ShiftRequestAlert shiftRequestAlert)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td><span>");
#nullable restore
#line 128 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                 Write(shiftRequestAlert.AddressedBy);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></td>\n");
#nullable restore
#line 129 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <td></td>\n");
#nullable restore
#line 133 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <td><span>");
#nullable restore
#line 134 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                             Write(alert.Volunteer.FirstName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>&nbsp;<span>");
#nullable restore
#line 134 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                          Write(alert.Volunteer.LastName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></td>\n                        <td>\n                            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa30590", async() => {
                WriteLiteral("\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa30877", async() => {
                    WriteLiteral("See Request");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_1);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_6.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
                if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
                {
                    throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
                }
                BeginWriteTagHelperAttribute();
#nullable restore
#line 137 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                                                        WriteLiteral(alert.Id);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n                                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "77fd25950ff50597c9182c824b286aee70daeefa33336", async() => {
                    WriteLiteral("Delete");
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_4.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
                if (__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues == null)
                {
                    throw new InvalidOperationException(InvalidTagHelperIndexerAssignment("asp-route-id", "Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper", "RouteValues"));
                }
                BeginWriteTagHelperAttribute();
#nullable restore
#line 138 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                                                                                                       WriteLiteral(alert.Id);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"] = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-route-id", __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.RouteValues["id"], global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\n                            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_5.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_5);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\n                        </td>\n                    </tr>\n");
#nullable restore
#line 142 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                </tr>\n            </table>\n");
#nullable restore
#line 146 "/Users/brendanball/Downloads/Work/FBWebsite-V3/MHFoodBank.Web/Areas/Admin/Pages/Alerts.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MHFoodBank.Web.Areas.Admin.Pages.AlertsModel> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<MHFoodBank.Web.Areas.Admin.Pages.AlertsModel> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<MHFoodBank.Web.Areas.Admin.Pages.AlertsModel>)PageContext?.ViewData;
        public MHFoodBank.Web.Areas.Admin.Pages.AlertsModel Model => ViewData.Model;
    }
}
#pragma warning restore 1591
